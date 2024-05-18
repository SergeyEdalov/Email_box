using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RSATools.RSAKeys;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using User.Abstractions;
using User.DataBase.Context;
using User.DataBase.DTO;
using User.Models;

namespace User.Services
{
    public class UserAuthenticationService : IUserAuthenticationService <LoginModel>
    {
        private readonly UserContext _userContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration? _configuration;
        private readonly IRabbitMqService _mqService;

        public UserAuthenticationService() { }

        public UserAuthenticationService(UserContext userContext, IMapper mapper, IConfiguration? configuration, IRabbitMqService mqService)
        {
            _userContext = userContext;
            _mapper = mapper;
            _configuration = configuration;
            _mqService = mqService;
        }
        public async Task<string> AuthenticateAsync(LoginModel loginModel)
        {
            var entity = _userContext.Users
                .FirstOrDefault(x => x.Email.Equals(loginModel.Email));
            if (entity == null) return "User not found";

            var data = Encoding.UTF8.GetBytes(loginModel.Password).Concat(entity.Salt).ToArray();

            SHA512 shaM = new SHA512Managed();

            var bpassword = shaM.ComputeHash(data);

            if (entity.Password.SequenceEqual(bpassword))
            {
                var user = _mapper.Map<UserDto>(entity);
                var token = await Task.Run(() => GeneratreToken(user));
                await Task.Run(() => _mqService.SendMessage(token));
                return token;
            }
            else return "Wrong password";
        }

        private string GeneratreToken(UserDto user)
        {
            var securityKey = new RsaSecurityKey(RsaToolsKeys.GetPrivateKey());
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature);
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
