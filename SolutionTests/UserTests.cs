using AutoMapper;
using Moq;
using SolutionTests.TestDbContext;
using System.Text;
using User.DataBase.DTO;
using User.DataBase.Entity;
using User.Models;
using User.Services;

namespace SolutionTests
{
    [TestFixture]
    class UserTests : TestCommandBase
    {
        Mock<IMapper> _userMockMapper;

        [SetUp]
        public void SetUp()
        {
            _userContext = getUContext();
            _userMockMapper = new Mock<IMapper>();

            _userMockMapper.Setup(x => x.Map<UserEntity>(It.IsAny<UserDto>()))
                .Returns((UserDto src) =>
                new UserEntity()
                {
                    Id = src.Id,
                    Email = src.Email,
                    Password = src.Password,
                    Salt = src.Salt,
                    RoleId = src.RoleId
                });
            _userMockMapper.Setup(x => x.Map<UserDto>(It.IsAny<UserEntity>()))
                .Returns((UserEntity src) =>
                new UserDto()
                {
                    Id = src.Id,
                    Email = src.Email,
                    Password = src.Password,
                    Salt = src.Salt,
                    RoleId = src.RoleId
                });
            _userMockMapper.Setup(x => x.Map<UserDto>(It.IsAny<UserModel>()))
                .Returns((UserModel src) => 
                new UserDto()
                {
                    Email = src.UserName,
                    Password = Encoding.UTF8.GetBytes(src.Password),
                    RoleId = (Role)src.Role
                });
        }

        [Test]
        public async Task TestAddAdmin()
        {
            // arrage
            var userModel = new UserModel()
            {
                UserName = "admin@mail.ru",
                Password = "Qwerty1)",
                Role = UserRole.Admin
            };
            var userService = new UserService(_userMockMapper.Object, _userContext);

            //act
            await userService.AddAdminAsync(userModel);

            //assert
            Assert.IsTrue(_userContext.SaveChangesAsync().Result == 0);

            Destroy(_userContext);
        }

        [Test]
        public async Task TestAddUser()
        {
            // arrage
            var userModel = new UserModel()
            {
                UserName = "bob@mail.ru",
                Password = "Hh45!4",
                Role = UserRole.User
            };
            var userService = new UserService(_userMockMapper.Object, _userContext);

            //act
            await userService.AddUserAsync(userModel);

            //assert
            Assert.IsTrue(_userContext.SaveChangesAsync().Result == 0);

            Destroy(_userContext);
        }


        [Test]
        public async Task TestGetListUser()
        {
            // arrage
            var userService = new UserService(_userMockMapper.Object, _userContext);
            var expectedListUsers = _userContext.Users.Count();

            //act
            var actualListUsers = await userService.GetListUsersAsync();

            //assert
            Assert.IsNotNull(actualListUsers);
            Assert.That(actualListUsers.Count, Is.EqualTo(expectedListUsers));

            Destroy(_userContext);
        }

        [Test]
        public async Task TestDeleteUser()
        {
            // arrage
            var userService = new UserService(_userMockMapper.Object, _userContext);
            var name = "bob@mail.ru";

            //act
            await userService.DeleteUserAsync(name);

            //assert
            Assert.IsTrue(_userContext.SaveChangesAsync().Result == 0);

            Destroy(_userContext);
        }

        [Test]
        public async Task TestGetIdUser()
        {
            // arrage
            var userService = new UserService(_userMockMapper.Object, _userContext);
            var expectedGuid = new Guid ("c5d627e4-e124-4811-a3ee-730a940b074f" );
            var token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImM1ZDYyN2U0LWUxMjQtNDgxMS1hM2VlLTczMGE5NDBiMDc0ZiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MTQyMjA2OTIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyMDUiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MjA1In0.Qe4IgTTnP9GTQmPeIwABBdyp34zQqbzRo_brOymaeqRGKrS8E4UnimDJ5stVJSO1740oQwRLzk2OsIRCIN1TFLLKiRUDfvzcyGE3m5ifviunFwcXomIW0wypmQpepAsIQs15bPFEaLBxHEDIyAsaXNAjP-1BFVByphg7x5H0N_s_kwQds9jPVExuWlZdpjitP1Klajtm1Y-ocwlKPTAeuu6RhqgKHZUZuuBWHBPhQisri3a17dETl8E3y7arhCCdMRoYKA2MIANw42ZquPGurJx6wNfuk3n9Ajik1a8gHxz4M5NSpL96dqYQb-D-7IIEdE4yyAihxMRa6ddyTrL_BQ";

            //act
            var guidUser = await userService.GetIdUserFromTokenAsync(token);

            //assert
            Assert.That(guidUser, Is.EqualTo(expectedGuid));

            Destroy(_userContext);
        }
    }
}
