using System;
using System.Linq;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementAPI.DTOs;
using AutoMapper;
using EventManagementAPI.Repo;
using System.Globalization;
namespace EventManagementAPI.Services
{
    public class EventReportService : IEventReportService
    {
        private readonly IEventRepo _eventRepo;
        private readonly IAttendeeRepo _attendeeRepo;
        private readonly IMapper _mapper;
        private readonly HttpClient _http; // -> Integrate with an external API

        public EventReportService(IEventRepo eventRepo, IMapper mapper, IHttpClientFactory httpFactory, IAttendeeRepo attendeeRepo)
        {
            _eventRepo = eventRepo;
            _mapper = mapper;
            _http = httpFactory.CreateClient();
            _attendeeRepo = attendeeRepo;

            // Set default headers
            _http.Timeout = TimeSpan.FromSeconds(15);
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("EventManagementAPI/1.0");
        }

        // Returns upcoming events (within 'days') with attendee counts and a weather snippet
        public async Task<List<UpcomingEventReportDto>> GetUpcomingWithWeatherAsync(int days)
        {
            var now = DateTime.UtcNow;
            var max = now.AddDays(days);

            var events = (await _eventRepo.GetAllEventsWithAttendeesAsync())
                .Where(e => e.Date >= now && e.Date <= max)
                .OrderBy(e => e.Date)
                .ToList();


            var result = new List<UpcomingEventReportDto>(events.Count);

            foreach (var ev in events)
            {
                var attendees = await _attendeeRepo.GetAttendeesByEventIdAsync(ev.EventId);
                var count = (attendees != null && attendees.Count > 0) ? attendees.Count : ev.Attendees.Count;

                string weather;
                try { weather = await GetWeatherAsync(_http, ev.Location, ev.Date); }
                catch { weather = "Weather: unavailable"; }

                result.Add(new UpcomingEventReportDto
                {
                    EventId = ev.EventId,
                    Title = ev.Title,
                    Date = ev.Date,
                    Location = ev.Location,
                    MaxAttendees = ev.MaxAttendees,
                    AttendeeCount = count,
                    WeatherSnippet = weather
                });
            }

            return result;
        }

        // ------- Helpers --------




        private static string CodeToText(int code) => code switch
        {
            0 => "Clear",
            1 or 2 => "Mostly clear",
            3 => "Overcast",
            45 or 48 => "Fog",
            51 or 53 or 55 => "Drizzle",
            61 or 63 or 65 => "Rain",
            80 or 81 or 82 => "Showers",
            95 => "Thunderstorm",
            _ => "Weather"
        };

        private async Task<string> GetWeatherAsync(HttpClient http, string location, DateTime whenUtc)
        {
            // Open-Meteo free forecast ≈ next ~16 days only
            var today = DateTime.UtcNow.Date;
            var target = whenUtc.ToUniversalTime().Date;
            var delta = (target - today).TotalDays;
            if (delta > 80) return "Weather: out of range";
            if (delta < 0) return "Weather: past date";

            // 1) Geocode name -> lat/lon
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?count=1&language=en&name={Uri.EscapeDataString(location)}";
            var geo = await http.GetFromJsonAsync<GeoSearchResponse>(geoUrl);
            var g = geo?.results?.FirstOrDefault();
            if (g is null) return "Weather: location not found";

            // 2) Daily forecast for that day (UTC + invariant formatting)
            var date = target.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var lat = g.latitude.ToString(CultureInfo.InvariantCulture);
            var lon = g.longitude.ToString(CultureInfo.InvariantCulture);

            var wxUrl =
                $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}" +
                $"&daily=temperature_2m_max,temperature_2m_min,weathercode&timezone=UTC" +
                $"&start_date={date}&end_date={date}";

            var wx = await http.GetFromJsonAsync<DailyForecastResponse>(wxUrl);
            if (wx?.daily?.time is null || wx.daily.time.Length == 0) return "Weather: n/a";

            var code = wx.daily.weathercode?[0] ?? -1;
            var tmin = wx.daily.temperature_2m_min?[0];
            var tmax = wx.daily.temperature_2m_max?[0];

            var text = CodeToText(code);
            return (tmin is null || tmax is null) ? text : $"{text}, {tmin:0.#}–{tmax:0.#}°C";
        }



    }
}
