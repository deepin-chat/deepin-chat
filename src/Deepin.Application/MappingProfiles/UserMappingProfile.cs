using AutoMapper;
using Deepin.Application.Models.Users;
using Deepin.Application.TypeConverters;
using Deepin.Domain.Entities;
using System.Security.Claims;

namespace Deepin.Application.MappingProfiles;
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserModel>(MemberList.Destination)
            .ForMember(d => d.Profile, o => o.Ignore());

        CreateMap<UserProfileRequest, IEnumerable<Claim>>().ConvertUsing<UserTypeConvertes>();
        CreateMap<IEnumerable<Claim>, UserProfile>().ConvertUsing<UserTypeConvertes>();
    }
}
