using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;

namespace Sdl.Community.Productivity.API
{
    public class LeaderboardApi
    {
        private readonly string _baseUrl = PluginResources.Leaderboard_Link;
        private readonly TwitterAccountInfo _twitterAccountInfo;
        private int _numberOfRetries;
        public LeaderboardApi(TwitterPersistenceService twitterPersistence)
        {
            _numberOfRetries = 0;
            _twitterAccountInfo = twitterPersistence.Load();
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            T typedResponse;
            try
            {
                var response = InternalExecute(request);

                typedResponse = JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception)
            {
                if (_numberOfRetries > 3) throw;
                //try again
                _numberOfRetries++;
                typedResponse = Execute<T>(request);
            }

            return typedResponse;
        }

        private IRestResponse InternalExecute(RestRequest request)
        {
            var client = new RestClient(new Uri(new Uri(_baseUrl), "api"));
          //  request.AddParameter("Authorization", _twitterAccountInfo.AccessToken, ParameterType.HttpHeader);
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info";
                var leaderboardException = new ApplicationException(message, response.ErrorException);
                throw leaderboardException;
            }
            return response;
        }

        public bool ExecuteWithoutResponse(RestRequest request)
        {
            var result = true;
            try
            {
                var response = InternalExecute(request);
            }
            catch (Exception)
            {
                if (_numberOfRetries > 3) result = false;
                //try again
                _numberOfRetries++;
                ExecuteWithoutResponse(request);
            }

            return result;
        }



    }
}
