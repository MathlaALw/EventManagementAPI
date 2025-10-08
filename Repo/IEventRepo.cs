using EventManagementAPI.Models;

namespace EventManagementAPI.Repo
{
    public interface IEventRepo
    {
        Task<List<Event>> GetAllEventsWithAttendeesAsync();
        Task<Event?> GetEventWithAttendeesAsync(Guid eventId);

    }
}