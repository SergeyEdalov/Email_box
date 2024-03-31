using Microsoft.EntityFrameworkCore;
using System.Data;
using User.DataBase.Entity;

namespace User.DataBase.Context
{
    public partial class UserContext : DbContext
    {
        private static string? _connectionString;

        public UserContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString).UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(x => x.Id)
                    .HasName("user_pkey");
                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.ToTable("users");

                entity.Property(x => x.Id)
                    .HasColumnName("id");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("user_login");
                entity.Property(e => e.Password)
                    .HasColumnName("user_password");
                entity.Property(e => e.Salt)
                    .HasColumnName("salt");

                entity.Property(e => e.RoleId).HasConversion<int>();
            });

            modelBuilder.Entity<RoleEntity>(entity =>
            {
                entity.HasKey(x => x.RoleId)
                    .HasName("role_id");
                entity.HasIndex(x => x.Name);
            });

            modelBuilder.Entity<RoleEntity>()
                .Property(e => e.RoleId)
                .HasConversion<int>();

            modelBuilder.Entity<RoleEntity>()
                .HasData
                (Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(e => new RoleEntity
                {
                    RoleId = e,
                    Name = e.ToString()
                }));
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
