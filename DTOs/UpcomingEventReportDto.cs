namespace EventManagementAPI.DTOs
{
    public class UpcomingEventReportDto
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public int MaxAttendees { get; set; }
        public int AttendeeCount { get; set; }
    
        public string WeatherSnippet { get; set; } = null!;
    }
}
