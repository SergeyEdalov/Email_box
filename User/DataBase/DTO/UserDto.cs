using User.DataBase.Entity;

namespace User.DataBase.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public Role RoleId { get; set; }
    }
}
