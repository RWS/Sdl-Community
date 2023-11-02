using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace LanguageWeaverProvider.Services
{
	public static class CloudService
	{
		public static async Task<(bool Success, Exception Error)> AuthenticateSSOUser(ITranslationOptions translationOptions, Auth0Config auth0Config, Uri uri)
		{
			try
			{
				var uriParams = uri.PathAndQuery.TrimStart('/');
				var parameters = HttpUtility.ParseQueryString(uriParams);
				var param = HttpUtility.ParseQueryString(uriParams).AllKeys.ToDictionary(x => x, x => parameters[x]);
				param.Add("client_id", auth0Config.ClientId);
				param.Add("redirect_uri", auth0Config.RedirectUri);
				param.Add("code_verifier", auth0Config.CodeVerifier);
				param.Add("grant_type", "authorization_code");

				var endpoint = new Uri("https://sdl-prod.eu.auth0.com/oauth/token");

				var httpClient = new HttpClient();
				using var httpRequest = new HttpRequestMessage()
				{
					Method = HttpMethod.Post,
					RequestUri = endpoint,
					Content = new FormUrlEncodedContent(param)
				};

				var result = await httpClient.SendAsync(httpRequest);
				var content = result.Content.ReadAsStringAsync().Result;
				var ssoToken = JsonConvert.DeserializeObject<CloudAuth0Response>(content);
				var currentDate = DateTime.UtcNow;

				translationOptions.AccessToken = new AccessToken
				{
					Token = ssoToken.AccessToken,
					TokenType = ssoToken.TokenType,
					RefreshToken = ssoToken.RefreshToken,
					ExpiresAt = (long)(currentDate.AddSeconds(ssoToken.ExpiresIn) - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalMilliseconds
				};

				translationOptions.AccessToken.AccountId = await GetUserInfo(translationOptions.AccessToken, "https://api.languageweaver.com/v4/accounts/users/self", "accountId");
				translationOptions.AccessToken.AccountNickname = await GetUserInfo(translationOptions.AccessToken, "https://sdl-prod.eu.auth0.com/userinfo", "nickname");
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}


		public static async Task<(bool Success, Exception Error)> AuthenticateUser(CloudCredentials cloudCredentials, ITranslationOptions translationOptions, AuthenticationType authenticationType)
		{
			try
			{
			var endpoint = authenticationType switch
			{
				AuthenticationType.CloudCredentials => "https://api.languageweaver.com/v4/token/user",
				AuthenticationType.CloudSecret => "https://api.languageweaver.com/v4/token"
			};

			var content = GetAuthenticationContent(cloudCredentials, authenticationType);
			var stringContent = new StringContent(content, null, "application/json");

				var response = await new HttpClient().PostAsync(endpoint, stringContent);
				response.EnsureSuccessStatusCode();

				var accessTokenString = await response.Content.ReadAsStringAsync();
				translationOptions.AccessToken = JsonConvert.DeserializeObject<AccessToken>(accessTokenString);

				var selfEndpoint = authenticationType == AuthenticationType.CloudCredentials
								 ? "https://api.languageweaver.com/v4/accounts/users/self"
								 : "https://api.languageweaver.com/v4/accounts/api-credentials/self";
				cloudCredentials.AccountId = await GetUserInfo(translationOptions.AccessToken, selfEndpoint, "accountId");
				translationOptions.AccessToken.AccountId = cloudCredentials.AccountId;
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

		private static async Task<string> GetUserInfo(AccessToken accessToken, string endpoint, string property)
		{
			var uri = new Uri(endpoint);
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

			var response = await new HttpClient().SendAsync(request);
			response.EnsureSuccessStatusCode();

			var userDetailsJson = await response.Content.ReadAsStringAsync();
			var accountId = JObject.Parse(userDetailsJson)[property].ToString();

			return accountId;
		}

		public static async Task<List<PairModel>> GetSupportedLanguages(AccessToken accessToken)
		{
			try
			{
				const string UriPath = "subscriptions/language-pairs";
				const string UriParameters = "includeChained=true";
				var uri = new Uri($"https://api.languageweaver.com/v4/accounts/{accessToken.AccountId}/{UriPath}?{UriParameters}");

				var request = new HttpRequestMessage(HttpMethod.Get, uri);
				request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

				var response = await new HttpClient().SendAsync(request);
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

		public static async Task<List<PairDictionary>> GetDictionaries(AccessToken accessToken)
		{
			try
			{
				var uri = new Uri($"https://api.languageweaver.com/v4/accounts/{accessToken.AccountId}/dictionaries?source=eng&target=ger");

				var request = new HttpRequestMessage(HttpMethod.Get, uri);
				request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

				var response = await new HttpClient().SendAsync(request);
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

		public static async Task<Xliff> Translate(AccessToken accessToken, PairMapping mappedPair, Xliff sourceXliff)
		{
			try
			{
				var translationRequestResponse = await SendTranslationRequest(accessToken, mappedPair, sourceXliff);
				var translationRequest = JsonConvert.DeserializeObject<CloudTranslationRequestResponse>(translationRequestResponse);

				var translationStatusReponse = await GetTranslationStatus(accessToken, translationRequest.RequestId);
				var translationStatus = JsonConvert.DeserializeObject<CloudTranslationStatus>(translationStatusReponse);
				while (translationStatus.Status != "DONE")
				{
					if (translationStatus.Status == "FAILED")
					{
						return null;
					}

					Thread.Sleep(500);
					translationStatusReponse = await GetTranslationStatus(accessToken, translationRequest.RequestId);
					translationStatus = JsonConvert.DeserializeObject<CloudTranslationStatus>(translationStatusReponse);
				}

				var translationResponse = await GetTranslation(accessToken, translationRequest.RequestId);
				var translation = JsonConvert.DeserializeObject<CloudTranslationResponse>(translationResponse);

				var translatedSegment = translation.Translation.First();
				var translatedXliffSegment = Converter.ParseXliffString(translatedSegment);
				return translatedXliffSegment;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private static async Task<string> GetTranslationStatus(AccessToken accessToken, string requestId)
		{
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

			var endpoint = $"https://api.languageweaver.com/v4/mt/translations/async/{requestId}?includeProgressInfo=true";
			var response = await httpClient.GetStringAsync(endpoint);
			return response;
		}

		private static async Task<string> SendTranslationRequest(AccessToken accessToken, PairMapping mappedPair, Xliff sourceXliff)
		{
			const string InputFormat = "xliff";

			var sourceXliffText = sourceXliff.ToString();
			var linguisticOptionsDictionary = mappedPair.LinguisticOptions?.ToDictionary(lo => lo.Id, lo => lo.SelectedValue);
			var translationRequestModel = new CloudTranslationRequest
			{
				SourceLanguageId = mappedPair.SourceCode,
				TargetLanguageId = mappedPair.TargetCode,
				Input = new[] { sourceXliff.ToString() },
				Model = mappedPair.SelectedModel.Model,
				InputFormat = InputFormat,
				Dictionaries = new object[] { },
				LinguisticOptions = linguisticOptionsDictionary
			};

			if (!string.IsNullOrEmpty(mappedPair.SelectedDictionary.DictionaryId))
			{
				translationRequestModel.Dictionaries = new object[] { mappedPair.SelectedDictionary.DictionaryId };
			}

			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			var translationRequestModelJson = JsonConvert.SerializeObject(translationRequestModel);
			var response = await httpClient.PostAsync("https://api.languageweaver.com/v4/mt/translations/async", new StringContent(translationRequestModelJson, Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private static async Task<string> GetTranslation(AccessToken accessToken, string requestId)
		{
			var endpoint = $"https://api.languageweaver.com/v4/mt/translations/async/{requestId}/content";

			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
			var response = await httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		public static async Task CreateFeedback(AccessToken accessToken, FeedbackRequest feedbackRequest)
		{
			var uri = $"https://api.languageweaver.com/v4/accounts/{accessToken.AccountId}/feedback/translations";
			var feedbackRequestJson = JsonConvert.SerializeObject(feedbackRequest);
			var content = new StringContent(feedbackRequestJson, new UTF8Encoding(), "application/json");

			var request = new HttpRequestMessage(HttpMethod.Post, uri);
			request.Headers.Add("Authorization", $"Bearer {accessToken.Token}");
			request.Content = content;

			var httpClient = new HttpClient();
			await httpClient.SendAsync(request);
		}

		public static async Task RefreshToken(AccessToken accessToken)
		{
			try
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
				var result = await new HttpClient().SendAsync(httpRequest);
			}
			catch { }
		}
	}
}