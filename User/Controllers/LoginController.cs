using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Abstractions;
using User.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace User.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserAuthenticationService<LoginModel> _userAuthenticationService; 

        public LoginController(IUserAuthenticationService<LoginModel> userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            var token = await _userAuthenticationService.AuthenticateAsync(loginModel);

            if (token != null)
            {
                return Ok(token);
            }
            return NotFound("Error Authentication. Check that the entered data is correct.");
        }
    }
}
