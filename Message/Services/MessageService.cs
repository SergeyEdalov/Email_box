using AutoMapper;
using Message.Abstractions;
using Message.Database.Context;
using Message.Database.DTO;
using Message.Database.Entity;

namespace Message.Services
{
    public class MessageService : IMessageService
    {

        private readonly IMapper _mapper;
        private readonly MessageContext _messageContext;

        public MessageService() { }

        public MessageService(IMapper mapper, MessageContext messageContext)
        {
            _mapper = mapper;
            _messageContext = messageContext;
        }
        public IEnumerable<string> GetMessage()
        {
            using (_messageContext)
            {
                var messageDtoList = _messageContext.Messages
                    .Where(x => x.IsDelivery == false)
                    .Select(x => _mapper.Map<MessageDto>(x))
                    .ToList();

                if (messageDtoList.Count == 0) { throw new Exception("There is no new message"); }

                var listMessage = new List<string>();

                foreach (var item in messageDtoList)
                {
                    listMessage.Add(item.Body);
                    item.IsDelivery = true;
                }
                return listMessage;
            }
        }

        public void SendMessage(string message, Guid fromUserId, Guid targetUserId)
        {
            using (_messageContext)
            {
                var messageDto = new MessageDto()
                {
                    Id = new Guid(),
                    Body = message,
                    FromUserId = fromUserId,
                    TargetUserId = new Guid(),
                    IsDelivery = false
                };
                _messageContext.Add (_mapper.Map<MessageEntity>(messageDto));
                _messageContext.SaveChanges();
            }
        }
    }
}
