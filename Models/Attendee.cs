using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementAPI.Models
{
    public class Attendee
    {

        [Key]
        
        public Guid AttendeeId { get; set; }

        [Required]
        [MaxLength(80)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        public DateTime RegisteredAt { get; set; }

        // Foreign Key
        [ForeignKey("Event")]
        public Guid EventId { get; set; }

        // Navigation
        public virtual Event Event { get; set; } = null!;
    }
}
