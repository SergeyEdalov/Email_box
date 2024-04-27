using Message.Database.Context;
using Message.Database.Entity;
using Microsoft.EntityFrameworkCore;
using User.DataBase.Context;

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

            //var salt = new byte[16];
            //new Random().NextBytes(salt);

            //context.Users.AddRange
            //    (
            //        new UserEntity
            //        {
            //            Id = new Guid("24cc81fb-5303-4e81-8da7-eeccc63d2531"),
            //            Email = "admin@mail.ru",
            //            Password = Encoding.UTF8.GetBytes("Qwerty1)"),
            //            Salt = salt,
            //            RoleId = (Role)1
            //        },
            //        new UserEntity
            //        {
            //            Id = new Guid("c5d627e4-e124-4811-a3ee-730a940b074f"),
            //            Email = "bob@mail.ru",
            //            Password = Encoding.UTF8.GetBytes("Hh45!4"),
            //            Salt = salt,
            //            RoleId = (Role)1
            //        },
            //        new UserEntity
            //        {
            //            Id = new Guid("c2cd92fb-5b4c-42d1-9cc2-8d12be77aa88"),
            //            Email = "masha@mail.ru",
            //            Password = Encoding.UTF8.GetBytes("UIop!23"),
            //            Salt = salt,
            //            RoleId = (Role)1
            //        }
            //    );
            //context.SaveChanges();
            return context;
        }
    }
}
