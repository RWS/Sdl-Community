using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.Jobs.Model;

namespace Sdl.Community.Jobs.API
{
    public class ProzApi
    {
        private const string BaseUrl = "https://api.proz.com/v2";
        private const string ClientId = "d09c67edad450fac837cfa38a135f5b62cf7c42d";
        private const string ClientSecret = "f125d5d7bfe2551337f01f9f0f4d539afacfec96";
        private ProzAccessToken _accessToken;
        private int _numberOfRetries;
        public ProzApi()
        {
            _numberOfRetries = 0;
            _accessToken = new ProzAccessToken();
        }


        public T Execute<T>(RestRequest request) where T : new()
        {
            T typedResponse;
            try
            {

           
            _accessToken = Authenticate();

            var client = new RestClient(BaseUrl);
            request.AddParameter("Authorization", string.Format("{0} {1}", _accessToken.Type, _accessToken.AccessToken),
                ParameterType.HttpHeader);
            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info";
                var prozException = new ApplicationException(message, response.ErrorException);
                throw prozException;
            }

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

        public static ProzAccessToken Authenticate()
        {
            var client = new RestClient
            {
                BaseUrl = new Uri("https://www.proz.com"),
                Authenticator = new HttpBasicAuthenticator(ClientId, ClientSecret)
            };

            var request = new RestRequest {Method = Method.POST};
            request.AddParameter("grant_type", "client_credentials");
            request.Resource = "oauth/token";
            var response = client.Execute(request);
            var prozAccessToken = JsonConvert.DeserializeObject<ProzAccessToken>(response.Content);

            return prozAccessToken;

        }

    }
}
