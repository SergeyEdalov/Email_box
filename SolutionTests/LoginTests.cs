using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using SolutionTests.TestDbContext;
using User.DataBase.DTO;
using User.DataBase.Entity;
using User.Models;
using User.Services;

namespace SolutionTests
{
    [TestFixture]
    class LoginTests : TestCommandBase
    {
        Mock<IMapper> _userMockMapper;
        private IConfiguration? _configuration;
        UserAuthenticationService _userAuthenticationService;
        LoginModel _loginModel;

        [OneTimeSetUp]
        public void Init()
        {
            _userMockMapper = new Mock<IMapper>();
            _userContext = getUContext();
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _userAuthenticationService = new UserAuthenticationService(_userContext, _userMockMapper.Object, _configuration, null); ; ;

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

            _loginModel = new LoginModel()
            {
                Email = "masha@mail.ru",
                Password = "UIop!23",
            };

            CreateUserEntityToUserContext("masha@mail.ru", "UIop!23", _userContext);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _userContext.Users.Remove(_userContext.Users.FirstOrDefault());
            _userContext.SaveChanges();
            Destroy(_userContext);
        }

        [Test]
        public async Task Login_DefaultSuccsess_ExpectedToken()
        {
            // arrage
            var expectedToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImMyY2Q5MmZiLTViNGMtNDJkMS05Y2MyLThkMTJiZTc3YWE4OCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MTQyMjE4OTEsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyMDUiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MjA1In0.fj5tbYvB-xzfPTSHVSU1a7oJR3JXNVUGMtu8e_DuwMEt2a-AzeivGB4z0CZtQP0edUrmxn15JhIdvj1qd95Ng5I6tWHw0XvkasbcaBhS4j6hYeeGdvyCAA8sG_FHckabFnjhUTlIxKy-0Zx-Q7vC6tcjVZGT2tjfnCxoJzagY7D8PtZ5Dkv9834EC-qkEHRKv0mr_TlQCiId10fv0UUC0-q6eiwaw7xHsDoP5ZpYLqWwKQ4pomC5ms8J5XZy5PXv_pi1DCfslj5oo6iO_sG1UFZYdZflVou-A2kbyF7fJh6I5WGh4vNYSsEanBcuwmIwfkAmkpvV-Py1Zv6Td1pHPA";

            //act
            var actualToken = await _userAuthenticationService.AuthenticateAsync(_loginModel);

            //assert
            Assert.That(actualToken.Length == expectedToken.Length);
            Assert.That(actualToken.Contains("eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0"));
        }

        [Test]
        public async Task Login_expectedUserNotFound()
        {
            // arrage
            _loginModel.Email = "Oleg@mail.ru";
            var expected = "User not found";

            //act
            var actual = await _userAuthenticationService.AuthenticateAsync(_loginModel);

            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task Login_expectedPasswordNotFound()
        {
            // arrage
            _loginModel.Email = "masha@mail.ru";
            _loginModel.Password = "UIop!2386";
            var expected = "Wrong password";

            //act
            var actual = await _userAuthenticationService.AuthenticateAsync(_loginModel);

            //assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
