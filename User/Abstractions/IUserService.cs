using User.DataBase.DTO;
using User.Models;

namespace User.Abstractions
{
    public interface IUserService
    {
        public Task AddAdminAsync(UserModel userModel);
        public Task AddUser(UserModel userModel);
        public Task<IEnumerable<UserDto>> GetListUsers();
        public void DeleteUser(string userName);
        public Task<Guid> GetIdIserFromToken (string token);  
    }
}
