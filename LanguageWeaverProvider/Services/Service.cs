using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;

namespace LanguageWeaverProvider.Services
{
	internal class Service
	{
		public static async Task<HttpResponseMessage> SendRequest(AccessToken accessToken, HttpMethod httpMethod, string requestUri, HttpContent content = null)
		{
			var request = new HttpRequestMessage(httpMethod, requestUri);
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

			if (content is not null)
			{
				request.Content = content;
			}

			var response = await new HttpClient().SendAsync(request);
			return response;
		}

		public static async void ValidateToken(ITranslationOptions translationOptions)
		{
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
	}
}