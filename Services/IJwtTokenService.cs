using System.Security.Claims;

namespace EventManagementAPI.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(IEnumerable<Claim> claims, DateTime? nowUtc = null);
        DateTime GetExpiryUtc(DateTime? nowUtc = null);
    }
}