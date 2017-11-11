using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Noam.Data.Interfaces
{
    public interface IHashtagRepository : IRepository<Hashtag>
    {
        bool HashtagAlreadyExists(string hashtagValue, out Hashtag hashtag);

        IEnumerable<Hashtag> GetMostCommonHashtags(int count);
        IEnumerable<Hashtag> GetMostCommonHashtags(int count, DateTime since);
        IEnumerable<Hashtag> GetMostCommonHashtags(int count, DateTime from, DateTime to);

        void AddTweetHashtag(TweetHashtag tweetHashtag);
    }
}
