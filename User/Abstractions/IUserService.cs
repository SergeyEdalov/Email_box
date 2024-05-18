using User.DataBase.DTO;
using User.Models;

namespace User.Abstractions
{
    public interface IUserService <T, E> where T : class where E : class
    {
        public Task AddAdminAsync(T userModel);
        public Task AddUserAsync(T userModel);
        public Task<IEnumerable<E>> GetListUsersAsync();
        public Task DeleteUserAsync(string userName);
        public Task<Guid> GetIdUserFromTokenAsync (string token);  
    }
}
