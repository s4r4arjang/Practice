using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client;
namespace RabbitSample
{
    public class RabbitMqPublisher
    {
        public RabbitMqPublisher()
        {
            
        }
        public async Task SendAsync ( string message )
        {
            var factory = new ConnectionFactory()
            {
                HostName="localhost",
                UserName="guest",
                Password="guest"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync(); 

            await channel.QueueDeclareAsync(
                queue: "test-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            var props = new BasicProperties();

            await channel.BasicPublishAsync<BasicProperties>(
                exchange: "",
                routingKey: "test-queue",
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: CancellationToken.None);

            Console.WriteLine($"پیام ارسال شد: {message}");
        }
    }
}
