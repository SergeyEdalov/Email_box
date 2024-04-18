using User.Models;

namespace User.Abstractions
{
    public interface IUserAuthenticationService
    {
        public Task<string> AuthenticateAsync (LoginModel loginModel);

        //public string AuthenticateMock(LoginModel loginModel); //Заглушка
    }
}
