using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;
using Sdl.Community.Productivity.Util;
using TweetSharp;

namespace Sdl.Community.Productivity.Services
{
    public class TwitterShareService
    {
        private readonly TwitterPersistenceService _twitterPersistenceService;
        private readonly TweetMessageService _tweetMessageService;
        private readonly Logger _logger;
        private TwitterService _twitterService;
        private bool _authenticated;

        public TwitterShareService(TwitterPersistenceService twitterPersistenceService,TweetMessageService tweetMessageService)
        {
            _twitterPersistenceService = twitterPersistenceService;
            _tweetMessageService = tweetMessageService;
            _logger = LogManager.GetLogger("log");
            Authenticate();
        }

        private void Authenticate()
        {
            if (!ProductivityUiHelper.IsTwitterAccountConfigured(_twitterPersistenceService, _logger))
            {
                _authenticated = false;
                return;
            }
            var twitterAccountInformation = _twitterPersistenceService.Load();
            _twitterService = new TwitterService(Constants.ConsumerKey,
                Constants.ConsumerSecret);
            _twitterService.AuthenticateWith(twitterAccountInformation.AccessToken,
                twitterAccountInformation.AccessTokenSecret);
            _authenticated = true;
        }

        public TwitterUser GetUserProfile()
        {
            if (!_authenticated) return null;
            var getUpo = new GetUserProfileOptions()
            {
                IncludeEntities = false,
                SkipStatus = false
            };
            return _twitterService.GetUserProfile(getUpo);
        }

        public bool IsAlive()
        {
            if (!_authenticated) return false;
            var twitterAccount = _twitterService.GetAccountSettings();
            return twitterAccount != null;
        }

        public void ShareOnTwitter(LeaderboardInfo leaderboardInfo)
        {
            if (!_authenticated) return;
            var tweetOptions = new SendTweetOptions
            {
                Status = _tweetMessageService.GetTwitterMessage(leaderboardInfo.Score)
            };
            _twitterService.SendTweet(tweetOptions);
        }
    }
}
