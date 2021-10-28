using AutoMapper;
using WebIntelligence.Common.Models;

namespace WebIntelligence.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserReminder, ReminderDto>();
    }
}