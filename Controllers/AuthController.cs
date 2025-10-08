using EventManagementAPI.DTOs;
using EventManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwt;

        public AuthController(IJwtTokenService jwt)
        {
            _jwt = jwt;
        }

        // Demo login: replace with real user validation (DB/Identity) later
        [HttpPost("login")]
        [AllowAnonymous]
       
        public IActionResult Login([FromBody] LoginRequestDto req)
        {
            // Demo validation 
            if (string.Equals(req.Username, "a", StringComparison.OrdinalIgnoreCase) &&
                req.Password == "1")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, req.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var token = _jwt.GenerateToken(claims);
                return Ok(new AuthResponseDto
                {
                    Token = token,
                    ExpiresAtUtc = _jwt.GetExpiryUtc()
                });
            }

            return Unauthorized(new { error = "Invalid username or password." });
        }
    }
}
