namespace Message.Abstractions
{
    public interface IMessageService
    {
        public Task<IEnumerable<string>> GetMessageAsync();
        public Task SendMessageAsync(string message, Guid targetUserId);
    }
}
