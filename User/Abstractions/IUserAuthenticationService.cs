using User.Models;

namespace User.Abstractions
{
    public interface IUserAuthenticationService
    {
        public string Authenticate (LoginModel loginModel);

        //public string AuthenticateMock(LoginModel loginModel); //Заглушка
    }
}
