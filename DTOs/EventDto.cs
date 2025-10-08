namespace EventManagementAPI.DTOs
{
    public class EventDto
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public int MaxAttendees { get; set; }
        public int AttendeeCount { get; set; } 
    }
}
