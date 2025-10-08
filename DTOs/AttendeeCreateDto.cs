namespace EventManagementAPI.DTOs
{
    public class AttendeeCreateDto
    {
        public Guid EventId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

    }
}
