using Message.Database.Context;
using Microsoft.EntityFrameworkCore;
using User.DataBase.Context;

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

        public static void Destroy(DbContext context)
        {
            context.Dispose();
        }
    }
}
