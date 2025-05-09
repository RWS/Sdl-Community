using Newtonsoft.Json.Linq;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sdl.Community.GSVersionFetch.Service
{
    public static class Authentication
    {
        public static string Token;

        public static async Task<HttpStatusCode> Login(Credentials credentials)
        {
            ApiUrl.BaseUrl = credentials?.ServiceUrl;
            try
            {
                var content =
                    $"username&passwd&scope={ApiUrl.Scopes}&grant_type=password";

                content =
                    Uri.EscapeUriString(content)
                        .Replace("username",
                            $"username={Uri.EscapeDataString(credentials?.UserName)}")
                        .Replace(
                            "passwd",
                            $"password={Uri.EscapeDataString(credentials?.Password)}");

                var request = GetHttpRequestMessage(ApiUrl.Login, HttpMethod.Post, content);
                var responseMessage = await AppInitializer.Client.SendAsync(request);

                if (responseMessage.StatusCode == HttpStatusCode.Redirect)
                {
                    SwitchToHttps();
                    request = GetHttpRequestMessage(ApiUrl.Login, HttpMethod.Post, content);
                    responseMessage =
                        await AppInitializer.Client.SendAsync(request);
                }

                var response = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    Token = JObject.Parse(response)["access_token"].ToString();
                }

                return responseMessage.StatusCode;
            }
            catch (Exception e)
            {
                if (e.InnerException != null &&
                    e.InnerException.Message.Contains(PluginResources.RemoteNameCouldNotBeSolved))
                {
                    var badRequest = HttpStatusCode.BadRequest;
                    return badRequest;
                }

                return HttpStatusCode.InternalServerError;
            }
        }

        public static async Task<HttpStatusCode> SetCredentials(Credentials credentials)
        {
            if (credentials == null)
                return HttpStatusCode.Unauthorized;

            var match = Regex.Match(credentials.ServiceUrl, @":\d+");

            ApiUrl.BaseUrl = match.Success ? credentials.ServiceUrl.Substring(0, match.Index) : credentials.ServiceUrl;
            var credentialType = credentials.CredentialType;

            var statusCode = new HttpStatusCode();
            Token = credentialType == CredentialType.SSO ? credentials.SsoCredentials?.AuthToken : credentials.WindowsSsoCredentials?.GetToken(out statusCode);

            if (statusCode == 0)
            {
                statusCode = await TestPermissions();
            }

            return statusCode;
        }

        public static async Task<HttpStatusCode> TestPermissions()
        {
            var projectFilter = new ProjectFilter
            {
                Page = 1,
                PageSize = 1
            };
            var request = GetHttpRequestMessage(ApiUrl.GetProjectsUri(projectFilter));

            var responseMessage = await AppInitializer.Client.SendAsync(request);

            if (responseMessage.StatusCode == HttpStatusCode.Redirect)
            {
                SwitchToHttps();
                request = GetHttpRequestMessage(ApiUrl.GetProjectsUri(projectFilter));

                responseMessage =
                    await AppInitializer.Client.SendAsync(request);
            }

            return responseMessage.StatusCode;
        }

        private static HttpRequestMessage GetHttpRequestMessage(string uri, HttpMethod httpMethod = null, string content = null, [CallerMemberName] string callerName = null)
        {
            httpMethod = httpMethod ?? HttpMethod.Get;
            var requestMessage = new HttpRequestMessage(httpMethod, uri);

            var addToken = !(callerName != null && callerName.Contains("Login"));
            ApiUrl.AddRequestHeaders(requestMessage, content, addToken);

            return requestMessage;
        }

        private static void SwitchToHttps()
        {
            ApiUrl.BaseUrl = ApiUrl.BaseUrl?.Replace("http", "https");
        }
    }
}