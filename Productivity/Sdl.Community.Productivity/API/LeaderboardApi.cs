using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog;
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
        private Logger _logger;
        public LeaderboardApi(TwitterPersistenceService twitterPersistence)
        {
            _numberOfRetries = 0;
            _twitterAccountInfo = twitterPersistence.Load();
            _logger = LogManager.GetLogger("log");
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


        public string Execute(RestRequest request)
        {
            string typedResponse;
            try
            {
                var response = InternalExecute(request);

                typedResponse = response.Content;
            }
            catch (Exception)
            {
                if (_numberOfRetries > 3) throw;
                //try again
                _numberOfRetries++;
                typedResponse = Execute(request);
            }

            return typedResponse;
        }

        public bool IsAlive()
        {
            var request = new RestRequest(Method.GET) { Resource = "version", RequestFormat = DataFormat.Json };
            var response = InternalExecute(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        private IRestResponse InternalExecute(RestRequest request)
        {
            var client = new RestClient(new Uri(new Uri(_baseUrl), "api"));
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info";
                var leaderboardException = new ApplicationException(message, response.ErrorException);
               _logger.Error(leaderboardException);
            }
            return response;
        }

        public bool ExecuteWithoutResponse(RestRequest request)
        {
            var result = true;
            try
            {
                InternalExecute(request);
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
