using AutoMapper;
using EventManagementAPI.DTOs;
using EventManagementAPI.Models;
namespace EventManagementAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {

            // Event -> EventDto (AttendeeCount = attendees.Count)
            CreateMap<Event, EventDto>()
                .ForMember(d => d.AttendeeCount, o => o.MapFrom(s => s.Attendees.Count));

            // Create DTOs -> Entities
            CreateMap<EventCreateDto, Event>();
            CreateMap<AttendeeCreateDto, Attendee>()
                .ForMember(a => a.RegisteredAt, o => o.MapFrom(_ => DateTime.UtcNow));

            // Entities -> Read DTOs
            CreateMap<Attendee, AttendeeDto>();



        }
    }
}
