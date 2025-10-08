using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementAPI.Models
{
    public class Event
    {
        [Key]
        public Guid EventId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; } = null!;

        [Range(10, 500)]
        public int MaxAttendees { get; set; }

        // Navigation property
        public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}
