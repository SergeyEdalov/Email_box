namespace Message.Database.Entity
{
    public class MessageEntity
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public Guid FromUserId { get; set; }
        public Guid TargetUserId { get; set; }
        public bool IsDelivery { get; set; } = false;
    }
}
