using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlDataProtectionSuite
{
	public class WindowsSsoData
	{
		public static string Scopes = "ManagementRestApi ProjectServerRestApi MultiTermRestApi TMServerRestApi";
		private readonly string _serverUrl;
		public HttpStatusCode StatusCode { get; private set; }

		public WindowsSsoData(string serverUrl)
		{
			_serverUrl = serverUrl;
			SetWindowsToken();
		}

		public string Token { get; private set; }

		public string UserName { get; set; }

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
				new KeyValuePair<string, string>("scope", Scopes),
				new KeyValuePair<string, string>("grant_type", "password")
			});
			var res = client.PostAsync(@"/authentication/api/token", content).Result;
			StatusCode = res.StatusCode;

			if (!res.IsSuccessStatusCode)
			{
				return;
			}

			var authInfo = JsonConvert.DeserializeAnonymousType(res.Content.ReadAsStringAsync().Result, new { access_token = "" });

			Token = authInfo?.access_token;
		}

		private void SetWindowsToken()
		{
			var userDomain = Environment.GetEnvironmentVariable("USERDOMAIN");
			var userName = Environment.GetEnvironmentVariable("USERNAME");

			UserName = $"{userDomain}\\{userName}";

			SetWindowsAuthToken(UserName, "SDLINTERNALDOMAIN");
		}
	}
}