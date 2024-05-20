namespace Message.Abstractions
{
    public interface IRabbitMqService<T1, T2>
    {
        bool TryGetLatest(out (T1 token, T2 userId) result);

        void AddDataToQueue(T1 token, T2 userId);

        public bool TryGetLatestForRabbitController(out (T1 token, T2 userId) result);
    }
}
