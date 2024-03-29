using AutoMapper;
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
            
            CreateMap<UserModel, UserDto>(MemberList.Destination).ReverseMap(); //доделать
        }
    }
}
