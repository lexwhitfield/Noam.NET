using Noam.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Noam.Data.Implementations
{
    public class TweetRepository : Repository<Tweet>, ITweetRepository
    {
        public TweetRepository(DbContext context) : base(context)
        {

        }

        /// <summary>
        /// Gets all tweets for a given user
        /// </summary>
        /// <param name="userId">The target user</param>
        /// <returns></returns>
        public IEnumerable<Tweet> GetAllTweetsForUser(long userId)
        {
            return NoamContext.Tweets.Where(o => o.UserId == userId).ToList();
        }

        /// <summary>
        /// Gets all tweets for a given user since (inclusive) the date given
        /// </summary>
        /// <param name="userId">The target user</param>
        /// <param name="since">Gets all tweets after (and on) this datetime</param>
        /// <returns></returns>
        public IEnumerable<Tweet> GetAllTweetsForUser(long userId, DateTime since)
        {
            return NoamContext.Tweets.Where(o => o.UserId == userId).Where(o => o.CreatedDate >= since).ToList();
        }

        /// <summary>
        /// Gets specified number of (most recent) tweets for a given user 
        /// </summary>
        /// <param name="userId">The target user</param>
        /// <param name="count">Number of tweets to return</param>
        /// <returns></returns>
        public IEnumerable<Tweet> GetAllTweetsForUser(long userId, int count)
        {
            return NoamContext.Tweets.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedDate).Take(count).ToList();
        }

        public IEnumerable<Tweet> GetAllRetweetsForUser(long userId)
        {
            return NoamContext.Tweets.Where(o => o.UserId == userId && o.IsRetweet).ToList();
        }

        public IEnumerable<Tweet> GetAllRetweetsForUser(long userId, DateTime since)
        {
            return NoamContext.Tweets.Where(o => o.UserId == userId && o.IsRetweet).Where(o => o.CreatedDate >= since).ToList();
        }

        public IEnumerable<Tweet> GetAllRetweetsForUser(long userId, int count)
        {
            return NoamContext.Tweets.Where(o => o.UserId == userId && o.IsRetweet).OrderByDescending(o => o.CreatedDate).Take(count).ToList();
        }

        public bool TweetAlreadyExists(long tweetId)
        {
            return NoamContext.Tweets.Any(o => o.TweetId == tweetId);
        }

        public IEnumerable<Tweet> GetAllTweetsWithHashtag(string hashtagGuid)
        {
            return NoamContext.Tweets.Where(o => o.TweetHashtags.Count > 0).Where(o => o.TweetHashtags.Any(a => a.HashtagGuid.ToString().Equals(hashtagGuid))).ToList();
        }

        public IEnumerable<Tweet> GetAllTweetsWithHashtagSince(string hashtagGuid, DateTime since)
        {
            return NoamContext.Tweets.Where(o => o.TweetHashtags.Count > 0).Where(o => o.TweetHashtags.Any(a => a.HashtagGuid.ToString().Equals(hashtagGuid))).Where(o => o.CreatedDate >= since).ToList();
        }

        public NoamEntities NoamContext
        {
            get { return (NoamEntities)dbContext; }
        }
    }
}
