using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSample
{
    public class RabbitMqConsumer : BackgroundService
    {
        //private readonly IMessageChannel _messageChannel;

        //public RabbitMqConsumer ( IMessageChannel messageChannel )
        //{
        //    _messageChannel=messageChannel;
        //}

        protected override async Task ExecuteAsync ( CancellationToken stoppingToken )
        {
            var factory = new ConnectionFactory
            {
                HostName="localhost",
                UserName="guest",
                Password="guest"
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "test-queue",
                durable: false,
                exclusive: false,   // ⚠️ حتما false
                autoDelete: false,
                arguments: null
            );


            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync+=async ( model, ea ) =>
            {
                var msg = Encoding.UTF8.GetString(ea.Body.ToArray());

           
              //  await _messageChannel.Channel.Writer.WriteAsync(msg);

                Console.WriteLine($"[RabbitMQ] RECEIVED: {msg}");
            };

            await channel.BasicConsumeAsync("test-queue", true, consumer);

            // نگه داشتن سرویس
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
