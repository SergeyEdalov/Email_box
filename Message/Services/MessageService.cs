using AutoMapper;
using Message.Abstractions;
using Message.Database.Context;
using Message.Database.DTO;
using Message.Database.Entity;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IEnumerable<string>> GetMessageAsync(Guid targetUserId)
        {
            var messageList = await _messageContext.Messages
                .Where(x => x.TargetUserId == targetUserId && x.IsDelivery == false)
                .ToListAsync();

            if (messageList.Count == 0) { throw new Exception("There is no new message"); }

            var listMessage = new List<string>();

            foreach (var item in messageList)
            {
                var messageDto = _mapper.Map<MessageDto>(item);
                listMessage.Add(messageDto.Body);

                item.IsDelivery = true;
                _messageContext.Update(item);
            }
            await _messageContext.SaveChangesAsync();
            return listMessage;
        }

        public async Task SendMessageAsync(string message, Guid fromUserId, Guid targetUserId)
        {
            if (message is null) throw new ArgumentNullException("Message is empty");

            var messageDto = new MessageDto()
            {
                Id = new Guid(),
                Body = message,
                FromUserId = fromUserId,
                TargetUserId = targetUserId,
                IsDelivery = false
            };
            _messageContext.Add(_mapper.Map<MessageEntity>(messageDto));
            await _messageContext.SaveChangesAsync();
        }
    }
}
