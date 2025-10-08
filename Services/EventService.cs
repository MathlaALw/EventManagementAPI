using AutoMapper;
using EventManagementAPI.DTOs;
using EventManagementAPI.Models;
using EventManagementAPI.Repo;

namespace EventManagementAPI.Services
{
    public class EventService : IEventService
    {

        private readonly IEventRepo _eventRepo;
        private readonly IGenericRepo<Event> _genericRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _ctx;

        public EventService(IEventRepo eventRepo, IGenericRepo<Event> genericRepo, IMapper mapper, ApplicationDbContext ctx)
        {
            _eventRepo = eventRepo;
            _genericRepo = genericRepo;
            _mapper = mapper;
            _ctx = ctx;
        }

        // Filtering & sorting with LINQ
        public async Task<List<EventDto>> GetAllAsync(string? location = null, DateTime? from = null, DateTime? to = null, string? sort = null)
        {
            var events = await _eventRepo.GetAllEventsWithAttendeesAsync();

            if (!string.IsNullOrWhiteSpace(location))
                events = events.Where(e => e.Location.ToLower().Contains(location.ToLower())).ToList();

            if (from.HasValue) events = events.Where(e => e.Date >= from.Value).ToList();
            if (to.HasValue) events = events.Where(e => e.Date <= to.Value).ToList();

            events = sort?.ToLower() switch
            {
                "date_desc" => events.OrderByDescending(e => e.Date).ToList(),
                "title" => events.OrderBy(e => e.Title).ToList(),
                "attendees" => events.OrderByDescending(e => e.Attendees.Count).ToList(),
                _ => events.OrderBy(e => e.Date).ToList()
            };

            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<EventDto?> GetByIdAsync(Guid id)
        {
            var ev = await _eventRepo.GetEventWithAttendeesAsync(id); // eager load attendees
            return _mapper.Map<EventDto?>(ev);
        }

        public async Task<Guid> CreateAsync(EventCreateDto dto)
        {
            if (dto.Date.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("Cannot create an event in the past.");

            var entity = _mapper.Map<Event>(dto);
            entity.EventId = Guid.NewGuid();

            await _genericRepo.AddAsync(entity); 
            await _genericRepo.SaveChangesAsync();

            return entity.EventId;
        }
    }
}
