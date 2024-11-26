using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageWeaverProvider.Services
{
	internal static class Service
	{
		public static async Task<HttpResponseMessage> SendRequest(HttpMethod httpMethod, string requestUri, AccessToken accessToken = null, HttpContent content = null)
		{
			var request = new HttpRequestMessage(httpMethod, requestUri) { Content = content };
			if (accessToken is not null)
			{
				request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			}

			var httpClient = GetHttpClient();
			var response = await httpClient.SendAsync(request);
			return response;
		}

		public static async Task<T> DeserializeResponse<T>(this HttpResponseMessage httpResponseMessage, string property = null)
		{
			var content = await httpResponseMessage.Content.ReadAsStringAsync();
			if (!string.IsNullOrEmpty(property))
			{
				content = JObject.Parse(content)[property].ToString();
			}

			var deserializedObject = JsonConvert.DeserializeObject<T>(content);
			return deserializedObject;
		}

		public static async void ValidateToken(ITranslationOptions translationOptions)
		{
			if (translationOptions.AccessToken is null) return;

			if (translationOptions.AuthenticationType == AuthenticationType.CloudSSO
			 && IsTimestampExpired(translationOptions.AccessToken.ExpiresAt))
			{
				await RefreshAuth0Token(translationOptions.AccessToken);
				return;
			}

			if (translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud
			 && translationOptions.AuthenticationType != AuthenticationType.CloudSSO
			 && IsTimestampExpired(translationOptions.AccessToken?.ExpiresAt))
			{
				await CloudService.AuthenticateUser(translationOptions, translationOptions.AuthenticationType);
				return;
			}
		}

		public static async Task RefreshAuth0Token(AccessToken accessToken)
		{
			var parameters = new Dictionary<string, string>
			{
				{ "grant_type", "refresh_token" },
				{ "refresh_token", accessToken.RefreshToken }
			};

			using var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://sdl-prod.eu.auth0.com/"),
				Content = new FormUrlEncodedContent(parameters)
			};

			await new HttpClient().SendAsync(httpRequest);
		}

		private static bool IsTimestampExpired(double? unixTimeStamp)
		{
			if (!unixTimeStamp.HasValue)
			{
				return false;
			}

			var expirationTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds((double)unixTimeStamp);
			var currentTime = DateTimeOffset.UtcNow;

			return expirationTime <= currentTime;
		}

		public static HttpClient GetHttpClient()
		{
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add(Constants.TraceAppKey, Constants.TraceAppValue);
			httpClient.DefaultRequestHeaders.Add(Constants.TraceAppVersionKey, ApplicationInitializer.CurrentAppVersion);
			return httpClient;
		}
	}
}