using System;
using System.Linq;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventManagementAPI.DTOs;
using AutoMapper;
using EventManagementAPI.DTOs;
using EventManagementAPI.Repo;
namespace EventManagementAPI.Services
{
    public class EventReportService : IEventReportService
    {
        private readonly IEventRepo _eventRepo;
        private readonly IAttendeeRepo _attendeeRepo;
        private readonly IMapper _mapper;
        private readonly HttpClient _http;

        public EventReportService(IEventRepo eventRepo, IMapper mapper, IHttpClientFactory httpFactory, IAttendeeRepo attendeeRepo)
        {
            _eventRepo = eventRepo;
            _mapper = mapper;
            _http = httpFactory.CreateClient();
            _attendeeRepo = attendeeRepo;
        }

        // Returns upcoming events (within 'days') with attendee counts and a weather snippet
        public async Task<List<UpcomingEventReportDto>> GetUpcomingWithWeatherAsync(int days = 30)
        {
            var now = DateTime.UtcNow;
            var max = now.AddDays(days);

            var events = (await _eventRepo.GetAllEventsWithAttendeesAsync())
                .Where(e => e.Date >= now && e.Date <= max)
                .OrderBy(e => e.Date)
                .ToList();

            // cache for geo lookups
            var geoCache = new Dictionary<string, (double lat, double lon)>(StringComparer.OrdinalIgnoreCase);

            var result = new List<UpcomingEventReportDto>(events.Count);

            foreach (var ev in events)
            {
                // Combine attendee counts (repo + nav as fallback)
                var attendeesFromRepo = await _attendeeRepo.GetAttendeesByEventIdAsync(ev.EventId);
                var attendeeCount = (attendeesFromRepo != null && attendeesFromRepo.Count > 0)
                    ? attendeesFromRepo.Count
                    : ev.Attendees.Count;

                string weatherSnippet;
                try
                {
                    var coords = await GetLatLonAsync(ev.Location, geoCache);
                    if (coords is null)
                    {
                        weatherSnippet = "Weather: location not found";
                    }
                    else
                    {
                        var (lat, lon) = coords.Value;
                       // weatherSnippet = await GetDailyWeatherSnippetAsync(lat, lon, ev.Date.Date);
                    }
                }
                catch
                {
                    weatherSnippet = "Weather: unavailable";
                }

                result.Add(new UpcomingEventReportDto
                {
                    EventId = ev.EventId,
                    Title = ev.Title,
                    Date = ev.Date,
                    Location = ev.Location,
                    MaxAttendees = ev.MaxAttendees,
                    AttendeeCount = attendeeCount,
                    //WeatherSnippet = weatherSnippet
                });
            }

            return result;
        }

        // -------- Helpers --------

        private async Task<(double lat, double lon)?> GetLatLonAsync(string location, Dictionary<string, (double, double)> cache)
        {
            if (string.IsNullOrWhiteSpace(location)) return null;

            if (cache.TryGetValue(location, out var cached))
                return cached;

            // Open-Meteo Geocoding API
            var url = $"https://geocoding-api.open-meteo.com/v1/search?count=1&language=en&name={Uri.EscapeDataString(location)}";
            var resp = await _http.GetFromJsonAsync<GeoSearchResponse>(url); // ✅ correct DTO

            var first = resp?.results?.FirstOrDefault();
            if (first == null) return null;

            var tuple = (first.latitude, first.longitude);
            cache[location] = tuple;
            return tuple;
        }

        //private async Task<string> GetDailyWeatherSnippetAsync(double lat, double lon, DateTime dateUtc)
        //{
        //    var date = dateUtc.ToString("yyyy-MM-dd");
        //    // Daily request for the event’s date: temps + weather code
        //    var url =
        //        $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}" +
        //        $"&daily=temperature_2m_max,temperature_2m_min,weathercode&timezone=auto" +
        //        $"&start_date={date}&end_date={date}";

        //    var resp = await _http.GetFromJsonAsync<DailyForecastResponse>(url);
        //    //if (resp?.daily == null || resp.daily.time is null || resp.daily.time.Length == 0)
        //    //    return "Weather: n/a";

        //    //// Single-day arrays because start_date == end_date
        //    //var tmin = SafeGet(resp.daily.temperature_2m_min, 0);
        //    //var tmax = SafeGet(resp.daily.temperature_2m_max, 0);
        //    //var code = SafeGet(resp.daily.weathercode, 0);

        //    //var desc = WeatherCodeToText(code);
        //    //if (double.IsNaN(tmin) || double.IsNaN(tmax))
        //    //    return desc;

        //    //return $"{desc}, {tmin:0.#}–{tmax:0.#}°C";
        //}

        private static double SafeGet(double[]? arr, int idx)
            => (arr != null && idx >= 0 && idx < arr.Length) ? arr[idx] : double.NaN;

        private static int SafeGet(int[]? arr, int idx)
            => (arr != null && idx >= 0 && idx < arr.Length) ? arr[idx] : -1;

        // Minimal mapping for common Open-Meteo weather codes
        private static string WeatherCodeToText(int code) => code switch
        {
            0 => "Clear",
            1 or 2 => "Mainly clear/partly cloudy",
            3 => "Overcast",
            45 or 48 => "Fog",
            51 or 53 or 55 => "Drizzle",
            56 or 57 => "Freezing drizzle",
            61 or 63 or 65 => "Rain",
            66 or 67 => "Freezing rain",
            71 or 73 or 75 => "Snow",
            77 => "Snow grains",
            80 or 81 or 82 => "Rain showers",
            85 or 86 => "Snow showers",
            95 => "Thunderstorm",
            96 or 99 => "Thunderstorm with hail",
            _ => $"Weather code {code}"
        };


    }
}
