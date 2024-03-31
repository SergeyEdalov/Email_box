namespace Message.Abstractions
{
    public interface IMessageService
    {
        public string GetMessage();
        public void SentMessage(string message);
    }
}
