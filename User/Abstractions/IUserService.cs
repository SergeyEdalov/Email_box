using User.DataBase.DTO;
using User.Models;

namespace User.Abstractions
{
    public interface IUserService
    {
        public Task AddAdminAsync(UserModel userModel);
        public Task AddUserAsync(UserModel userModel);
        public Task<IEnumerable<UserDto>> GetListUsersAsync();
        public Task DeleteUserAsync(string userName);
        public Task<Guid> GetIdUserFromTokenAsync (string token);  
    }
}
