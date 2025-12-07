using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace RabbitSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitController : ControllerBase
    {
        
        private readonly RabbitMqPublisher _publisher;
        private readonly IMessageStore _store;

        public RabbitController(RabbitMqPublisher publisher, IMessageStore store)
        {
            _publisher = publisher;
            _store = store;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendAsync([FromBody] object body)
        {
            await _publisher.SendAsync(body);
            return Ok(new { status = "sent", body });
        }

        [HttpGet("receive")]
        public IActionResult Receive()
        {
            if (_store.TryGet(out var json))
                return Ok(JsonDocument.Parse(json));

            return NotFound("هیچ پیامی دریافت نشده");
        }
    }
}
