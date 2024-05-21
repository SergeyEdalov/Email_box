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
        private readonly IRabbitMqService<string, Guid> _mqService;

        public MessageService() { }

        public MessageService(IMapper mapper, MessageContext messageContext, IRabbitMqService<string, Guid> mqService)
        {
            _mapper = mapper;
            _messageContext = messageContext;
            _mqService = mqService;
        }
        public async Task<IEnumerable<string>> GetMessageAsync()
        {
            if (!_mqService.TryGetLatest(out var result)) throw new Exception("No message available.");
            var userId = result.userId;

            var messageList = await _messageContext.Messages
                .Where(x => x.TargetUserId == userId && x.IsDelivery == false)
                .ToListAsync();

            if (messageList.Count == 0) throw new Exception("There is no new message");

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

        public async Task SendMessageAsync(string message, Guid targetUserId)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException("Message is empty");
            if (!_mqService.TryGetLatest(out var result)) throw new Exception("No message available.");
            var userId = result.userId;

            var messageDto = new MessageDto()
            {
                Id = new Guid(),
                Body = message,
                FromUserId = userId,
                TargetUserId = targetUserId,
                IsDelivery = false
            };
            _messageContext.Add(_mapper.Map<MessageEntity>(messageDto));
            await _messageContext.SaveChangesAsync();
        }
    }
}
