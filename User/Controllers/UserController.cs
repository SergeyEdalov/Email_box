using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Abstractions;
using User.Models;

namespace User.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("AddAdmin")]
        public IActionResult AddAdmin([FromBody] UserModel user)
        {
            try
            {
                var userId = _userService.AddAdmin(user);
                return Ok();
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost]
        [Route("AddUser")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddUser([FromBody] UserModel user)
        {
            try
            {
                var userId = _userService.AddUser(user);
                return Ok();
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("GetListUsers")]
        //[Authorize(Roles = "Admin, User")]
        public IActionResult GetListUsers()
        {
            var users = _userService.GetListUsers();
            return Ok(users);
        }

        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser([FromQuery] UserModel user)
        {
            try 
            { 
                _userService.DeleteUser(user.UserName);
                return Ok("User deleted");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("GetIdUser")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetIdUser(string token)
        {
            try
            {
                Guid userID = _userService.GetIdIserFromToken(token);
                return Ok(userID);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
