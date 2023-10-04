using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LanguageWeaverProvider.Services
{
	public static class CloudService
	{
		private static HttpClient _httpClient;

		private const string HeaderNameAuth = "Authorization";
		private const string ClientSecretsEndpoint = "https://api.languageweaver.com/v4/token";
		private const string UserCredentialsEndpoint = "https://api.languageweaver.com/v4/token/user";
		private const string AccountInfoApiEndpoint = "https://api.languageweaver.com/v4/accounts/api-credentials/self";
		private const string AccountInfoCredentialsEndpoint = "https://api.languageweaver.com/v4/accounts/users/self";
		private const string TranslationEndpoint = "https://api.languageweaver.com/v4/mt/translations/async";

		public static async Task<(bool Success, Exception Error)> AuthenticateUser(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
		{
			_httpClient = new();
			var endpoint = authenticationType switch
			{
				AuthenticationType.CloudCredentials => UserCredentialsEndpoint,
				AuthenticationType.CloudSecret => ClientSecretsEndpoint
			};

			var content = GetAuthenticationContent(cloudCredentials, authenticationType);
			var stringContent = new StringContent(content, null, "application/json");

			try
			{
				var response = await _httpClient.PostAsync(endpoint, stringContent);
				response.EnsureSuccessStatusCode();

				var accessToken = await response.Content.ReadAsStringAsync();
				cloudCredentials.AccessToken = JsonConvert.DeserializeObject<AccessToken>(accessToken);

				var selfEndpoint = authenticationType == AuthenticationType.CloudCredentials ? AccountInfoCredentialsEndpoint : AccountInfoApiEndpoint;
				cloudCredentials.AccountId = await GetUserInfo(cloudCredentials, selfEndpoint);
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}

		private static string GetAuthenticationContent(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
		{
			var isCredentialsSelected = authenticationType == AuthenticationType.CloudCredentials;
			var idKey = isCredentialsSelected ? "username" : "clientId";
			var idValue = isCredentialsSelected ? cloudCredentials.UserID : cloudCredentials.ClientID;
			var secretKey = isCredentialsSelected ? "password" : "clientSecret";
			var secretValue = isCredentialsSelected ? cloudCredentials.UserPassword : cloudCredentials.ClientSecret;

			return $"\r\n{{\r\n    \"{idKey}\": \"{idValue}\",\r\n    \"{secretKey}\": \"{secretValue}\"\r\n}}";
		}

		private static async Task<string> GetUserInfo(CloudCredentials cloudCredentials, string endpoint)
		{
			_httpClient = new();
			var uri = new Uri(endpoint);
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add(HeaderNameAuth, $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var userDetailsJson = await response.Content.ReadAsStringAsync();
			var accountId = JObject.Parse(userDetailsJson)["accountId"].ToString();

			return accountId;
		}

		public static async Task<List<PairModel>> GetSupportedLanguages(CloudCredentials cloudCredentials)
		{
			await EnsureConnection(cloudCredentials);
			_httpClient = new();
			
			try
			{
				const string UriPath = "subscriptions/language-pairs";
				const string UriParameters = "includeChained=true";
				var uri = new Uri($"https://api.languageweaver.com/v4/accounts/{cloudCredentials.AccountId}/{UriPath}?{UriParameters}");

				var request = new HttpRequestMessage(HttpMethod.Get, uri);
				request.Headers.Add(HeaderNameAuth, $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");

				var response = await _httpClient.SendAsync(request);
				response.EnsureSuccessStatusCode();

				var responseContent = await response.Content.ReadAsStringAsync();
				var languagePairs = JObject.Parse(responseContent)["languagePairs"].ToString();
				var languagePairsOutput = JsonConvert.DeserializeObject<List<PairModel>>(languagePairs);

				return languagePairsOutput;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static async Task<List<PairDictionary>> GetDictionaries(CloudCredentials cloudCredentials)
		{
			await EnsureConnection(cloudCredentials);
				_httpClient = new();

			try
			{

				var uri = new Uri($"https://api.languageweaver.com/v4/accounts/{cloudCredentials.AccountId}/dictionaries");

				var request = new HttpRequestMessage(HttpMethod.Get, uri);
				request.Headers.Add(HeaderNameAuth, $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");

				var response = await _httpClient.SendAsync(request);
				response.EnsureSuccessStatusCode();

				var responseContent = await response.Content.ReadAsStringAsync();
				var dictionariesJson = JObject.Parse(responseContent)["dictionaries"].ToString();
				var dictionariesOutput = JsonConvert.DeserializeObject<List<PairDictionary>>(dictionariesJson);

				return dictionariesOutput;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static async Task<Xliff> Translate(CloudCredentials cloudCredentials, PairMapping mappedPair, Xliff sourceXliff)
		{
			await EnsureConnection(cloudCredentials);
			_httpClient = new();

			try
			{
				var translationRequestResponse = await SendTranslationRequest(cloudCredentials, mappedPair, sourceXliff);
				var translationRequest = JsonConvert.DeserializeObject<TranslationRequestResponse>(translationRequestResponse);

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
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private static async Task<string> GetTranslationStatus(CloudCredentials cloudCredentials, string requestId)
		{
			const string UriParameters = "includeProgressInfo=true";

			_httpClient = new();
			_httpClient.DefaultRequestHeaders.Add(HeaderNameAuth, $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");

			var endpoint = $"{TranslationEndpoint}/{requestId}?{UriParameters}";
			var response = await _httpClient.GetStringAsync(endpoint);
			return response;
		}

		private static async Task<string> SendTranslationRequest(CloudCredentials cloudCredentials, PairMapping mappedPair, Xliff sourceXliff)
		{
			const string InputFormat = "xliff";
			const string QualityEstimationAbbreviation = "qe";

			var sourceXliffText = sourceXliff.ToString();
			var linguisticOptionsDictionary = mappedPair.LinguisticOptions?.ToDictionary(lo => lo.Id, lo => lo.SelectedValue);

			var translationRequestModel = new TranslationRequest
			{
				SourceLanguageId = mappedPair.SourceCode,
				TargetLanguageId = mappedPair.TargetCode,
				Input = new[] { sourceXliff.ToString() },
				Model = mappedPair.SelectedModel.Model,
				InputFormat = InputFormat,
				Dictionaries = new object[] { },
				QualityEstimation = mappedPair.SelectedModel.Model.ToLower().Contains(QualityEstimationAbbreviation) ? 1 : 0,
				LinguisticOptions = linguisticOptionsDictionary
			};

			_httpClient = new();
			_httpClient.DefaultRequestHeaders.Add(HeaderNameAuth, $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");
			var translationRequestModelJson = JsonConvert.SerializeObject(translationRequestModel);
			var response = await _httpClient.PostAsync(TranslationEndpoint, new StringContent(translationRequestModelJson, Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private static async Task<string> GetTranslation(CloudCredentials cloudCredentials, string requestId)
		{
			var endpoint = $"{TranslationEndpoint}/{requestId}/content";

			_httpClient = new();
			_httpClient.DefaultRequestHeaders.Add(HeaderNameAuth, $"{cloudCredentials.AccessToken.TokenType} {cloudCredentials.AccessToken.Token}");
			using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private static async Task EnsureConnection(CloudCredentials cloudCredentials)
		{
			var authenticationType = string.IsNullOrEmpty(cloudCredentials.UserID) ? AuthenticationType.CloudSecret : AuthenticationType.CloudCredentials;

			try
			{
				var selfEndpoint = authenticationType == AuthenticationType.CloudCredentials ? AccountInfoCredentialsEndpoint : AccountInfoApiEndpoint;
				await GetUserInfo(cloudCredentials, selfEndpoint);
			}
			catch (Exception ex)
			{
				if (!ex.Message.Contains("401"))
				{
					return;
				}

				await AuthenticateUser(cloudCredentials, authenticationType);
			}
		}

		public static async Task CreateFeedback(CloudCredentials cloudCredentials, FeedbackRequest feedbackRequest)
		{
			var uri = $"https://api.languageweaver.com/v4/accounts/{cloudCredentials.AccountId}/feedback/translations";
			var feedbackRequestJson = JsonConvert.SerializeObject(feedbackRequest);
			var content = new StringContent(feedbackRequestJson, new UTF8Encoding(), "application/json");

			var request = new HttpRequestMessage(HttpMethod.Post, uri);
			request.Headers.Add("Authorization", $"Bearer {cloudCredentials.AccessToken.Token}");
			request.Content = content;

			_httpClient = new();
			await _httpClient.SendAsync(request);
		}
	}
}