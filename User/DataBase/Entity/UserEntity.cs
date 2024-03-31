namespace User.DataBase.Entity
{
    public partial class UserEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public Role RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }
    }
}
