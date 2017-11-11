using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noam.Data.Interfaces
{
    public interface ITweetRepository : IRepository<Tweet>
    {
        IEnumerable<Tweet> GetAllTweetsForUser(long userId);
        IEnumerable<Tweet> GetAllTweetsForUser(long userId, DateTime since);

        IEnumerable<Tweet> GetAllRetweetsForUser(long userId);
        IEnumerable<Tweet> GetAllRetweetsForUser(long userId, DateTime since);

        IEnumerable<Tweet> GetAllTweetsWithHashtag(string hashtagGuid);
        IEnumerable<Tweet> GetAllTweetsWithHashtagSince(string hashtagGuid, DateTime since);

        bool TweetAlreadyExists(long tweetId);
    }
}
