using AutoMapper;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using User.Abstractions;
using User.DataBase.Context;
using User.DataBase.DTO;
using User.DataBase.Entity;

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
        public Guid AddAdmin(string email, string password)
        {
            using (_userContext)
            {
                if (_userContext != null) { throw new Exception("There is already have users in system"); }

                var userDb = _mapper.Map<UserEntity>(CreateUser(email, password, Role.Admin));

                _userContext.Add(userDb);
                _userContext.SaveChanges();

                return userDb.Id;
            }
        }

        public Guid AddUser(string email, string password, Role role)
        {
            using (_userContext)
            {
                if (role == Role.Admin)
                {
                    var count = _userContext.Users.Count(x => x.RoleId == Role.Admin);
                    if (count > 0) { throw new Exception("Second Admin!"); }
                }
                if (_userContext.Users.Select(x => x.Email.Equals(email)).FirstOrDefault())
                {
                    throw new Exception("Email is already exsits");
                }
                var userDb = _mapper.Map<UserEntity>(CreateUser(email, password, role));

                _userContext.Add(userDb);
                _userContext.SaveChanges();

                return userDb.Id;
            }
        }

        public void DeleteUser(string userName)
        {
            using (_userContext)
            {
                var deleteUser = _userContext.Users.Where(x => x.Email.Equals(userName)).FirstOrDefault();

                if (deleteUser != null)
                {
                    if (deleteUser.RoleId == Role.Admin)
                    {
                        throw new Exception("You can`t delete yourself");
                    }
                    _userContext.Users.Remove(_mapper.Map<UserEntity>(deleteUser));
                    _userContext.SaveChanges();
                }
                else { throw new Exception("User not found"); }
            }
        }

        public IEnumerable<UserDto> GetListUsers()
        {
            using (_userContext)
            {
                var listUsers = _userContext.Users.Select(x => _mapper.Map<UserDto>(x)).ToList();
                return listUsers;
            }
        }

        private static UserDto CreateUser(string email, string password, Role role)
        {
            var userDto = new UserDto() { Id = new Guid(), Email = email, Salt = new byte[16], RoleId = role };

            new Random().NextBytes(userDto.Salt);

            var data = Encoding.UTF8.GetBytes(password).Concat(userDto.Salt).ToArray();

            SHA512 shaM = new SHA512Managed();

            userDto.Password = shaM.ComputeHash(data);

            return userDto;
        }
    }
}
