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
        UserService _userService;
        UserModel _userModel;

        [OneTimeSetUp]
        public void Init()
        {
            _userMockMapper = new Mock<IMapper>();
            _userContext = getUContext();

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

            _userService = new UserService(_userMockMapper.Object, _userContext);
            _userModel = new UserModel();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            Destroy(_userContext);
        }

        [Test]
        public async Task AddAdmin_DefaultSuccsess()
        {
            // arrage
            _userModel.UserName = "admin@mail.ru";
            _userModel.Password = "Qwerty1)";
            _userModel.Role = UserRole.Admin;

            //act
            await _userService.AddAdminAsync(_userModel);
            var expected = _userContext.Users.FirstOrDefault();

            CleanUserContext(_userContext);

            //assert
            Assert.That(expected.Email.Equals(_userModel.UserName));
            Assert.That(((int)expected.RoleId) == (int)_userModel.Role);
        }

        [Test]
        public void AddAdmin_AwaitException_HaveAlreadyUsersInDatabase()
        {
            // arrage
            CreateUserEntityToUserContext("victor@mail.ru", "O5#5hGb", _userContext);

            //act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _userService.AddAdminAsync(_userModel));

            CleanUserContext(_userContext);

            //assert
            Assert.That(ex.Message, Is.EqualTo("There is already have users in system"));
        }

        [Test]
        public void AddAdmin_AwaitException_WrongEmail()
        {
            // arrage
            _userModel.UserName = "admin34mailkfy!!ru";
            _userModel.Password = "Qwerty1)";

            //act
            var ex = Assert.ThrowsAsync<IOException>(async () => await _userService.AddAdminAsync(_userModel));


            //assert
            Assert.That(ex.Message, Is.EqualTo("Wrong format email"));
        }

        [Test]
        public void AddAdmin_AwaitException_WrongLengthPassword()
        {
            // arrage
            _userModel.UserName = "admin@mail.ru";
            _userModel.Password = "Qwer";

            //act
            var ex = Assert.ThrowsAsync<IOException>(async () => await _userService.AddAdminAsync(_userModel));


            //assert
            Assert.That(ex.Message, Is.EqualTo("Length password must be more than six characters"));
        }

        [Test]
        public void AddAdmin_AwaitException_WrongDifficultPassword()
        {
            // arrage
            _userModel.UserName = "admin@mail.ru";
            _userModel.Password = "Qwerrrrrryui";

            //act
            var ex = Assert.ThrowsAsync<IOException>(async () => await _userService.AddAdminAsync(_userModel));


            //assert
            Assert.That(ex.Message, Is.EqualTo("The password must contain upper and lower case letters, numbers and special characters"));
        }

        [Test]
        public async Task AddUser_Default()
        {
            // arrage
            _userModel.UserName = "bob@mail.ru";
            _userModel.Password = "Hh45!4";
            _userModel.Role = UserRole.User;

            //act
            await _userService.AddUserAsync(_userModel);
            var expected = _userContext.Users.FirstOrDefault();

            CleanUserContext(_userContext);

            //assert
            Assert.That(expected.Email.Equals(_userModel.UserName));
            Assert.That(((int)expected.RoleId) == (int)_userModel.Role);
        }

        [Test]
        public void AddUser_AwaitException_SecondAdmin()
        {
            // arrage
            CreateUserEntityToUserContext("victor@mail.ru", "O5#5hGb", _userContext, Role.Admin);

            _userModel.UserName = "bob@mail.ru";
            _userModel.Password = "Hh45!4";
            _userModel.Role = UserRole.Admin;

            //act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _userService.AddUserAsync(_userModel));

            CleanUserContext(_userContext);

            //assert
            Assert.That(ex.Message, Is.EqualTo("Second Admin!"));
        }

        [Test]
        public void AddUser_AwaitException_EmailIsExcists()
        {
            // arrage
            CreateUserEntityToUserContext("bob@mail.ru", "Hh45!4", _userContext);

            _userModel.UserName = "bob@mail.ru";
            _userModel.Password = "Hh45!4";
            _userModel.Role = UserRole.User;

            //act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _userService.AddUserAsync(_userModel));

            CleanUserContext(_userContext);

            //assert
            Assert.That(ex.Message, Is.EqualTo("Email is already exsits"));
        }


        [Test]
        public async Task GetListUser_DefaultSuccsess()
        {
            // arrage
            CreateUserEntityToUserContext("bob@mail.ru", "Hh45!4", _userContext);
            CreateUserEntityToUserContext("Tedd@mail.ru", "Tyu45^4", _userContext);
            var expectedListUsers = _userContext.Users.Count();

            //act
            var actualListUsers = await _userService.GetListUsersAsync();

            CleanUserContext(_userContext);

            //assert
            Assert.IsNotNull(actualListUsers);
            Assert.That(actualListUsers.Count, Is.EqualTo(expectedListUsers));
        }

        [Test]
        public async Task DeleteUser_DefaultSuccsess()
        {
            // arrage
            CreateUserEntityToUserContext("victor@mail.ru", "O5#5hGb", _userContext);
            _userModel.UserName = "victor@mail.ru";
            var count = _userContext.Users.Count();

            //act
            await _userService.DeleteUserAsync(_userModel.UserName);

            //assert
            Assert.IsTrue(count - _userContext.Users.Count() == 1);
        }

        [Test]
        public void DeleteUser_AwaitException_UserNotFound()
        {
            // arrage
            _userModel.UserName = "victor@mail.ru";

            //act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _userService.DeleteUserAsync(_userModel.UserName));

            //assert
            Assert.That(ex.Message, Is.EqualTo("User not found"));
        }

        [Test]
        public void DeleteUser_AwaitException_TryDeleteAdmin()
        {
            // arrage
            CreateUserEntityToUserContext("admin@mail.ru", "O5#5hGb", _userContext, Role.Admin);
            _userModel.UserName = "admin@mail.ru";

            //act
            var ex = Assert.ThrowsAsync<Exception>(async () => await _userService.DeleteUserAsync(_userModel.UserName));

            //assert
            Assert.That(ex.Message, Is.EqualTo("You can`t delete yourself"));
        }

        [Test]
        public async Task GetGuidUser_DefaultSuccsess()
        {
            // arrage
            var expectedGuid = new Guid("c5d627e4-e124-4811-a3ee-730a940b074f");
            var token = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImM1ZDYyN2U0LWUxMjQtNDgxMS1hM2VlLTczMGE5NDBiMDc0ZiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MTQyMjA2OTIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyMDUiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MjA1In0.Qe4IgTTnP9GTQmPeIwABBdyp34zQqbzRo_brOymaeqRGKrS8E4UnimDJ5stVJSO1740oQwRLzk2OsIRCIN1TFLLKiRUDfvzcyGE3m5ifviunFwcXomIW0wypmQpepAsIQs15bPFEaLBxHEDIyAsaXNAjP-1BFVByphg7x5H0N_s_kwQds9jPVExuWlZdpjitP1Klajtm1Y-ocwlKPTAeuu6RhqgKHZUZuuBWHBPhQisri3a17dETl8E3y7arhCCdMRoYKA2MIANw42ZquPGurJx6wNfuk3n9Ajik1a8gHxz4M5NSpL96dqYQb-D-7IIEdE4yyAihxMRa6ddyTrL_BQ";

            //act
            var guidUser = await _userService.GetIdUserFromTokenAsync(token);

            //assert
            Assert.That(guidUser, Is.EqualTo(expectedGuid));
        }

        [Test]
        public void GetGuidUser_AwaitException_TokenNotValid()
        {
            // arrage
            var shortToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWcNoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZy9uYW1laWRlbnRpZmllciI6ImM1ZDYyN2U0LWUxMjQtNDgxMS1hM2VlLTczMGE5NDBiMDc0ZiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MTQyMjA2OTIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyMDUiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MjA1In0.Qe4IgTTnP9GTQmPeIwABBdyp34zQqbzRo_brOymaeqRGKrS8E4UnimDJ5stVJSO1740oQwRLzk2OsIRCIN1TFLLKiRUDfvzcyGE3m5ifviunFwcXomIW0wypmQpepAsIQs15bPFEaLBxHEDIyAsaXNAjP-1BFVByphg7x5H0N_s_kwQds9jPVExuWlZdpjitP1Klajtm1Y-ocwlKPTAeuu6RhqgKHZUZuuBWHBPhQisri3a17dETl8E3y7arhCCdMRoYKA2MIANw42ZquPGurJx6wNfuk3n9Ajik1a8gHxz4M5NSpL96dqYQb-D-7IIEdE4yyAihxMRa6ddyTrL_BQ";
            var changedToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yyy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImM1ZDYyN2U0LWUxMjQtNDgxMS1hM2VlLTczMGE5NDBiMDc0ZiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MTQyMjA2OTIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyMDUiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MjA1In0.Qe4IgTTnP9GTQmPeIwABBdyp34zQqbzRo_brOymaeqRGKrS8E4UnimDJ5stVJSO1740oQwRLzk2OsIRCIN1TFLLKiRUDfvzcyGE3m5ifviunFwcXomIW0wypmQpepAsIQs15bPFEaLBxHEDIyAsaXNAjP-1BFVByphg7x5H0N_s_kwQds9jPVExuWlZdpjitP1Klajtm1Y-ocwlKPTAeuu6RhqgKHZUZuuBWHBPhQisri3a17dETl8E3y7arhCCdMRoYKA2MIANw42ZquPGurJx6wNfuk3n9Ajik1a8gHxz4M5NSpL96dqYQb-D-7IIEdE4yyAihxMRa6ddyTrL_BQ";

            //act
            var ex1 = Assert.ThrowsAsync<Exception>(async () => await _userService.GetIdUserFromTokenAsync(shortToken));
            var ex2 = Assert.ThrowsAsync<Exception>(async () => await _userService.GetIdUserFromTokenAsync(changedToken));

            //assert
            Assert.That(ex1.Message, Is.EqualTo("Token is not valid"));
            Assert.That(ex2.Message, Is.EqualTo("Token is not valid"));
        }
    }
}
