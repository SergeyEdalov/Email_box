using System.Data;
using User.Models;

namespace User.DataBase.Entity
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public virtual Role Role { get; set; }
    }
}
