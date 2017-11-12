using System;
using System.ComponentModel.DataAnnotations;

namespace Noam.Data.ViewModels
{
    public class UserViewModel
    {
        public long Id { get; set; }

        public string ScreenName { get; set; }

        public string FullName { get; set; }

        public bool IsVerified { get; set; }

        public int MentionCount { get; set; }

        public int TweetCount { get; set; }

        public DateTime CreatedDate { get; set; }

        public UserViewModel(User user)
        {
            Id = user.UserId;
            ScreenName = user.ScreenName;
            FullName = user.FullName;
            MentionCount = user.Mentions.Count;
            TweetCount = user.Tweets.Count;
            IsVerified = user.IsVerified;
            CreatedDate = user.CreatedDate;
        }
    }
}