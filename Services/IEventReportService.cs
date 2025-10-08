using EventManagementAPI.DTOs;

namespace EventManagementAPI.Services
{
    public interface IEventReportService
    {
        Task<List<UpcomingEventReportDto>> GetUpcomingWithWeatherAsync(int days = 30);
    }
}