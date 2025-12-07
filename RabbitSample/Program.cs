
namespace RabbitSample;

public class Program
{
    public static void Main ( string[] args )
    {
        var builder = WebApplication.CreateBuilder(args);
        

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<RabbitMqPublisher>();
        builder.Services.AddHostedService<RabbitMqConsumer>();
        builder.Services.AddSingleton<IMessageStore, MessageStore>();
        var app = builder.Build();

       
            app.UseSwagger();
            app.UseSwaggerUI();
     

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
