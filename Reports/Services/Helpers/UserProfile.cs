using AutoMapper;
using Reports.Entities;
using Reports.Models;

namespace Reports.Services.Helpers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserModel, User>()
                .ForMember(dst => dst.Login, opt => opt.MapFrom(src => src.Login))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Surname, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                ;

            CreateMap<User, AuthenticateResponse>()
                .ForMember(dst => dst.Login, opt => opt.MapFrom(src => src.Login))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Surname, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dst => dst.Token, opt => opt.Ignore())
                ;
        }
    }
}
