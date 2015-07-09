using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using Sdl.Community.Productivity.API;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;
using TweetSharp;

namespace Sdl.Community.Productivity.Services
{
    public class ShareService
    {
        private readonly ProductivityService _productivityService;
        private readonly TwitterPersistenceService _twitterPersistenceService;
        private readonly TweetMessageService _tweetMessageService;
        private readonly LeaderboardApi _leaderboardApi;
        private Logger _logger;
        private TwitterService _twitterService;
        private TwitterAccount _twitterAccount;
        private TwitterAccountInfo _twitterAccountInformation;
        public Version LeaderboardVersion;
        public Version PluginVersion;

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
            _twitterAccountInformation = _twitterPersistenceService.Load();
            _twitterService = new TwitterService(Constants.ConsumerKey,
                Constants.ConsumerSecret);
            _twitterService.AuthenticateWith(_twitterAccountInformation.AccessToken,
                _twitterAccountInformation.AccessTokenSecret);
            _twitterAccount = _twitterService.GetAccountSettings();
            LeaderboardVersion = new Version(GetLeaderboardVersion());
            PluginVersion = new Version(GetVersion());
        }

        public string GetTwitterMessage()
        {
            return _tweetMessageService.GetTwitterMessage(_productivityService.Score);
        }

        public bool CanShareOnLeaderBoadrd()
        {
            return LeaderboardVersion.CompareTo(PluginVersion) <=0;
        }

        public bool IsLeaderboardAvailable()
        {
            return _leaderboardApi.IsAlive();
        }

        public bool IsTwitterAvailable()
        {
            return _twitterAccount != null;
        }
        public void Share()
        {
            if (!CanShareOnLeaderBoadrd()) return;
            if (!IsLeaderboardAvailable()) return;
            if (!IsTwitterAvailable()) return;
            var leaderboardInfo = new LeaderboardInfo
            {
                TwitterHandle = _twitterAccount.ScreenName,
                Score = _productivityService.Score,
                Language = _productivityService.Language,
                LastTranslationAt = _productivityService.LastTranslationDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                AppVersion = GetVersion()
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

        private string GetLeaderboardVersion()
        {
            if (!_leaderboardApi.IsAlive()) return GetVersion();
            var request = new RestRequest(Method.GET) { Resource = "version", RequestFormat = DataFormat.Json };
            dynamic dVersion = JObject.Parse(_leaderboardApi.Execute(request));
            return dVersion.version;
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

            result.Add(new Parameter
            {
                Name = "accessToken",
                Type = ParameterType.GetOrPost,
                Value = _twitterAccountInformation.AccessToken
            });
            result.Add(new Parameter
            {
                Name = "accessTokenSecret",
                Type = ParameterType.GetOrPost,
                Value = _twitterAccountInformation.AccessTokenSecret
            });

            return result;
        }

        private void ShareOnTwitter(LeaderboardInfo leaderboardInfo)
        {
            var tweetOptions = new SendTweetOptions
            {
                Status = _tweetMessageService.GetTwitterMessage(leaderboardInfo.Score)
            };
            _twitterService.SendTweet(tweetOptions);
        }

        private string GetVersion()
        {
            var assembly = typeof (ShareService).Assembly;
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fullVersion = versionInfo.FileVersion;
            return fullVersion;
        }

    }
}
