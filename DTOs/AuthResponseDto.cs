namespace EventManagementAPI.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
    }
}
