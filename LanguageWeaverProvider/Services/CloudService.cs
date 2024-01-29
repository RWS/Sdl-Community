using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageWeaverProvider.Services
{
	public static class CloudService
	{
		public static async Task<(bool Success, Exception Error)> AuthenticateSSOUser(ITranslationOptions translationOptions, CloudAuth0Config auth0Config, Uri uri, string selectedRegion)
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
				if (!result.IsSuccessStatusCode)
				{
					var errorResponse = JsonConvert.DeserializeObject<CloudAuth0Error>(content);
					ErrorHandling.ShowDialog(null, $"{result.StatusCode} {(int)result.StatusCode}", errorResponse.ToString());
					return (false, null);
				}

				var ssoToken = JsonConvert.DeserializeObject<CloudAuth0Response>(content);
				var currentDate = DateTime.UtcNow;

				translationOptions.AccessToken = new AccessToken
				{
					Token = ssoToken.AccessToken,
					TokenType = ssoToken.TokenType,
					RefreshToken = ssoToken.RefreshToken,
					ExpiresAt = (long)(currentDate.AddSeconds(ssoToken.ExpiresIn) - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalMilliseconds,
					BaseUri = new Uri(selectedRegion)
				};

				translationOptions.AccessToken.AccountId = await GetUserInfo(translationOptions.AccessToken, $"{selectedRegion}v4/accounts/users/self", "accountId");
				translationOptions.AccessToken.AccountNickname = await GetUserInfo(translationOptions.AccessToken, "https://sdl-prod.eu.auth0.com/userinfo", "nickname");

				var success = translationOptions.AccessToken.AccountId is not null && translationOptions.AccessToken.AccountNickname is not null;
				return (success, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}

		public static async Task<(bool Success, Exception Error)> AuthenticateUser(ITranslationOptions translationOptions, AuthenticationType authenticationType)
		{
			try
			{
				var cloudCredentials = translationOptions.CloudCredentials;
				var endpoint = authenticationType switch
				{
					AuthenticationType.CloudCredentials => $"{cloudCredentials.AccountRegion}v4/token/user",
					AuthenticationType.CloudAPI => $"{cloudCredentials.AccountRegion}v4/token"
				};

				var content = GetAuthenticationContent(cloudCredentials, authenticationType);
				var stringContent = new StringContent(content, null, "application/json");

				var response = await new HttpClient().PostAsync(endpoint, stringContent);
				var accessTokenString = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode)
				{
					var codeJson = JObject.Parse(accessTokenString)["status"].ToString();
					var error = JsonConvert.DeserializeObject<CloudAccountError>(codeJson);
					ErrorHandling.ShowDialog(null, $"{response.StatusCode} {(int)response.StatusCode}", $"Code: {error.Code}\nMessage: {error.Description}");
					return (false, null);
				}

				response.EnsureSuccessStatusCode();

				translationOptions.AccessToken = JsonConvert.DeserializeObject<AccessToken>(accessTokenString);
				translationOptions.AccessToken.BaseUri = new Uri(cloudCredentials.AccountRegion);

				var selfEndpoint = authenticationType == AuthenticationType.CloudCredentials
								 ? $"{cloudCredentials.AccountRegion}v4/accounts/users/self"
								 : $"{cloudCredentials.AccountRegion}v4/accounts/api-credentials/self";
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
			var idValue = isCredentialsSelected ? cloudCredentials.UserName : cloudCredentials.ClientID;
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
			var userDetailsJson = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				var errorResponse = JsonConvert.DeserializeObject<CloudAccountErrors>(userDetailsJson).Errors.FirstOrDefault();
				ErrorHandling.ShowDialog(null, $"{response.StatusCode} {(int)response.StatusCode}", errorResponse?.Description);
				return null;
			}
			response.EnsureSuccessStatusCode();

			var accountId = JObject.Parse(userDetailsJson)[property].ToString();

			return accountId;
		}

		public static async Task<CloudAccount> GetAccountDetails(AccessToken accessToken)
		{
			var uri = new Uri($"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/subscriptions");
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			var response = await new HttpClient().SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();

			var output = JsonConvert.DeserializeObject<CloudAccount>(responseContent);
			return output;
		}

		public static async Task<CloudUsageReport> GetUsageReport(AccessToken accessToken, IEnumerable<CloudAccountSubscription> subscriptions)
		{
			var usageReport = new CloudUsageReport();
			foreach (var subscription in subscriptions.Where(sub => sub.IsActive))
			{
				var startDate = DateTime.ParseExact(subscription.StartDate, "yyyy/MM/dd", null);
				var endDate = DateTime.ParseExact(subscription.EndDate, "yyyy/MM/dd", null);

				for (var currentMonthStart = startDate; currentMonthStart < endDate; currentMonthStart = currentMonthStart.AddMonths(3))
				{
					var currentMonthEnd = currentMonthStart.AddMonths(2);
					if (currentMonthEnd > endDate)
					{
						currentMonthEnd = endDate;
					}

					var uri = $"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/reports/usage/translations";
					var request = new HttpRequestMessage(HttpMethod.Get, uri);
					request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

					var period = new CloudSubscriptionPeriod
					{
						StartDate = currentMonthStart.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
						EndDate = currentMonthEnd.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)
					};

					var feedbackRequestJson = JsonConvert.SerializeObject(period);
					var content = new StringContent(feedbackRequestJson, new UTF8Encoding(), "application/json");

					var response = await Service.SendRequest(accessToken, HttpMethod.Post, uri, content);
					var responseContent = await response.Content.ReadAsStringAsync();

					var currentPeriodUsageReports = JsonConvert.DeserializeObject<CloudUsageReports>(responseContent);

					foreach (var currentPeriodUsageReport in currentPeriodUsageReports.Reports)
					{
						UpdateUsageReport(usageReport, currentPeriodUsageReport);
					}
				}
			}

			return usageReport;
		}

		public static async Task<List<AccountCategoryFeature>> GetSubscriptionDetails(AccessToken accessToken)
		{
			var uri = new Uri($"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}");
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			var response = await new HttpClient().SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();

			var json = JObject.Parse(responseContent);
			var accountCategoryFeatures = json["accountSetting"]["accountCategoryFeatures"].ToObject<List<AccountCategoryFeature>>();
			return accountCategoryFeatures;
		}

		private static void UpdateUsageReport(CloudUsageReport totalUsageReport, CloudUsageReport currentPeriodReport)
		{
			totalUsageReport.OutputWordCount += currentPeriodReport.OutputWordCount;
			totalUsageReport.OutputCharCount += currentPeriodReport.OutputCharCount;
			totalUsageReport.Count += currentPeriodReport.Count;
			totalUsageReport.InputWordCount += currentPeriodReport.InputWordCount;
			totalUsageReport.InputCharCount += currentPeriodReport.InputCharCount;
			// Add more properties if needed
		}

		public static async Task<List<PairModel>> GetSupportedLanguages(AccessToken accessToken)
		{
			try
			{
				const string UriPath = "subscriptions/language-pairs";
				const string UriParameters = "includeChained=true";
				var uri = new Uri($"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/{UriPath}?{UriParameters}");

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
				ex.ShowDialog("Language Models", ex.Message, true);
				throw ex;
			}
		}

		public static async Task<List<PairDictionary>> GetDictionaries(AccessToken accessToken)
		{
			try
			{
				var uri = new Uri($"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/dictionaries");

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

			var endpoint = $"{accessToken.BaseUri}v4/mt/translations/async/{requestId}?includeProgressInfo=true";
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

			var dictionaries = mappedPair.Dictionaries.Where(x => x.IsSelected);
			if (dictionaries.Any())
			{
				translationRequestModel.Dictionaries = new object[dictionaries.Count()];
				for (var i = 0; i < translationRequestModel.Dictionaries.Length; i++)
				{
					translationRequestModel.Dictionaries[i] = dictionaries.ElementAt(i).DictionaryId;
				}
			}

			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			var translationRequestModelJson = JsonConvert.SerializeObject(translationRequestModel);
			var response = await httpClient.PostAsync($"{accessToken.BaseUri}v4/mt/translations/async", new StringContent(translationRequestModelJson, Encoding.UTF8, "application/json"));
			var responseContent = await response.Content.ReadAsStringAsync();
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private static async Task<string> GetTranslation(AccessToken accessToken, string requestId)
		{
			var endpoint = $"{accessToken.BaseUri}v4/mt/translations/async/{requestId}/content";

			var response = await Service.SendRequest(accessToken, HttpMethod.Get, endpoint);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		public static async Task<bool> CreateFeedback(AccessToken accessToken, FeedbackRequest feedbackRequest)
		{
			try
			{
				var requestUri = $"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/feedback/translations";
				var feedbackRequestJson = JsonConvert.SerializeObject(feedbackRequest);
				var content = new StringContent(feedbackRequestJson, new UTF8Encoding(), "application/json");

				var response = await Service.SendRequest(accessToken, HttpMethod.Post, requestUri, content);
				response.EnsureSuccessStatusCode();

				return true;
			}
			catch (Exception ex)
			{
				ErrorHandling.ShowDialog(ex, "Feedback", ex.Message, true);
				return false;
			}
		}

		public static async Task<bool> CreateDictionaryTerm(AccessToken accessToken, PairDictionary pairDictionary, DictionaryTerm newDictionaryTerm)
		{

			var requestUri = $"https://api.languageweaver.com/v4/accounts/{accessToken.AccountId}/dictionaries/{pairDictionary.DictionaryId}/terms";
			var content = JsonConvert.SerializeObject(newDictionaryTerm);
			var stringContent = new StringContent(content, new UTF8Encoding(), "application/json");

			var response = await Service.SendRequest(accessToken, HttpMethod.Post, requestUri, stringContent);
			var isSuccessStatusCode = response.IsSuccessStatusCode;
			if (isSuccessStatusCode)
			{
				return isSuccessStatusCode;
			}

			var result = await response.Content.ReadAsStringAsync();
			ErrorHandling.ShowDialog(null, PluginResources.Dictionary_NewTerm_Unsuccessfully, result);
			return isSuccessStatusCode;
		}
	}
}