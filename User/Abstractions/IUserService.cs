using User.DataBase.DTO;
using User.Models;

namespace User.Abstractions
{
    public interface IUserService
    {
        public Guid AddAdmin(UserModel userModel);
        public Guid AddUser(UserModel userModel);
        public IEnumerable<UserDto> GetListUsers();
        public void DeleteUser(string userName);
        public Guid GetIdIserFromToken (string token);  
    }
}
