using User.Models;

namespace User.Abstractions
{
    public interface IUserAuthenticationService <T>
    {
        public Task<string> AuthenticateAsync (T loginModel);
    }
}
