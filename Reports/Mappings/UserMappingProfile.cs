using AutoMapper;
using BCrypt.Net;
using Reports.Entities;
using Reports.Models;

namespace Reports.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<RegistrationRequest, User>()
                .ForMember(user => user.HashedPassword, 
                    regRequest => regRequest.MapFrom(regRequest => BCrypt.Net.BCrypt.HashPassword(regRequest.Password)));
        }
    }
}
