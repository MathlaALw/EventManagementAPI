using EventManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeesController : ControllerBase
    {
        private readonly IAttendeeService _service;

        public AttendeesController(IAttendeeService service)
        {
            _service = service;
        }
        
        // GET attendees by event id 
        [HttpGet("Get attendees by event id")]
        public IActionResult GetEventAttendees([FromQuery] Guid eventId)
        {
            try
            {
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
