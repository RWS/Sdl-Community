using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using RestSharp;
using Sdl.Community.Productivity.API;
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
        private readonly TweetMessageService _tweetMessageService;
        private readonly LeaderboardApi _leaderboardApi;
        private Logger _logger;
        private ILoggedUser _loggedUser;

        public ShareService(ProductivityService productivityService,
            TwitterPersistenceService twitterPersistenceService,
            TweetMessageService tweetMessageService,
            LeaderboardApi leaderboardApi,
            Logger logger)
        {
            _productivityService = productivityService;
            _twitterPersistenceService = twitterPersistenceService;
            _tweetMessageService = tweetMessageService;
            _leaderboardApi = leaderboardApi;
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

        public string GetTwitterMessage()
        {
            return _tweetMessageService.GetTwitterMessage(_productivityService.Score);
        }

        public void Share()
        {
            var leaderboardInfo = new LeaderboardInfo
            {
                TwitterHandle = _loggedUser.ScreenName,
                Score = _productivityService.Score,
                Language = _productivityService.Language,
                LastTranslationAt = _productivityService.LastTranslationDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                AppVersion = "3"
            };

            var isSharedOnTheLeaderboard = ShareOnLeaderboard(leaderboardInfo);

            if (isSharedOnTheLeaderboard &&
                bool.FalseString.Equals(PluginResources.IsStaging, StringComparison.InvariantCultureIgnoreCase))
            {
                ShareOnTwitter(leaderboardInfo);
            }
        }

        private bool ShareOnLeaderboard(LeaderboardInfo leaderboardInfo)
        {
            var request = new RestRequest(Method.POST) { Resource = "entries", RequestFormat = DataFormat.Json };
            request.Parameters.AddRange(CreateParameters(leaderboardInfo));

            return _leaderboardApi.ExecuteWithoutResponse(request);
        }

        private IEnumerable<Parameter> CreateParameters(LeaderboardInfo leaderboardInfo)
        {
            var result = new List<Parameter>();
            if (leaderboardInfo == null) return result;

            result.Add( new Parameter
            {
                Name = "twitterHandle",
                Type = ParameterType.GetOrPost,
                Value = leaderboardInfo.TwitterHandle
            });
            result.Add(new Parameter
            {
                Name = "score",
                Type = ParameterType.GetOrPost,
                Value = leaderboardInfo.Score
            });
            result.Add(new Parameter
            {
                Name = "language",
                Type = ParameterType.GetOrPost,
                Value = leaderboardInfo.Language
            });
            result.Add(new Parameter
            {
                Name = "lastTranslationAt",
                Type = ParameterType.GetOrPost,
                Value = leaderboardInfo.LastTranslationAt
            });
            result.Add(new Parameter
            {
                Name = "appVersion",
                Type = ParameterType.GetOrPost,
                Value = leaderboardInfo.AppVersion
            });

            return result;
        }

        private void ShareOnTwitter(LeaderboardInfo leaderboardInfo)
        {
            var newTweet = Tweet.CreateTweet(_tweetMessageService.GetTwitterMessage(leaderboardInfo.Score));
            newTweet.Publish();
        }

    }
}
