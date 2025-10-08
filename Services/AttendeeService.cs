using AutoMapper;
using EventManagementAPI.DTOs;
using EventManagementAPI.Models;
using EventManagementAPI.Repo;

namespace EventManagementAPI.Services
{
    public class AttendeeService : IAttendeeService
    {
        private readonly IAttendeeRepo _attendeeRepo;
        private readonly IGenericRepo<Attendee> _attendeeGeneric;
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _ctx;
        public AttendeeService(IAttendeeRepo attendeeRepo, IGenericRepo<Attendee> attendeeGeneric, IMapper mapper, ApplicationDbContext ctx, IEventRepo eventRepo)
        {
            _attendeeRepo = attendeeRepo;
            _attendeeGeneric = attendeeGeneric;
            _mapper = mapper;
            _ctx = ctx;
            _eventRepo = eventRepo;
        }

        public async Task<Guid> RegisterAsync(AttendeeCreateDto dto)
        {
            // Validate event exists 
            var ev = await _eventRepo.GetEventWithAttendeesAsync(dto.EventId)
                     ?? throw new KeyNotFoundException("Event not found.");

            // check capacity
            if (ev.Attendees.Count >= ev.MaxAttendees)
                throw new InvalidOperationException("Event is full (MaxAttendees reached).");

            // Unique Email 
            var existing = await _attendeeRepo.GetAttendeeByEmailAsync(dto.Email);
            if (existing is not null && existing.EventId == dto.EventId)
                throw new InvalidOperationException("This email is already registered for the event.");

            var entity = _mapper.Map<Attendee>(dto);
            entity.AttendeeId = Guid.NewGuid();
            

            await _attendeeGeneric.AddAsync(entity);
            await _attendeeGeneric.SaveChangesAsync();

            return entity.AttendeeId;
        }

        public async Task<List<AttendeeDto>> GetByEventIdAsync(Guid eventId)
        {
            
            var ev = await _eventRepo.GetEventWithAttendeesAsync(eventId)
                     // Using global exception handler ( KeyNotFoundException) 
                     ?? throw new KeyNotFoundException("Event not found.");

            var attendees = ev.Attendees.ToList(); // -> Lazy loading
            return _mapper.Map<List<AttendeeDto>>(attendees);
        }


    }
}
