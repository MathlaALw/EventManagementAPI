using EventManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repo
{
    public class AttendeeRepo : GenericRepo<Attendee>, IAttendeeRepo
    {
        // Inject db context
        private readonly ApplicationDbContext _context;
        public AttendeeRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Attendee?> GetAttendeeByEmailAsync(string email)
        {
            return await _context.Attendees
                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
        }

        public async Task<List<Attendee>> GetAttendeesByEventIdAsync(Guid eventId)
        {
            return await _context.Attendees
                .Where(a => a.EventId == eventId)
                .AsNoTracking()
                .ToListAsync();
        }




    }
}
