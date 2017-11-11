using Noam.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noam.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NoamEntities _context;

        public UnitOfWork(NoamEntities context)
        {
            _context = context;
            Tweets = new TweetRepository(_context);
            Users = new UserRepository(_context);
            Hashtags = new HashtagRepository(_context);
        }

        public ITweetRepository Tweets { get; private set; }

        public IUserRepository Users { get; private set; }

        public IHashtagRepository Hashtags { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
