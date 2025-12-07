using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSample
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly IMessageStore _store;

        public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IMessageStore store)
        {
            _logger = logger;
            _store = store;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("test-queue", false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

    
                _store.Add(json);

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync("test-queue", autoAck: true, consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
