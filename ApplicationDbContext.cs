using EventManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace EventManagementAPI
{
    public class ApplicationDbContext : DbContext
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for Models
        public DbSet<Event> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Make the Email field unique
            modelBuilder.Entity<Attendee>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // RegisterAt default value of current date/time
            modelBuilder.Entity<Attendee>()
                .Property(a => a.RegisteredAt)
                .HasDefaultValueSql("GETUTCDATE()"); // Get defult UTC date/time from SQL Server


            // Adding Seed Data 

            var e1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var e2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var a1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            var a2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

            // SeedData for two events
            var e1 = new Event
            {
                EventId = e1Id,
                Title = "Tech Meetup Muscat",
                Description = "Monthly developer meetup",
                //Date = DateTime.UtcNow,
                Location = "Muscat",
                MaxAttendees = 10
            };
            var e2 = new Event
            {
                EventId = e2Id,
                Title = "AI & Data Summit",
                Description = "Talks on AI/ML",
                //Date = DateTime.UtcNow,
                Location = "Sohar",
                MaxAttendees = 20
            };
            // Seed Data for two attendees
            var a1 = new Attendee
            {
                AttendeeId = a1Id,
                FullName = "Sara Al-Harthy",
                Email = "sara.alharthy@example.com",
                Phone = "+96890123456",
                EventId = e1Id,
                //RegisteredAt = DateTime.UtcNow


            };
            var a2 = new Attendee
            {
                AttendeeId = a2Id,
                FullName = "Mohammed Al-Busaidi",
                Email = "mohammed.busaidi@example.com",
                Phone = "+96891234567",
                EventId = e2Id,
                //RegisteredAt = DateTime.UtcNow
            };
            // Apply the seed data
            modelBuilder.Entity<Event>().HasData(e1, e2);
            modelBuilder.Entity<Attendee>().HasData(a1, a2);
        }

    }
}
