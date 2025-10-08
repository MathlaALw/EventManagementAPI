using EventManagementAPI.DTOs;
using EventManagementAPI.Repo;
using EventManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
   
    public class ReportsController : ControllerBase
    {
        private readonly IEventReportService _reportService;

        public ReportsController(IEventReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("upcoming-events")]
        [AllowAnonymous]
        //Swagger documentation
        [SwaggerOperation(Summary = "Upcoming events report")]
        [ProducesResponseType(typeof(IEnumerable<UpcomingEventReportDto>), 200)]
        public async Task<IActionResult> Upcoming([FromQuery] int days = 30)
        {
            var data = await _reportService.GetUpcomingWithWeatherAsync(days);
            return Ok(data);
        }
    }
}
