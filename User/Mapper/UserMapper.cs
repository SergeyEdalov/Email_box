using AutoMapper;
using System.Text;
using User.DataBase.DTO;
using User.DataBase.Entity;
using User.Models;

namespace User.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserEntity, UserDto>(MemberList.Destination).ReverseMap();
            CreateMap<RoleEntity, RoleDto>(MemberList.Destination).ReverseMap();
            
            CreateMap<UserModel, UserDto>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.Email, opts => opts.MapFrom(y => y.UserName))
                .ForMember(dest => dest.Password, opts => opts.MapFrom(y => Encoding.UTF8.GetBytes(y.Password)))
                .ForMember(dest => dest.Salt, opts => opts.Ignore())
                .ForMember(dest => dest.RoleId, opts => opts.MapFrom(y => y.Role))
                .ReverseMap();
        }
    }
}
