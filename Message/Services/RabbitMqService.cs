using Message.Abstractions;
using System.Collections.Concurrent;

namespace Message.Services
{
    public class RabbitMqService : IRabbitMqService<string, Guid>
    {
        private ConcurrentQueue<(string token, Guid userId)> _queue = new ConcurrentQueue<(string token, Guid userId)>();

        public RabbitMqService() { }

        public virtual bool TryGetLatest(out (string token, Guid userId) result) => _queue.TryPeek(out result);

        public void AddDataToQueue(string token, Guid userId) => _queue.Enqueue((token, userId));
        public bool TryGetLatestForRabbitController(out (string token, Guid userId) result) => _queue.TryDequeue(out result);
    }
}
