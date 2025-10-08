using AutoMapper;
using EventManagementAPI.DTOs;
using EventManagementAPI.Repo;

namespace EventManagementAPI.Services
{
    public class EventReportService : IEventReportService
    {
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;

        public EventReportService(IEventRepo eventRepo, IMapper mapper)
        {
            _eventRepo = eventRepo;
            _mapper = mapper;
        }

        public async Task<List<UpcomingEventReportDto>> GetUpcomingWithWeatherAsync(int days = 30)
        {
            var now = DateTime.UtcNow;
            var max = now.AddDays(days);

            var events = (await _eventRepo.GetAllEventsWithAttendeesAsync())
                .Where(e => e.Date >= now && e.Date <= max)
                .OrderBy(e => e.Date)
                .ToList();

            var result = new List<UpcomingEventReportDto>();

            foreach (var ev in events)
            {
                result.Add(new UpcomingEventReportDto
                {
                    EventId = ev.EventId,
                    Title = ev.Title,
                    Date = ev.Date,
                    Location = ev.Location,
                    AttendeeCount = ev.Attendees.Count,


                });
            }

            return result;
        }



    }
}
