using Message.Database.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using User.DataBase.Context;
using User.DataBase.Entity;

namespace SolutionTests.TestDbContext
{
    public class TestCommandBase
    {
        public UserContext _userContext;
        public MessageContext _messageContext;

        public static MessageContext getMContext() 
        {
            return TestDbContext.CreateTableMessages();
        }

        public static UserContext getUContext()
        {
            return TestDbContext.CreateTableUsers();
        }

        public static void CreateUserEntityToUserContext(string email, string password,  UserContext context, Role roleId = Role.User)
        {
            var salt = new byte[16];
            new Random().NextBytes(salt);
            var bytePassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            SHA512 shaM = new SHA512Managed();
            bytePassword = shaM.ComputeHash(bytePassword);

            var userEntity = new UserEntity
            {
                Id = new Guid(),
                Email = email,
                Password = bytePassword,
                Salt = salt,
                RoleId = roleId
            };
            context.Users.Add(userEntity);
            context.SaveChanges();
        }

        public static void CleanUserContext (UserContext context)
        {
            context.Users.RemoveRange(context.Users.Select(x => x));
            context.SaveChanges();
        }

        public static void CleanMessageContext(MessageContext context)
        {
            context.Messages.RemoveRange(context.Messages.Select(x => x));
            context.SaveChanges();
        }

        public static void Destroy(DbContext context)
        {     
            context.Dispose();
        }
    }
}
