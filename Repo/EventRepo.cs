using EventManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repo
{
    public class EventRepo : GenericRepo<Event>, IEventRepo
    {
        // Inject db context
        private readonly ApplicationDbContext _context;
        public EventRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        // Eager load all events with attendees
        public async Task<List<Event>> GetAllEventsWithAttendeesAsync()
        {
            return await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .ToListAsync();
        }

        // Eager load a single event with attendees by EventId
        public async Task<Event?> GetEventWithAttendeesAsync(Guid eventId)
        {
            return await _context.Events
                .Include(e => e.Attendees)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == eventId);
        }



    }
}
