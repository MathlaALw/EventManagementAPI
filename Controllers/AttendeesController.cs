using EventManagementAPI.DTOs;
using EventManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeesController : ControllerBase
    {
        private readonly IAttendeeService _service;
        private readonly ILogger<AttendeesController> _logger;


        public AttendeesController(IAttendeeService service, ILogger<AttendeesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET attendees by event id 
        [HttpGet("Get attendees by event id")]

        // Swagger documentation
        [SwaggerOperation(Summary = "Get attendees by event id")]
        [ProducesResponseType(typeof(IEnumerable<AttendeeDto>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetEventAttendees([FromQuery] Guid eventId)
        {
            try
            {
                // Logger 
                _logger.LogInformation("Fetching attendees for event {EventId}", DateTime.UtcNow);
                var attendees = _service.GetByEventIdAsync(eventId).Result;
                return new OkObjectResult(attendees); 
            }
           
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }


        // Register an attendee for an event
        [HttpPost("Register an attendee for an event")]
        [Authorize] // Requires authentication -> Demo Username = "a" Password = "1"

        // Swagger documentation
        [SwaggerOperation(Summary = "Register attendee", Description = "Prevents duplicates and overbooking.")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]

        public IActionResult Register([FromBody] DTOs.AttendeeCreateDto dto)
        {
            try
            {
                var attendee = _service.RegisterAsync(dto).Result;
                return new OkObjectResult(attendee);
            }
            catch (KeyNotFoundException ex)
            {
                return new NotFoundObjectResult(new { error = ex.Message });
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
