using EventManagementAPI.Models;

namespace EventManagementAPI.Repo
{
    public interface IAttendeeRepo
    {
        Task<Attendee?> GetAttendeeByEmailAsync(string email);
        Task<List<Attendee>> GetAttendeesByEventIdAsync(Guid eventId);
    }
}