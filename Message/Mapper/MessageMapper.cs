using AutoMapper;
using Message.Database.DTO;
using Message.Database.Entity;

namespace Message.Mapper
{
    public class MessageMapper : Profile
    {
        public MessageMapper()
        {
            CreateMap<MessageEntity, MessageDto>(MemberList.Destination).ReverseMap();
        }
    }
}
