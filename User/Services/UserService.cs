using AutoMapper;
using CheckUnputDataLibrary;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using User.Abstractions;
using User.DataBase.Context;
using User.DataBase.DTO;
using User.DataBase.Entity;
using User.Models;

namespace User.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserContext _userContext;

        public UserService() { }

        public UserService(IMapper mapper, UserContext userContext)
        {
            _mapper = mapper;
            _userContext = userContext;
        }
        public async Task AddAdminAsync(UserModel userModel)
        {
            var count = _userContext.Users.Count();

            if (count != 0) { throw new Exception("There is already have users in system"); }

            userModel.Role = UserRole.Admin;

            var userDb = await Task.Run(() => _mapper.Map<UserEntity>(CreateUser(userModel)));

            await _userContext.AddAsync(userDb);
            await _userContext.SaveChangesAsync();
        }

        public async Task AddUserAsync(UserModel userModel)
        {
            if (userModel.Role == 0)
            {
                var count = _userContext.Users.Count(x => x.RoleId == 0);
                if (count > 0) { throw new Exception("Second Admin!"); }
            }

            var matchEmail = _userContext.Users.Where(x => x.Email.Equals(userModel.UserName)).FirstOrDefault();

            if (matchEmail?.Email != null)
            {
                throw new Exception("Email is already exsits");
            }
            var userDb = await Task.Run(() => _mapper.Map<UserEntity>(CreateUser(userModel)));

            await _userContext.AddAsync(userDb);
            await _userContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetListUsersAsync()
        {
            try
            {
                var listUsers = await _userContext.Users.Select(x => _mapper.Map<UserDto>(x)).ToListAsync();
                return listUsers;
            }
            catch (Exception) { throw new Exception("Server error"); }
        }

        public async Task DeleteUserAsync(string userName)
        {
            var deleteUser = _userContext.Users.Where(x => x.Email.Equals(userName)).FirstOrDefault();

            if (deleteUser != null)
            {
                if (deleteUser.RoleId == 0)
                {
                    throw new Exception("You can`t delete yourself");
                }
                _userContext.Users.Remove(deleteUser);
                await _userContext.SaveChangesAsync();
            }
            else { throw new Exception("User not found"); }
        }

        public async Task<Guid> GetIdUserFromTokenAsync(string token)
        {
            var tokenJwt = new JwtSecurityToken();
            await Task.Run(() =>
            {
                try { tokenJwt = new JwtSecurityTokenHandler().ReadJwtToken(token); }
                catch (Exception) { throw new Exception("Token is not valid"); }
            });

            var claim = tokenJwt.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier));

            if (claim != null)
            {
                if (Guid.TryParse(claim.Value, out Guid userId))
                {
                    return userId;
                }
            }
            else throw new ArgumentNullException("Token is not contain user id");
            return Guid.Empty;
        }

        private UserDto CreateUser(UserModel userModel)
        {
            if (!Class1.CheckEmail(userModel.UserName))
            {
                throw new IOException("Wrong format email");
            }
            else if (!Class1.CheckLengthPassword(userModel.Password))
            {
                throw new IOException("Length password must be more than six characters");
            }
            else if (!Class1.CheckDifficultPassword(userModel.Password))
            {
                throw new IOException("The password must contain upper and lower case letters, numbers and special characters");
            }
            else
            {
                var userDto = _mapper.Map<UserDto>(userModel);
                userDto.Id = new Guid();
                userDto.Salt = new byte[16];

                new Random().NextBytes(userDto.Salt);

                var data = userDto.Password.Concat(userDto.Salt).ToArray();

                SHA512 shaM = new SHA512Managed();

                userDto.Password = shaM.ComputeHash(data);
                return userDto;
            }
        }
    }
}
