using EventManagementAPI.DTOs;
using EventManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;



namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _service;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService service, ILogger<EventsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // List all events (eager loads attendees count).
        [HttpGet("List all events")]

        public async Task<ActionResult<List<EventDto>>> GetAllAsync()
        {
            try
            {
                // Logger
                _logger.LogInformation("Fetching all events", DateTime.UtcNow);
                var events = await _service.GetAllAsync();
                return new OkObjectResult(events);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
            }
           
        }


        // Create a new event
        [HttpPost("Create a new event")]
        [Authorize(Roles = "Admin")] // Demo Username = "a" Password = "1"
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] EventCreateDto dto)
        {
            try
            {
                var eventId = await _service.CreateAsync(dto);
                return new OkObjectResult(eventId);
            }
            catch (InvalidOperationException ex)
            {
                return new BadRequestObjectResult(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }




    }
}
