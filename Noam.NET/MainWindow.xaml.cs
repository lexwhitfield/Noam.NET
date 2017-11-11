using Noam.Data;
using Noam.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TweetSharp;

namespace Noam.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string customerKey = "mnNH2Zc2y4sEy8QQlrV48aDnO";
        private const string customerSecret = "fe44eqbdNlyQMS2maoMTxGoOaFdeMEF1MREwCYrCpBvaPVLO1d";
        private const string accessToken = "3200528246-1UxPggoLVoQUY0pqj4H7FnSvW14DHfVJg7VDwBB";
        private const string accessSecret = "imMBxXUweuBLere0tVkPmcmAQAcK6HvfU6lztwXwKSebA";

        TwitterService service = new TwitterService(customerKey, customerSecret, accessToken, accessSecret);
        UnitOfWork unitOfWork = new UnitOfWork(new Data.NoamEntities());

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetAllTweetsForUser(string username)
        {
            tweetList.Items.Clear();
            retweetList.Items.Clear();
            replyList.Items.Clear();

            var currentTweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
            {
                ScreenName = username,
                Count = 100
            });

            foreach (var tweet in currentTweets)
            {
                if (unitOfWork.Tweets.TweetAlreadyExists(tweet.Id))
                    continue;

                // save the user
                TwitterUser tUser = tweet.User;
                User user;

                if (unitOfWork.Users.UserAlreadyExists(tUser.Id, out user) == false)
                {
                    user = CreateUser(tUser);

                    unitOfWork.Users.Add(user);
                }

                // save the tweet
                Tweet t = CreateTweet(tweet);

                unitOfWork.Tweets.Add(t);

                // save any linked hashtags
                if (tweet.Entities.HashTags.Any())
                {
                    HandleHashtags(tweet);
                }

                if (tweet.Entities.Mentions.Any())
                {
                    HandleMentions(tweet);
                }

                unitOfWork.Complete();

                // DISPLAY

                OutputNormalTweet(tweet);

            }
        }

        private void HandleMentions(TwitterStatus tweet)
        {
            foreach (TwitterMention mention in tweet.Entities.Mentions)
            {
                User mentionedUser;

                if (unitOfWork.Users.UserAlreadyExists(mention.Id, out mentionedUser) == false)
                {
                    TwitterUser mUser = service.GetUserProfileFor(new GetUserProfileForOptions
                    {
                        UserId = mention.Id,
                        ScreenName = mention.ScreenName
                    });

                    mentionedUser = CreateUser(mUser);

                    unitOfWork.Users.Add(mentionedUser);
                }

                Mention m = new Mention
                {
                    TweetId = tweet.Id,
                    UserId = mentionedUser.UserId
                };

                unitOfWork.Users.AddUserMention(m);
            }
        }

        private void HandleHashtags(TwitterStatus tweet)
        {
            foreach (TwitterHashTag hashtag in tweet.Entities.HashTags)
            {
                Hashtag h = new Hashtag();

                if (unitOfWork.Hashtags.HashtagAlreadyExists(hashtag.Text, out h) == false)
                {
                    h = new Hashtag
                    {
                        HashtagGuid = Guid.NewGuid(),
                        HashtagValue = hashtag.Text
                    };

                    unitOfWork.Hashtags.Add(h);
                }

                TweetHashtag th = new TweetHashtag
                {
                    TweetId = tweet.Id,
                    HashtagGuid = h.HashtagGuid
                };

                unitOfWork.Hashtags.AddTweetHashtag(th);
            }
        }

        private User CreateUser(TwitterUser tu)
        {
            User user = new User
            {
                UserId = tu.Id,
                ScreenName = tu.ScreenName,
                Description = tu.Description,
                IsGeoEnabled = tu.IsGeoEnabled ?? false,
                IsProtected = tu.IsProtected ?? false,
                IsTranslator = tu.IsTranslator ?? false,
                IsVerified = tu.IsVerified ?? false,
                Language = tu.Language,
                Timezone = tu.TimeZone,
                Url = tu.Url
            };

            return user;
        }

        private Tweet CreateTweet(TwitterStatus ts)
        {
            Tweet tweet = new Tweet
            {
                TweetId = ts.Id,
                UserId = ts.User.Id,
                CreatedDate = ts.CreatedDate,
                TweetText = CleanUpWhiteSpace(ts.TextDecoded),
                IsQuoteStatus = ts.IsQuoteStatus,
                IsRetweet = ts.Text.StartsWith("RT "),
                ReplyToTweetId = ts.InReplyToStatusId,
            };

            return tweet;
        }

        private string CleanUpWhiteSpace(string text)
        {
            while (text.Contains("\n"))
            {
                text = text.Replace("\n", " ");
            }

            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            return text;
        }

        private void OutputNormalTweet(TwitterStatus tweet)
        {
            string prefix = $"[{tweet.CreatedDate}] ";
            string tweetContent = (string.IsNullOrEmpty(tweet.Text)) ? "" : CleanUpWhiteSpace(tweet.Text);

            if (tweetList.Items.Contains(prefix + tweetContent) == false)
            {
                tweetList.Items.Add(prefix + tweetContent);
            }
            else
            {
                return;
            }
        }

        private void OutputReTweet(TwitterStatus tweet)
        {
            string prefix = $"[{tweet.CreatedDate}] ";
            string tweetContent = (string.IsNullOrEmpty(tweet.Text)) ? "" : CleanUpWhiteSpace(tweet.Text);

            if (retweetList.Items.Contains(prefix + tweetContent) == false)
            {
                retweetList.Items.Add(prefix + tweetContent);
            }
            else
            {
                return;
            }
        }

        private void OutputReplyTweet(TwitterStatus tweet)
        {
            string prefix = $"[{tweet.CreatedDate}] ";
            string tweetContent = (string.IsNullOrEmpty(tweet.Text)) ? "" : CleanUpWhiteSpace(tweet.Text);

            if (replyList.Items.Contains(prefix + tweetContent) == false)
            {
                replyList.Items.Add(prefix + tweetContent);
            }
            else
            {
                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetAllTweetsForUser(usernameTextBox.Text);
        }
    }
}
