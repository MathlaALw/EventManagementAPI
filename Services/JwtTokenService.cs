using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventManagementAPI.Services
{
    public class JwtTokenService :  IJwtTokenService
    {
        private readonly IConfiguration _config;
        public JwtTokenService(IConfiguration config) => _config = config;

        public string GenerateToken(IEnumerable<Claim> claims, DateTime? nowUtc = null)
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = _config["Jwt:Key"]!;
            var minutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 120;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var now = nowUtc ?? DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public DateTime GetExpiryUtc(DateTime? nowUtc = null)
        {
            var minutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 120;
            return (nowUtc ?? DateTime.UtcNow).AddMinutes(minutes);
        }
    }
}
