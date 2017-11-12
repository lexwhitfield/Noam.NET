
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using TweetSharp;

using Noam.Data;
using Noam.Data.ViewModels;
using Noam.Data.Implementations;

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
        UnitOfWork unitOfWork = new UnitOfWork(new NoamEntities());

        private readonly BackgroundWorker worker = new BackgroundWorker();

        Dictionary<string, int> mentionCounter = new Dictionary<string, int>();
        List<UserViewModel> mainUserList = new List<UserViewModel>();

        public MainWindow()
        {
            InitializeComponent();

            UpdateMainUserList();
        }

        private void UpdateMainUserList()
        {
            mainUserList.Clear();

            foreach (var user in unitOfWork.Users.All())
            {
                mainUserList.Add(new UserViewModel(user));
            }

            if (verifiedUserCheckBox.IsChecked ?? false)
            {
                mainUserList = (from a in mainUserList where a.IsVerified == true select a).ToList();
            }

            mainUserList = mainUserList.OrderByDescending(o => o.MentionCount).ToList();

            userGrid.ItemsSource = mainUserList.ToArray();
        }

        private void GetAllTweetsForUser(string username)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                tweetList.Items.Clear();
                retweetList.Items.Clear();
                replyList.Items.Clear();
                mentionCounter.Clear();
            }));

            var currentTweets = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
            {
                ScreenName = username,
                Count = 200
            });

            TwitterRateLimitStatus rateStatus = service.Response.RateLimitStatus;

            if (currentTweets == null)
            {
                MessageBox.Show($"Couldn't find any tweets for this user. Rate limits remaining: {rateStatus.RemainingHits}");
                return;
            }

            int tweetCount = currentTweets.Count();

            Dispatcher.BeginInvoke(new Action(delegate
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = tweetCount;
                progressBar.Value = 0;
            }));

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
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (tweet.Text.StartsWith("RT "))
                        OutputReTweet(tweet);
                    else if (tweet.InReplyToStatusId.HasValue)
                        OutputReplyTweet(tweet);
                    else
                        OutputNormalTweet(tweet);

                    progressBar.Value += 1;
                }));
            }

            Dispatcher.BeginInvoke(new Action(delegate
            {
                UpdateMainUserList();
            }));
        }

        private void HandleMentions(TwitterStatus tweet)
        {
            foreach (TwitterMention mention in tweet.Entities.Mentions)
            {
                // remove this to capture self-mentions
                if (mention.ScreenName == tweet.User.ScreenName)
                    continue;

                User mentionedUser;

                if (unitOfWork.Users.UserAlreadyExists(mention.Id, out mentionedUser) == false)
                {
                    TwitterUser mUser = service.GetUserProfileFor(new GetUserProfileForOptions
                    {
                        UserId = mention.Id,
                        ScreenName = mention.ScreenName
                    });

                    if (mUser == null)
                        continue;

                    mentionedUser = CreateUser(mUser);

                    unitOfWork.Users.Add(mentionedUser);
                }

                Mention m = new Mention
                {
                    TweetId = tweet.Id,
                    UserId = mentionedUser.UserId
                };

                unitOfWork.Users.AddUserMention(m);

                Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (mentionCounter.Keys.Contains(mention.ScreenName) == false)
                    {
                        mentionCounter.Add(mention.ScreenName, 1);
                    }
                    else
                    {
                        mentionCounter[mention.ScreenName] += 1;
                    }

                    // re-sort by value
                    var sortedDictionary = mentionCounter.OrderByDescending(o => o.Value);
                    mentionCounter = sortedDictionary.ToDictionary(pair => pair.Key, pair => pair.Value);

                    mentionGrid.ItemsSource = mentionCounter;
                }));
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
                FullName = tu.Name,
                ScreenName = tu.ScreenName,
                Description = tu.Description,
                IsGeoEnabled = tu.IsGeoEnabled ?? false,
                IsProtected = tu.IsProtected ?? false,
                IsTranslator = tu.IsTranslator ?? false,
                IsVerified = tu.IsVerified ?? false,
                Language = tu.Language,
                Timezone = tu.TimeZone,
                Url = tu.Url,
                CreatedDate = tu.CreatedDate,
                Location = tu.Location
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            await Task.Run(() => GetAllTweetsForUser(username));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateMainUserList();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateMainUserList();
        }

        private void userGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            UserViewModel user = (UserViewModel)e.AddedItems[0];

            displayUserControl.SelectedUser = user;

            userTweetGrid.ItemsSource = unitOfWork.Users.Get(user.Id).Tweets.OrderByDescending(o => o.CreatedDate).ToList();
        }

        private async void GetTWeetsForUser_Button_Click(object sender, RoutedEventArgs e)
        {
            UserViewModel user = ((FrameworkElement)sender).DataContext as UserViewModel;

            await Task.Run(() => GetAllTweetsForUser(user.ScreenName));
        }
    }
}
