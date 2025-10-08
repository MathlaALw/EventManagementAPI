namespace EventManagementAPI.DTOs
{
    public class AttendeeDto
    {
        public Guid AttendeeId { get; set; }
        public Guid EventId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
