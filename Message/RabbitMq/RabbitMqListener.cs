using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using CheckUnputDataLibrary;
using RSATools.RSAKeys;
using Message.Abstractions;

namespace Message.RabbitMq
{
    public class RabbitMqListener : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IRabbitMqService<string, Guid> _rabbitMqService;
        private readonly ILogger<RabbitMqListener> _logger;

        public RabbitMqListener(
            IConnectionFactory connectionFactory, 
            ILogger<RabbitMqListener> logger, 
            IRabbitMqService<string, Guid> rabbitMqService)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _rabbitMqService = rabbitMqService;
            // Не забудьте вынести значения "localhost" и "MyQueue" в файл конфигурации
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                               queue: "TokenQueue",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, args) =>
            {
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                _logger.LogInformation("Received message: {content}", content);

                var tokenAndUserId = GetTokenAndUserIdFromContent(content);

                if (tokenAndUserId.Item1 != null)
                {
                    _rabbitMqService.AddDataToQueue(tokenAndUserId.Item1, tokenAndUserId.Item2);
                }
                _channel.BasicAck(args.DeliveryTag, false);
            };

            _channel.BasicConsume("TokenQueue", false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        private (string, Guid) GetTokenAndUserIdFromContent(string token)
        {
            var securityKey = new RsaSecurityKey(RsaToolsKeys.GetPublicKey());

            if (CheckerLibrary.ValidateToken(token, securityKey))
            {
                var tokenJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var claim = tokenJwt.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier));
                Guid.TryParse(claim.Value, out Guid userId);

                return (token, userId);
            }
            return (null, new Guid());
        }
    }
}
