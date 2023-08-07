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

namespace LanguageWeaverProvider.NewFolder
{
	public class CloudService : ITranslationService
	{
		private static HttpClient _httpClient;

		private const string TranslationEndpoint = "https://api.languageweaver.com/v4/mt/translations/async";
		private const string UserCredentialsEndpoint = "https://api.languageweaver.com/v4/token/user";
		private const string ClientSecretsEndpoint = "https://api.languageweaver.com/v4/token";
		private const string MediaType = "application/json";

		public async Task<bool> AuthenticateUser(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
		{
			_httpClient = new();
			var endpoint = authenticationType switch
			{
				AuthenticationType.CloudCredentials => UserCredentialsEndpoint,
				AuthenticationType.CloudSecret => ClientSecretsEndpoint,
				_ => throw new ArgumentException("Invalid authentication type")
			};

			var content = GetAuthenticationContent(cloudCredentials, authenticationType);
			var stringContent = new StringContent(content, null, MediaType);

			try
			{
				var response = await _httpClient.PostAsync(endpoint, stringContent);
				response.EnsureSuccessStatusCode();

				var accessToken = await response.Content.ReadAsStringAsync();
				cloudCredentials.AccessToken = JsonConvert.DeserializeObject<AccessToken>(accessToken);
				return true;
			}
			catch
			{
				return false;
			}
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

			var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private async Task<string> SendTranslationRequest(CloudCredentials cloudCredentials, Xliff sourceXliff)
		{
			_httpClient = new();
			var sourceXliffText = sourceXliff.ToString();
			var translationData = new
			{
				sourceLanguageId = sourceXliff.File.SourceCulture.ThreeLetterISOLanguageName,
				targetLanguageId = sourceXliff.File.TargetCulture.ThreeLetterISOLanguageName,
				submissionType = "text",
				model = "generic",
				inputFormat = "xliff",
				input = new[] { sourceXliffText },
				dictionaries = new object[] { }
			};

			var jsonContent = JsonConvert.SerializeObject(translationData);
			var stringContent = new StringContent(jsonContent, Encoding.UTF8, MediaType);

			_httpClient.DefaultRequestHeaders.Add("Authorization", $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");

			var response = await _httpClient.PostAsync(TranslationEndpoint, stringContent);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private async Task<string> GetTranslation(CloudCredentials cloudCredentials, string requestId)
		{
			_httpClient = new();
			var endpoint = $"{TranslationEndpoint}/{requestId}/content";

			_httpClient.DefaultRequestHeaders.Add("Authorization", $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");

			using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
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
	}
}