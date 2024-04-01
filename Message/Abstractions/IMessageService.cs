namespace Message.Abstractions
{
    public interface IMessageService
    {
        public IEnumerable<string> GetMessage();
        public void SendMessage(string message, Guid fromUserId, Guid targetUserId);
    }
}
