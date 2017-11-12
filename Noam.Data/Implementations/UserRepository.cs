using Noam.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Noam.Data.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {

        }

        public bool UserAlreadyExists(long userId, out User user)
        {
            user = Get(userId);
            return (user == null) ? false : true;
        }

        public void AddUserMention(Mention m)
        {
            NoamContext.Mentions.Add(m);
        }

        public NoamEntities NoamContext
        {
            get { return (NoamEntities)dbContext; }
        }
    }
}
