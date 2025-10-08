using EventManagementAPI.DTOs;

namespace EventManagementAPI.Services
{
    public interface IAttendeeService
    {
        Task<List<AttendeeDto>> GetByEventIdAsync(Guid eventId);
        Task<Guid> RegisterAsync(AttendeeCreateDto dto);
    }
}