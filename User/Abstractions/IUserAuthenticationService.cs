using User.Models;

namespace User.Abstractions
{
    public interface IUserAuthenticationService
    {
        string Authenticate (LoginModel loginModel);

        public string AuthenticateMock(LoginModel loginModel);
    }
}
