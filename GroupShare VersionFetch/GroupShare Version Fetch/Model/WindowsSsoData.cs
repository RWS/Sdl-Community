using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sdl.Community.GSVersionFetch.Model
{
    public class WindowsSsoData
    {
        private readonly string _serverUrl;
        private HttpStatusCode _httpStatusCode;
        private string _token;

        public WindowsSsoData(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public string GetToken(out HttpStatusCode httpStatusCode)
        {
            if (_token == null)
            {
                SetWindowsToken();
            }

            httpStatusCode = _httpStatusCode;

            return _token;
        }

        private void SetWindowsAuthToken(string username, string password)
        {
            var baseUrl = new Uri(_serverUrl);
            var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true })
            {
                BaseAddress = baseUrl
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "R3JvdXBTaGFyZTpHcjB1cFNoYXJl");
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("scope", ApiUrl.Scopes),
                new KeyValuePair<string, string>("grant_type", "password")
            });
            var res = client.PostAsync(@"/authentication/api/token", content).Result;
            _httpStatusCode = res.StatusCode;

            if (!res.IsSuccessStatusCode)
            {
                return;
            }

            var authInfo = JsonConvert.DeserializeAnonymousType(res.Content.ReadAsStringAsync().Result, new { access_token = "" });

            _token = authInfo?.access_token;
        }

        private void SetWindowsToken()
        {
            var userDomain = Environment.GetEnvironmentVariable("USERDOMAIN");
            var userName = Environment.GetEnvironmentVariable("USERNAME");

            SetWindowsAuthToken($"{userDomain}\\{userName}", "SDLINTERNALDOMAIN");
        }
    }
}