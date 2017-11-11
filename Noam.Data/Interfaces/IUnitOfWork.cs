using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noam.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITweetRepository Tweets { get; }
        IUserRepository Users { get; }
        IHashtagRepository Hashtags { get; }

        int Complete();
    }
}
