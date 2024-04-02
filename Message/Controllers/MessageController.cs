using Message.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Message.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        [Route("GetMessage")]
        [Authorize]
        public IActionResult GetMessage()
        {
            try
            {
                var message = _messageService.GetMessage();
                return Ok(message);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost]
        [Route("SendMessage")]
        [Authorize]
        public IActionResult SendMessage([FromQuery] string message, Guid fromUserId, Guid targetUserId)
        {
            try
            {
                _messageService.SendMessage(message, fromUserId, targetUserId);
                return Ok("Message sent");
            }
            catch (Exception ex) { return StatusCode(500); }
        }
    }
}
