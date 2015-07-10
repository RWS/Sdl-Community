using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Sdl.Community.Productivity.API;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;

namespace Sdl.Community.Productivity.Services
{
    public class LeaderboardShareService
    {
        private readonly LeaderboardApi _leaderboardApi;
        private readonly TwitterAccountInfo _twitterAccountInfo;

        public LeaderboardShareService(LeaderboardApi leaderboardApi, TwitterPersistenceService twitterPersistenceService)
        {
            _leaderboardApi = leaderboardApi;
            _twitterAccountInfo = twitterPersistenceService.Load();
        }

        private IEnumerable<Parameter> CreateParameters(LeaderboardInfo leaderboardInfo)
        {
            var result = new List<Parameter>();
            if (leaderboardInfo == null) return result;

            result.Add(new Parameter
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
                Value = _twitterAccountInfo.AccessToken
            });
            result.Add(new Parameter
            {
                Name = "accessTokenSecret",
                Type = ParameterType.GetOrPost,
                Value = _twitterAccountInfo.AccessTokenSecret
            });

            return result;
        }

        public bool IsAlive()
        {
            return _leaderboardApi.IsAlive();
        }
        public bool ShareOnLeaderboard(LeaderboardInfo leaderboardInfo)
        {
            var request = new RestRequest(Method.POST) { Resource = "entries", RequestFormat = DataFormat.Json };
            request.Parameters.AddRange(CreateParameters(leaderboardInfo));

            return _leaderboardApi.ExecuteWithoutResponse(request);
        }
    }
}
