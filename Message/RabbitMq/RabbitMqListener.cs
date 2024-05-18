using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;
using Message.RSAKeys;
using CheckUnputDataLibrary;

namespace Message.RabbitMq
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ConcurrentQueue<(string token, Guid userId)> _queue = new ConcurrentQueue<(string token, Guid userId)>();

        public RabbitMqListener(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            // Не забудьте вынести значения "localhost" и "MyQueue"
            // в файл конфигурации
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                               queue: "MyQueue",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, args) =>
            {
                var content = Encoding.UTF8.GetString(args.Body.ToArray());

                var token = GetTokenFromContent(content);
                var userId = GetuserIdFromToken(token);

                _queue.Enqueue((content, userId));

                _channel.BasicAck(args.DeliveryTag, false);
            };

            _channel.BasicConsume("MyQueue", false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        public bool TryGetLatest(out (string token, Guid userId) result)
        {
            return _queue.TryDequeue(out result);
        }

        public JwtSecurityToken GetTokenFromContent (string token)
        {
            var securityKey = new RsaSecurityKey(RSATools.GetPublicKey());
            bool s = CheckerLibrary.ValidateToken(token, securityKey);
            if (s)
            {
                var tokenJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return tokenJwt;
            }
            return null;
        }

        public Guid GetuserIdFromToken(JwtSecurityToken token)
        {
            var claim = token.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier));

            Guid.TryParse(claim.Value, out Guid userId);
            return userId;
        }
    }
}
