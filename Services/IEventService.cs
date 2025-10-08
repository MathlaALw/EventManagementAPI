using EventManagementAPI.DTOs;

namespace EventManagementAPI.Services
{
    public interface IEventService
    {
        Task<Guid> CreateAsync(EventCreateDto dto);
        //Task<List<EventDto>> GetAllAsync();
        Task<EventDto?> GetByIdAsync(Guid id);
        Task<List<EventDto>> GetAllAsync(string? location = null, DateTime? from = null, DateTime? to = null, string? sort = null);
    }
}