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

		public static async Task<string> Login(Credentials credentials)
		{
			if (credentials == null)
			{
				return null;
			}

			ApiUrl.BaseUrl = credentials.ServiceUrl;
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
				var request = new HttpRequestMessage(HttpMethod.Post, new Uri(ApiUrl.Login()));
				var authorizationHeader = GetEuthorizationHeader(credentials);
				request.Headers.Authorization = new AuthenticationHeaderValue("Basic",authorizationHeader);

				var content = JsonConvert.SerializeObject(ApiUrl.Scopes);
				request.Content = new StringContent(content, new UTF8Encoding(), "application/json");
				var responseMessage = await httpClient.SendAsync(request);

				var response = await responseMessage.Content.ReadAsStringAsync();
				if (responseMessage.StatusCode == HttpStatusCode.OK)
				{
					Token = JsonConvert.DeserializeObject<string>(response);
					return response;
				}

				// if the respsonse is anything other than 200 -> OK, then throw and exception with the response message
				throw new Exception(response);
			}
		}

		private static string GetEuthorizationHeader(Credentials credentials)
		{
			if (string.IsNullOrEmpty(credentials.Password) || string.IsNullOrEmpty(credentials.UserName)) return string.Empty;

			var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.UserName}:{credentials.Password}"));
			return base64;
		}
	}
}
