using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RabbitSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitController : ControllerBase
    {
        private readonly RabbitMqPublisher _publisher = new RabbitMqPublisher();

        //private readonly IMessageChannel _messageChannel;

        //public RabbitController ( IMessageChannel messageChannel )
        //{
        //    _messageChannel=messageChannel;
        //}
        [HttpPost("send")]
        public async Task<IActionResult> SendAsync ( [FromBody] string text )
        {
           await _publisher.SendAsync(text);
            return Ok("پیام ارسال شد");
        }


        //[HttpGet("next")]
        //public async Task<IActionResult> GetNextMessage ()
        //{
        
        //    var msg = await _messageChannel.Channel.Reader.ReadAsync();
        //    return Ok(msg);
        //}

        //[HttpGet("stream")]
        //public async IAsyncEnumerable<string> StreamMessages ()
        //{
           
        //    await foreach (var msg in _messageChannel.Channel.Reader.ReadAllAsync())
        //    {
        //        yield return msg;
        //    }
        //}
    }
}
