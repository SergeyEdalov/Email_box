using Microsoft.EntityFrameworkCore;
using System.Data;
using User.DataBase.Entity;

namespace User.DataBase.Context
{
    public class UserContext : DbContext
    {
        private static string? _connectionString;

        public UserContext() { }

        public UserContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<UserEntity> Users { get; set; }

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
                    .HasName("User_id");
                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.Property(e => e.Email)
                    .HasColumnName("User_login");
                entity.Property(e => e.Password).IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("User_assword");
            });
        }
    }
}
