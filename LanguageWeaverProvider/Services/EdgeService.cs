using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services
{
	public static class EdgeService
	{
		public static async Task<(bool Success, Exception Error)> AuthenticateUser(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
		{
			try
			{
				var userName = Uri.EscapeDataString(edgeCredentials.UserName);
				var password = Uri.EscapeDataString(edgeCredentials.Password);
				var uri = new UriBuilder(edgeCredentials.Uri)
				{
					Path = "/api/v2/auth",
					Query = $"username={userName}&password={password}"
				};

				var x = $"{edgeCredentials.Uri}api/v2/auth$username={userName}&password={password}";

				var response = await new HttpClient().PostAsync(uri.Uri, null);
				response.EnsureSuccessStatusCode();

				var token = await response.Content.ReadAsStringAsync();
				SetAccessToken(translationOptions, token, "Bearer", edgeCredentials.Uri);

				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}

		public static async Task<(bool Success, Exception Error)> VerifyAPI(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
		{
			try
			{
				var encodedApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{edgeCredentials.ApiKey}:"));
				SetAccessToken(translationOptions, encodedApiKey, "Basic", edgeCredentials.Uri);

				var temporaryAccessToken = translationOptions.AccessToken;
				var requestUri = $"{temporaryAccessToken.BaseUri}api/v2/language-pairs";

				var response = await Service.SendRequest(temporaryAccessToken, HttpMethod.Get, requestUri);
				response.EnsureSuccessStatusCode();

				return (true, null);
			}
			catch (Exception ex)
			{
				translationOptions.AccessToken = null;
				return (false, ex);
			}
		}

		public static async Task<(bool Success, Exception Error)> SignInAuthAsync(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions, WebView2 browser)
		{
			try
			{
				var baseUrl = edgeCredentials.Uri;
				var uri2 = $"{baseUrl}/api/v2/auth/saml/init";
				var uri3 = $"{baseUrl}/api/v2/auth/saml/wait";

				var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri($"{baseUrl}/api/v2/auth/saml/gen-secret"));
				using var httpClient = new HttpClient();
				var response = await httpClient.SendAsync(httpRequest);

				var secret = string.Empty;
				if (response.IsSuccessStatusCode)
				{
					secret = await response.Content.ReadAsStringAsync();
				}

				var url = $"{uri2}?secret={secret}";
				browser.CoreWebView2.Navigate(url);

				httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{uri3}?secret={secret}");
				response = await httpClient.SendAsync(httpRequest);

				string token;
				if (response.IsSuccessStatusCode)
				{
					token = await response.Content.ReadAsStringAsync();
					SetAccessToken(translationOptions, token, "Bearer", edgeCredentials.Uri);
				}

				response.EnsureSuccessStatusCode();
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}

		private static void SetAccessToken(ITranslationOptions translationOptions, string token, string tokenType, Uri edgeUri)
			=> translationOptions.AccessToken = new() { Token = token, TokenType = tokenType, BaseUri = edgeUri };

		public static async Task<List<PairModel>> GetLanguagePairs(AccessToken accessToken)
		{
			try
			{
				var requestUri = $"{accessToken.BaseUri}api/v2/language-pairs";

				var response = await Service.SendRequest(accessToken, HttpMethod.Get, requestUri);
				var languagePairs = await DeserializeResponse<EdgeLanguagePairResult>(response);

				var pairModels = languagePairs.LanguagePairs.Select(lp => new PairModel()
				{
					DisplayName = $"{lp.Model} {lp.Version}",
					SourceLanguageId = lp.SourceLanguageId,
					TargetLanguageId = lp.TargetLanguageId,
					LinguisticOptions = lp.LinguisticOptions,
					Name = lp.LanguagePairId,
					Model = lp.LanguagePairId
				}).ToList();

				foreach (var linguisticOption in pairModels.Where(pair => pair.LinguisticOptions is not null).SelectMany(pair => pair.LinguisticOptions))
				{
					linguisticOption.Name = linguisticOption.Id;
				}

				return pairModels;
			}
			catch (Exception ex)
			{
				ErrorHandling.ShowDialog(ex, "Language pairs", ex.Message, true);
				return null;
			}
		}

		public static async Task<List<PairDictionary>> GetDictionaries(AccessToken accessToken)
		{
			try
			{
				var requestUri = $"{accessToken.BaseUri}api/v2/dictionaries";
				var response = await Service.SendRequest(accessToken, HttpMethod.Get, requestUri);
				var content = await response.Content.ReadAsStringAsync();
				var dictionaries = JsonConvert.DeserializeObject<EdgeDictionariesResponse>(content);

				var pairDictionaries = dictionaries.Dictionaries.Select(dictionary => new PairDictionary()
				{
					Name = dictionary.DictionaryId,
					DictionaryId = dictionary.DictionaryId,
					Source = dictionary.SourceLanguageId,
					Target = dictionary.TargetLanguageId
				}).ToList();

				return pairDictionaries;
			}
			catch (Exception ex)
			{
				ErrorHandling.ShowDialog(ex, "Dictionaries", ex.Message, true);
				return null;
			}
		}

		public static async Task<bool> CreateDictionaryTerm(AccessToken accessToken, PairDictionary pairDictionary, DictionaryTerm newDictionaryTerm)
		{
			var requestUri = $"{accessToken.BaseUri}api/v2/dictionaries/{pairDictionary.DictionaryId}/term";
			var content = newDictionaryTerm.ToKeyValuePairDictionary();
			var encodedContent = new FormUrlEncodedContent(content);

			var response = await Service.SendRequest(accessToken, HttpMethod.Post, requestUri, encodedContent);
			var isSuccessStatusCode = response.IsSuccessStatusCode;
			if (isSuccessStatusCode)
			{
				return isSuccessStatusCode;
			}

			var result = await response.Content.ReadAsStringAsync();
			ErrorHandling.ShowDialog(null, PluginResources.Dictionary_NewTerm_Unsuccessfully, result);
			return isSuccessStatusCode;
		}

		public static async Task<Xliff> Translate(AccessToken accessToken, PairMapping pairMapping, Xliff sourceXliff)
		{
			try
			{
				var translationRequest = await SendTranslationRequest(accessToken, pairMapping, sourceXliff);
				var translationStatus = await GetTranslationStatus(accessToken, translationRequest.TranslationId);
				if (translationStatus.Error is not null)
				{
					ErrorHandling.ShowDialog(null, translationStatus.Error.Code, translationStatus.Error.Message);
					return null;
				}

				await WaitForTranslationCompletion(accessToken, translationRequest.TranslationId);
				var translationResponse = await GetTranslation(accessToken, translationRequest.TranslationId);
				var translatedXliff = Converter.ParseXliffString(Base64Decode(translationResponse));

				return translatedXliff;
			}
			catch
			{
				return null;
			}
		}

		private static async Task<EdgeTranslationRequestResponse> SendTranslationRequest(AccessToken accessToken, PairMapping pairMapping, Xliff sourceXliff)
		{
			var requestUri = $"{accessToken.BaseUri}api/v2/translations";
			var query = BuildQuery(pairMapping, sourceXliff);
			var content = new FormUrlEncodedContent(query);

			var response = await Service.SendRequest(accessToken, HttpMethod.Post, requestUri, content);
			var translationRequestResponse = await DeserializeResponse<EdgeTranslationRequestResponse>(response);

			return translationRequestResponse;
		}

		private static async Task<string> GetTranslation(AccessToken accessToken, string translationId)
		{
			var requestUri = $"{accessToken.BaseUri}api/v2/translations/{translationId}/download";

			var response = await Service.SendRequest(accessToken, HttpMethod.Get, requestUri);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		private static async Task<EdgeTranslationStatus> GetTranslationStatus(AccessToken accessToken, string requestId)
		{
			var requestUri = $"{accessToken.BaseUri}api/v2/translations/{requestId}";

			var response = await Service.SendRequest(accessToken, HttpMethod.Get, requestUri);
			response.EnsureSuccessStatusCode();

			var translationStatus = await DeserializeResponse<EdgeTranslationStatus>(response);
			return translationStatus;
		}

		private static async Task WaitForTranslationCompletion(AccessToken accessToken, string translationId)
		{
			EdgeTranslationStatus translationStatus;
			bool isWaiting;
			do
			{
				translationStatus = await GetTranslationStatus(accessToken, translationId);
				isWaiting = translationStatus.State != "done";
				if (isWaiting)
				{
					await Task.Delay(1000);
				}
			} while (isWaiting);
		}

		public static async Task<bool> SendFeedback(AccessToken accessToken, List<KeyValuePair<string, string>> feedback)
		{
			try
			{
				var requestUri = $"{accessToken.BaseUri}api/v2/feedback";
				var content = new FormUrlEncodedContent(feedback);

				var response = await Service.SendRequest(accessToken, HttpMethod.Post, requestUri, content);
				if (!response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					var error = JsonConvert.DeserializeObject<EdgeFeedbackError>(responseContent);
					ErrorHandling.ShowDialog(null, $"Code {error.Error.Code}: {error.Error.Message}", error.Error.Details);
				}

				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				ErrorHandling.ShowDialog(ex, "Feedback", ex.Message, true);
				return false;
			}
		}

		private static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
		{
			var content = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(content);
		}

		private static Dictionary<string, string> BuildQuery(PairMapping pairMapping, Xliff sourceXliff)
		{
			const string LanguagePairIdKey = "languagePairId";
			const string InputKey = "input";
			const string InputFormatKey = "inputFormat";
			const string XliffMimeType = "application/x-xliff";
			const string DictionaryIdsKey = "dictionaryIds";
			const string LinguisticOptionsKey = "linguisticOptions";

			var input = Base64Encode(sourceXliff.ToString());
			var queryString = new Dictionary<string, string>
			{
				[LanguagePairIdKey] = pairMapping.SelectedModel.Model,
				[InputKey] = input,
				[InputFormatKey] = XliffMimeType
			};

			var dictionaries = pairMapping.Dictionaries.Where(x => x.IsSelected);
			if (dictionaries.Any())
			{
				queryString[DictionaryIdsKey] = string.Empty;
				foreach (var dictionary in dictionaries)
				{
					queryString[DictionaryIdsKey] += $"{dictionary.DictionaryId},";
				}

				queryString[DictionaryIdsKey] = queryString[DictionaryIdsKey].Substring(0, queryString[DictionaryIdsKey].Length - 1);
			}

			if (pairMapping.LinguisticOptions is not null)
			{
				queryString[LinguisticOptionsKey] = string.Empty;
				var linguisticOptionsDictionary = pairMapping.LinguisticOptions.ToDictionary(lo => lo.Id, lo => lo.SelectedValue);
				foreach (var linguisticOption in linguisticOptionsDictionary)
				{
					queryString[LinguisticOptionsKey] += $"{linguisticOption.Key}:{linguisticOption.Value},";
				}

				queryString[LinguisticOptionsKey] = queryString[LinguisticOptionsKey].Substring(0, queryString[LinguisticOptionsKey].Length - 1);
			}

			return queryString;
		}

		private static string Base64Encode(this string text)
			=> Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

		private static string Base64Decode(this string encodedText)
			=> Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
	}
}