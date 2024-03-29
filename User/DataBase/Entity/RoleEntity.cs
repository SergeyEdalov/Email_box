namespace User.DataBase.Entity
{
    public partial class RoleEntity
    {
        public Role RoleId { get; set; }
        public string Name { get; set; }
        public virtual List<UserEntity> Users { get; set; }
    }
}
