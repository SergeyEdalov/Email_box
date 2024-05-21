using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using User.Abstractions;

namespace User.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "TokenQueue",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: string.Empty,
                               routingKey: "TokenQueue",
                               basicProperties: null,
                               body: body);
            }
        }
    }
}
