using Message.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Message.Database.Context
{
    public class MessageContext : DbContext
    {
        private static string? _connectionString;
        public DbSet<MessageEntity> Messages { get; set; }

        public MessageContext() { }

        public MessageContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MessageContext(DbContextOptions<MessageContext> options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString).UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MessageEntity>(entity =>
            {
                entity.HasKey(x => x.Id)
                    .HasName("message_key");

                entity.ToTable("messages");

                entity.Property(x => x.Id)
                    .HasColumnName("id");
                entity.Property(e => e.Body)
                    .HasColumnName("message");
                entity.Property(e => e.FromUserId)
                    .HasColumnName("fromUserId");
                entity.Property(e => e.TargetUserId)
                    .HasColumnName("targetUserId");
                entity.Property(e => e.IsDelivery)
                    .HasColumnName("status");
            });
        }
    }
}
