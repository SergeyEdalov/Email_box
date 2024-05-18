using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Message.RabbitMq;
using Newtonsoft.Json;

namespace Message.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMqController : ControllerBase
    {
        private readonly RabbitMqListener _mqListener;

        public RabbitMqController(RabbitMqListener mqListener)
        {
            _mqListener = mqListener;
        }

        [HttpGet]
        [Route("GetTokenAndGuidFromRabbit")]
        public IActionResult GetTokenAndGuidFromRabbit()
        {
            if (_mqListener.TryGetLatest(out var result))
            {
                var res = JsonConvert.SerializeObject(result);
                return Ok(res);
            }

            return NoContent();
        }
    }
}
