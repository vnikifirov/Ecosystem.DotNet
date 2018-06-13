using AutoMapper;
using ConfAutoMapper.Controllers.Resources;
using ConfAutoMapper.Models;

namespace ConfAutoMapper.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Domain to API, and vice versa
            CreateMap<User, UserDTO>()
                .ForMember(um => um.Location , opt => opt.MapFrom(u => u.Location))
                .ReverseMap();

        }
    }
}