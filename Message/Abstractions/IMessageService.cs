namespace Message.Abstractions
{
    public interface IMessageService
    {
        public Task<IEnumerable<string>> GetMessageAsync(Guid fromUserId);
        public Task SendMessageAsync(string message, Guid fromUserId, Guid targetUserId);
    }
}
