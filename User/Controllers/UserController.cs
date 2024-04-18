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
        public async Task<IActionResult> AddAdmin([FromBody] UserModel user)
        {
            try
            {
                await _userService.AddAdminAsync(user);
                return Ok();
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPost]
        [Route("AddUser")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] UserModel user)
        {
            try
            {
                await _userService.AddUser(user);
                return Ok();
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("GetListUsers")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetListUsers()
        {
            try
            {
                var users = await _userService.GetListUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteUser")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromQuery] UserModel user)
        {
            try
            {
                await Task.Run(() => _userService.DeleteUser(user.UserName));
                return Ok("User deleted");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("GetIdUser")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetIdUser(string token)
        {
            try
            {
                Guid userID = await _userService.GetIdIserFromToken(token);
                return Ok(userID);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
