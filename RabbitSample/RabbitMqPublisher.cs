using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
namespace RabbitSample
{
    public class RabbitMqPublisher
    {
        public async Task SendAsync(object obj)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("test-queue", false, false, false, null);

            string json = JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "test-queue",
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: body
            );
        }
    }
}
