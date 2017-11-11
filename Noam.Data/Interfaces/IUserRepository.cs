using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noam.Data.Interfaces
{
    public interface IUserRepository :IRepository<User>
    {
        bool UserAlreadyExists(long userId, out User user);

        void AddUserMention(Mention m);
    }
}
