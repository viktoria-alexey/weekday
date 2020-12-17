using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Weekday.Data.Models;
using Weekday.DataContracts;

namespace Weekday
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDataContract>()
                   .ForMember(d => d.RoleIds, map => map.MapFrom(x => x.Roles.Select(x => x.RoleId)));
            CreateMap<UserDataContract, ApplicationUser>()
                    .ForMember(d => d.Roles, map => map.Ignore())
                    .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));


            CreateMap<IdentityRole, RoleDataContract>();
        }
    }
}
