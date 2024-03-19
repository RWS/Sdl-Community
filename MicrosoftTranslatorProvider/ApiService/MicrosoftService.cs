using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LanguageMappingProvider.Model;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Model;
using Newtonsoft.Json;
using RestSharp;

namespace MicrosoftTranslatorProvider.ApiService
{
	public static class MicrosoftService
    {
		public static async Task<bool> AuthenticateUser(MicrosoftCredentials credentials)
		{
			var region = credentials.Region;
			if (!string.IsNullOrEmpty(region))
			{
				region += ".";
			}

			var uriString = $"https://{region}{Constants.MicrosoftProviderServiceUriBase}/sts/v1.0/issueToken";
			var uri = new Uri(uriString);
			try
			{
				using var client = new HttpClient();
				using var request = new HttpRequestMessage();
				request.Method = HttpMethod.Post;
				request.RequestUri = uri;
				request.Headers.TryAddWithoutValidation(Constants.OcpApimSubscriptionKeyHeader, credentials.APIKey);

				var response = await client.SendAsync(request);
				response.EnsureSuccessStatusCode();
				var tokenString = await response.Content.ReadAsStringAsync();
				var token = ReadToken(tokenString);
				credentials.AccessToken = "Bearer " + tokenString;
				return true;
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(ex);
				return false;
			}
		}

		public async static Task<List<LanguageMapping>> GetSupportedLanguages()
		{
			var httpClient = new HttpClient();
			var url = "https://api.cognitive.microsofttranslator.com/languages?api-version=3.0";

			HttpResponseMessage response = await httpClient.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			var languages = JsonConvert.DeserializeObject<LanguageResponse>(responseBody)?.Translation?.Distinct().ToList();

			return null;
		}

		private static JwtSecurityToken ReadToken(string token)
		{
			var jwtHandler = new JwtSecurityTokenHandler();
			var readableToken = jwtHandler.CanReadToken(token);
			return readableToken ? jwtHandler.ReadJwtToken(token)
								 : null;
		}
	}
}