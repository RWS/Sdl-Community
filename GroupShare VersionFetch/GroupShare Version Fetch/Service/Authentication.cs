using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;

namespace Sdl.Community.GSVersionFetch.Service
{
	public static class Authentication
	{
		public static string Token;

		public static async Task<HttpStatusCode> Login(Credentials credentials)
		{
			ApiUrl.BaseUrl = credentials?.ServiceUrl;
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
				var request = new HttpRequestMessage(HttpMethod.Post, new Uri(ApiUrl.Login()));
				var authorizationHeader = GetAuthorizationHeader(credentials);
				request.Headers.Authorization = new AuthenticationHeaderValue("Basic",authorizationHeader);

				var content = JsonConvert.SerializeObject(ApiUrl.Scopes);
				request.Content = new StringContent(content, new UTF8Encoding(), "application/json");
				var responseMessage = await httpClient.SendAsync(request);

				var response = await responseMessage.Content.ReadAsStringAsync();
				if (responseMessage.StatusCode == HttpStatusCode.OK)
				{
					Token = JsonConvert.DeserializeObject<string>(response);
				}
				return responseMessage.StatusCode;
			}
		}

		private static string GetAuthorizationHeader(Credentials credentials)
		{
			if (string.IsNullOrEmpty(credentials.Password) || string.IsNullOrEmpty(credentials.UserName)) return string.Empty;

			var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.UserName}:{credentials.Password}"));
			return base64;
		}
	}
}
