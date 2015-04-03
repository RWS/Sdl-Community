using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;

namespace Sdl.Community.Productivity.Services
{
    public class ShareService
    {
        private readonly ProductivityService _productivityService;
        private readonly TwitterPersistenceService _twitterPersistenceService;
        private Logger _logger;
        private ILoggedUser _loggedUser;

        public ShareService(ProductivityService productivityService, TwitterPersistenceService twitterPersistenceService,
            Logger logger)
        {
            _productivityService = productivityService;
            _twitterPersistenceService = twitterPersistenceService;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
        {
            var twitterAccountInformation = _twitterPersistenceService.Load();

            TwitterCredentials.SetCredentials(twitterAccountInformation.AccessToken,
                twitterAccountInformation.AccessTokenSecret, Constants.ConsumerKey,
                Constants.ConsumerSecret);
            _loggedUser = User.GetLoggedUser();
        }

        public void Share()
        {
            var leaderboardInfo = new LeaderboardInfo
            {
                TwitterHandle = _loggedUser.ScreenName,
                Score = _productivityService.Score,
                Language = _productivityService.Language
            };

            //TODO: post to leaderboard

            ShareOnTwitter(leaderboardInfo);
        }

        private void ShareOnTwitter(LeaderboardInfo leaderboardInfo)
        {
            var newTweet = Tweet.CreateTweet(string.Format(Constants.TweetMessage, leaderboardInfo.Score));
            newTweet.Publish();
        }

    }
}
