using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using System.ComponentModel;

namespace User.RabbitMq
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
            // Не забудьте вынести значения "localhost" и "MyQueue"
            // в файл конфигурации
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                //Port = 15672,
                //UserName = "rabbitMqUserName",
                //Password = "rabbitMqPassword",
                //VirtualHost = "rabbitMqVirtualHost",
                //Ssl =
                //{
                //    ServerName = "rabbitMqHostName",
                //}
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "MyQueue",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: string.Empty,
                               routingKey: "MyQueue",
                               basicProperties: null,
                               body: body);
            }
        }
    }
}
