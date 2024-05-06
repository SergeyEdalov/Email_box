using Message.Database.Context;
using Message.Database.Entity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using User.DataBase.Context;
using User.DataBase.Entity;

namespace SolutionTests.TestDbContext
{
    public class TestDbContext
    {
        public TestDbContext() { }
        public static MessageContext CreateTableMessages()
        {
            var builder = new DbContextOptionsBuilder<MessageContext>()
                .UseInMemoryDatabase("MessageTestDbContext");

            var options = builder.Options;

            var context = new MessageContext(options);

            context.Database.EnsureCreated();
            context.Messages.AddRange(
                new MessageEntity
                {
                    Id = new Guid(),
                    Body = "Hello",
                    FromUserId = Guid.Parse("4530fc35-b32f-4da0-84a5-1baaf8f7fb38"),
                    TargetUserId = Guid.Parse("e6ad89aa-8885-4a7b-acf5-c3356ab09ea5"),
                    IsDelivery = false
                },
                new MessageEntity
                {
                    Id = new Guid(),
                    Body = "How are you?",
                    FromUserId = Guid.Parse("4530fc35-b32f-4da0-84a5-1baaf8f7fb38"),
                    TargetUserId = Guid.Parse("e6ad89aa-8885-4a7b-acf5-c3356ab09ea5"),
                    IsDelivery = false
                },
                new MessageEntity
                {
                    Id = new Guid(),
                    Body = "I`m fine",
                    FromUserId = Guid.Parse("e6ad89aa-8885-4a7b-acf5-c3356ab09ea5"),
                    TargetUserId = Guid.Parse("4530fc35-b32f-4da0-84a5-1baaf8f7fb38"),
                    IsDelivery = false
                }
                );
            context.SaveChanges();
            return context;
        }

        public static UserContext CreateTableUsers()
        {
            var builder = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase("MessageTestDbContext");

            var options = builder.Options;

            var context = new UserContext(options);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
