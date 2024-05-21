using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Message.Abstractions;

namespace Message.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMqController : ControllerBase
    {
        private readonly IRabbitMqService<string, Guid> _mqService;

        public RabbitMqController(IRabbitMqService<string, Guid> mqService)
        {
            _mqService = mqService;
        }

        [HttpGet]
        [Route("GetTokenAndGuidFromRabbit")]
        public IActionResult GetTokenAndGuidFromRabbit()
        {
            if (_mqService.TryGetLatestForRabbitController(out var result))
            {
                var res = JsonConvert.SerializeObject(result);
                _mqService.AddDataToQueue(result.token, result.userId);
                return Ok(res);
            }

            return NoContent();
        }
    }
}
