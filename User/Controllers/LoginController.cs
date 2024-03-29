﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Abstractions;
using User.Models;
using User.Services;

namespace User.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserAuthenticationService _userAuthenticationService;

        public LoginController(IUserAuthenticationService userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] LoginModel loginModel)
        {
            //var token = _userAuthenticationService.Authenticate(loginModel);
            var token = _userAuthenticationService.AuthenticateMock(loginModel); //Заглушка

            if (token != null)
            {
                return Ok(token);
            }
            return NotFound("Error Authentication");  
        }
    }
}
