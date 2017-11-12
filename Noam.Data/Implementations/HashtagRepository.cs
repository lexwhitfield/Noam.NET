using Noam.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Noam.Data.Implementations
{
    public class HashtagRepository : Repository<Hashtag>, IHashtagRepository
    {
        public HashtagRepository(DbContext context) : base(context)
        {

        }

        public IEnumerable<Hashtag> GetMostCommonHashtags(int count)
        {
            return NoamContext.Hashtags.OrderByDescending(o => o.TweetHashtags.Count).Take(count).ToList();
        }

        public IEnumerable<Hashtag> GetMostCommonHashtags(int count, DateTime since)
        {
            // probably wrong
            return NoamContext.Hashtags.Where(o => o.TweetHashtags.Any(a => a.Tweet.CreatedDate >= since)).OrderByDescending(o => o.TweetHashtags.Count).Take(count).ToList();
        }

        public IEnumerable<Hashtag> GetMostCommonHashtags(int count, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public bool HashtagAlreadyExists(string hashtagValue, out Hashtag hashtag)
        {
            hashtagValue = hashtagValue.ToLower();

            var hashtags = (from a in NoamContext.Hashtags where a.HashtagValue == hashtagValue select a);

            if (hashtags.Any())
            {
                hashtag = hashtags.First();
            }
            else
            {
                hashtag = null;
            }

            return (hashtag == null) ? false : true;
        }

        public void AddTweetHashtag(TweetHashtag tweetHashtag)
        {
            NoamContext.TweetHashtags.Add(tweetHashtag);
        }

        public NoamEntities NoamContext
        {
            get { return (NoamEntities)dbContext; }
        }
    }
}
