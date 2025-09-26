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

		public static bool ValidateToken(ITranslationOptions translationOptions, bool showErrors = true)
		{
			if (translationOptions.AccessToken is null) return false;

			if (
				translationOptions.AuthenticationType == AuthenticationType.CloudSSO
			 && IsTimestampExpired(translationOptions.AccessToken.ExpiresAt))
			{
				return
				CloudService
					.RefreshAuth0Token(translationOptions)
					.GetAwaiter()
					.GetResult();
			}

			if (translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud
			 && translationOptions.AuthenticationType != AuthenticationType.CloudSSO
			 && IsTimestampExpired(translationOptions.AccessToken?.ExpiresAt))
			{
				return CloudService
					.AuthenticateUser(translationOptions, translationOptions.AuthenticationType)
					.GetAwaiter()
					.GetResult();
			}

			return false;
		}

		public static async void ValidateTokenAsync(ITranslationOptions translationOptions)
		{
			if (translationOptions.AccessToken is null) return;

			if (translationOptions.AuthenticationType == AuthenticationType.CloudSSO
			 && IsTimestampExpired(translationOptions.AccessToken.ExpiresAt))
			{
				await CloudService.RefreshAuth0Token(translationOptions);
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

		private static bool IsTimestampExpired(double? unixTimeStamp)
		{
			if (!unixTimeStamp.HasValue)
			{
				return false;
			}

			var expirationTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds((double)unixTimeStamp);
			var currentTime = DateTimeOffset.UtcNow.AddHours(-1);

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