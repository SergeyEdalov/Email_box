using AutoMapper;
using User.DataBase.DTO;
using User.DataBase.Entity;

namespace User.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserEntity, UserDto>(MemberList.Destination).ReverseMap();
        }
    }
}
