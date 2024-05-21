using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Abstractions;
using User.DataBase.DTO;
using User.Models;

namespace User.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService<UserModel, UserDto> _userService;

        public UserController(IUserService<UserModel, UserDto> userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("AddAdmin")]
        public async Task<IActionResult> AddAdminAsync([FromBody] UserModel user)
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserAsync([FromBody] UserModel user)
        {
            try
            {
                await _userService.AddUserAsync(user);
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
                var users = await _userService.GetListUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAsync([FromQuery] UserModel user)
        {
            try
            {
                await _userService.DeleteUserAsync(user.UserName);
                return Ok("User deleted");
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("GetIdUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetIdUser(string token)
        {
            try
            {
                Guid userID = await _userService.GetIdUserFromTokenAsync(token);
                return Ok(userID);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
