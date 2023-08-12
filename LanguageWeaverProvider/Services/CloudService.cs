using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Interface;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageWeaverProvider.NewFolder
{
	public class CloudService : ITranslationService
	{
		private static HttpClient _httpClient;

		private const string ClientSecretsEndpoint = "https://api.languageweaver.com/v4/token";
		private const string UserCredentialsEndpoint = "https://api.languageweaver.com/v4/token/user";
		private const string AccountInfoEndpoint = "https://api.languageweaver.com/v4/accounts/api-credentials/self";
		private const string TranslationEndpoint = "https://api.languageweaver.com/v4/mt/translations/async";
		private const string MediaType = "application/json";

		public async Task<bool> AuthenticateUser(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
		{
			_httpClient = new();
			var endpoint = authenticationType switch
			{
				AuthenticationType.CloudCredentials => UserCredentialsEndpoint,
				AuthenticationType.CloudSecret => ClientSecretsEndpoint
			};

			var content = GetAuthenticationContent(cloudCredentials, authenticationType);
			var stringContent = new StringContent(content, null, MediaType);

			try
			{
				var response = await _httpClient.PostAsync(endpoint, stringContent);
				response.EnsureSuccessStatusCode();

				var accessToken = await response.Content.ReadAsStringAsync();
				cloudCredentials.AccessToken = JsonConvert.DeserializeObject<AccessToken>(accessToken);
				cloudCredentials.AccountId = await GetUserInfo(cloudCredentials);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private string GetAuthenticationContent(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
		{
			var isCredentialsSelected = authenticationType == AuthenticationType.CloudCredentials;
			var idKey = isCredentialsSelected ? "username" : "clientId";
			var idValue = isCredentialsSelected ? cloudCredentials.UserID : cloudCredentials.ClientID;
			var secretKey = isCredentialsSelected ? "password" : "clientSecret";
			var secretValue = isCredentialsSelected ? cloudCredentials.UserPassword : cloudCredentials.ClientSecret;

			return $@"
{{
    ""{idKey}"": ""{idValue}"",
    ""{secretKey}"": ""{secretValue}""
}}";
		}

		private async Task<string> GetUserInfo(CloudCredentials cloudCredentials)
		{
			_httpClient = new();
			var uri = new Uri(AccountInfoEndpoint);
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add("Authorization", $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");

			var response = await _httpClient.SendAsync(request);
			var userDetailsJson = await response.Content.ReadAsStringAsync();
			var accountId = JObject.Parse(userDetailsJson)["accountId"].ToString();

			return accountId;
		}

		public async Task<string> GetSupportedLanguages(CloudCredentials cloudCredentials)
		{
			_httpClient = new();
			var uri = new Uri($"https://api.languageweaver.com/v4/accounts/{cloudCredentials.AccountId}/subscriptions/language-pairs?includeChained=true");
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add("Authorization", $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");
			var response = await _httpClient.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<Xliff> Translate(CloudCredentials cloudCredentials, Xliff sourceXliff)
		{
			_httpClient = new();
			try
			{
				var translationRequestResponse = await SendTranslationRequest(cloudCredentials, sourceXliff);
				var translationRequest = JsonConvert.DeserializeObject<TranslationRequest>(translationRequestResponse);

				var translationStatusReponse = await GetTranslationStatus(cloudCredentials, translationRequest.RequestId);
				var translationStatus = JsonConvert.DeserializeObject<TranslationStatus>(translationStatusReponse);
				while (translationStatus.Status != "DONE")
				{
					if (translationStatus.Status == "FAILED")
					{
						return null;
					}

					Thread.Sleep(500);
					translationStatusReponse = await GetTranslationStatus(cloudCredentials, translationRequest.RequestId);
					translationStatus = JsonConvert.DeserializeObject<TranslationStatus>(translationStatusReponse);
				}

				var translationResponse = await GetTranslation(cloudCredentials, translationRequest.RequestId);
				var translation = JsonConvert.DeserializeObject<TranslationResponse>(translationResponse);

				var translatedSegment = translation.Translation.First();
				var translatedXliffSegment = Converter.ParseXliffString(translatedSegment);
				return translatedXliffSegment;
			}
			catch
			{
				return null;
			}
		}

		private async Task<string> GetTranslationStatus(CloudCredentials cloudCredentials, string requestId)
		{
			_httpClient = new();
			var endpoint = $"{TranslationEndpoint}/{requestId}?includeProgressInfo=true";
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");
			var response = await _httpClient.GetStringAsync(endpoint);
			return response;
		}

		private async Task<string> SendTranslationRequest(CloudCredentials cloudCredentials, Xliff sourceXliff)
		{
			var sourceXliffText = sourceXliff.ToString();
			var jsonContent = JsonConvert.SerializeObject(new
			{
				sourceLanguageId = "rum",
				targetLanguageId = sourceXliff.File.TargetCulture.ThreeLetterISOLanguageName,
				submissionType = "text",
				model = "generic",
				inputFormat = "xliff",
				input = new[] { sourceXliffText },
				dictionaries = new object[] { }
			});

			_httpClient = new();
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");
			var response = await _httpClient.PostAsync(TranslationEndpoint, new StringContent(jsonContent, Encoding.UTF8, MediaType));
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private async Task<string> GetTranslation(CloudCredentials cloudCredentials, string requestId)
		{
			var endpoint = $"{TranslationEndpoint}/{requestId}/content";

			_httpClient = new();
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");
			using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

	}
}