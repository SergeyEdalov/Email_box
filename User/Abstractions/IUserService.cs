using User.DataBase.DTO;
using User.DataBase.Entity;

namespace User.Abstractions
{
    public interface IUserService
    {
        public Guid AddAdmin(string email, string password);
        public Guid AddUser(string email, string password, Role role);
        public IEnumerable<UserDto> GetListUsers();
        public void DeleteUser(string userName);
    }
}
