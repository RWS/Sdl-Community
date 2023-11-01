using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Services
{
	public static class EdgeService
	{
		public static async Task<(bool Success, Exception Error)> AuthenticateUser(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.DefaultConnectionLimit = 9999;
			try
			{
				using var httpClient = new HttpClient();

				var queryString = HttpUtility.ParseQueryString(string.Empty);
				queryString["username"] = edgeCredentials.UserName;
				queryString["password"] = edgeCredentials.Password;

				var builder = new UriBuilder(edgeCredentials.Uri)
				{
					Path = "/api/v2/auth",
					Query = queryString.ToString()
				};

				var httpResponse = httpClient.PostAsync(builder.Uri, null).Result;
				httpResponse.EnsureSuccessStatusCode();
				var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
				translationOptions.AccessToken = new() { Token = responseContent, TokenType = "Bearer", EdgeUri = edgeCredentials.Uri };
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}

		public static async Task<(bool Success, Exception Error)> VerifyAPI(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.DefaultConnectionLimit = 9999;
			try
			{
				using var httpClient = new HttpClient();
				var encodedApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{edgeCredentials.ApiKey}:"));
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedApiKey);

				var uriBuilder = new UriBuilder(edgeCredentials.Uri)
				{
					Path = "/api/v2/language-pairs"
				};

				var response = await httpClient.GetAsync(uriBuilder.Uri);
				response.EnsureSuccessStatusCode();
				translationOptions.AccessToken = new() { Token = encodedApiKey, TokenType = "Basic", EdgeUri = edgeCredentials.Uri };
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}

		public static async Task<(bool Success, Exception Error)> SignInAuthAsync(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
		{
			try
			{
				var baseUrl = edgeCredentials.Uri;
				var uri2 = $"{baseUrl}/api/v2/auth/saml/init";
				var uri3 = $"{baseUrl}/api/v2/auth/saml/wait";

				var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri($"{baseUrl}/api/v2/auth/saml/gen-secret"));
				using var httpClient = new HttpClient();
				var response = await httpClient.SendAsync(httpRequest);

				var secret = "";
				if (response.IsSuccessStatusCode)
				{
					secret = await response.Content.ReadAsStringAsync();
				}

				var url = $"{uri2}?secret={secret}";
				var ps = new ProcessStartInfo(url)
				{
					UseShellExecute = true,
					Verb = "open"
				};

				Process.Start(ps);

				httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{uri3}?secret={secret}");
				response = await httpClient.SendAsync(httpRequest);

				var token = string.Empty;
				if (response.IsSuccessStatusCode)
				{
					token = await response.Content.ReadAsStringAsync();
				}

				translationOptions.AccessToken = new() { Token = token, TokenType = "Bearer", EdgeUri = edgeCredentials.Uri };
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, ex);
			}
		}

		public static async Task<List<PairModel>> GetLanguagePairs(ITranslationOptions translationOptions)
		{
			try
			{
				ServicePointManager.Expect100Continue = true;
				ServicePointManager.DefaultConnectionLimit = 9999;
				var edgeCredentials = translationOptions.EdgeCredentials;
				var accessToken = translationOptions.AccessToken;

				var client = new HttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, $"{edgeCredentials.Uri}api/v2/language-pairs");
				request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

				var response = client.SendAsync(request).Result;
				response.EnsureSuccessStatusCode();

				var content = response.Content.ReadAsStringAsync().Result;
				var languagePairs = JsonConvert.DeserializeObject<EdgeLanguagePairResult>(content);

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
				return null;
			}
		}

		public static async Task<List<PairDictionary>> GetDictionaries(ITranslationOptions translationOptions)
		{
			try
			{
				ServicePointManager.Expect100Continue = true;
				ServicePointManager.DefaultConnectionLimit = 9999;
				var edgeCredentials = translationOptions.EdgeCredentials;
				var accessToken = translationOptions.AccessToken;

				var request = new HttpRequestMessage(HttpMethod.Get, $"{edgeCredentials.Uri}api/v2/dictionaries");
				request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

				var client = new HttpClient();
				var response = await client.SendAsync(request);
				response.EnsureSuccessStatusCode();

				var content = response.Content.ReadAsStringAsync().Result;
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
				return null;
			}
		}

		public static async Task<Xliff> Translate(ITranslationOptions translationOptions, PairMapping pairMapping, Xliff sourceXliff)
		{
			try
			{
				var translationRequestResponse = await SendTranslationRequest(translationOptions, pairMapping, sourceXliff);
				var translationRequest = JsonConvert.DeserializeObject<EdgeTranslationRequestResponse>(translationRequestResponse);

				var translationStatusReponse = await GetTranslationStatus(translationOptions, translationRequest.TranslationId);
				var translationStatus = JsonConvert.DeserializeObject<EdgeTranslationStatus>(translationStatusReponse);

				if (translationStatus.Error is not null)
				{
					ErrorHandling.ShowDialog(null, translationStatus.Error.Code, translationStatus.Error.Message);
					return null;
				}

				while (translationStatus.State != "done")
				{
					Thread.Sleep(500);
					translationStatusReponse = await GetTranslationStatus(translationOptions, translationRequest.TranslationId);
					translationStatus = JsonConvert.DeserializeObject<EdgeTranslationStatus>(translationStatusReponse);
				}

				var translationResponse = await GetTranslation(translationOptions, translationRequest.TranslationId);

				var translationDecoded = Base64Decode(translationResponse);
				var translatedXliff = Converter.ParseXliffString(translationDecoded);

				return translatedXliff;
			}
			catch
			{
				return null;
			}
		}

		private static async Task<string> GetTranslation(ITranslationOptions translationOptions, string translationId)
		{
			var edgeCredentials = translationOptions.EdgeCredentials;
			var accessToken = translationOptions.AccessToken;

			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, $"{edgeCredentials.Uri}api/v2/translations/{translationId}/download");
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		private static async Task<string> GetTranslationStatus(ITranslationOptions translationOptions, string requestId)
		{
			var edgeCredentials = translationOptions.EdgeCredentials;
			var accessToken = translationOptions.AccessToken;

			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, $"{edgeCredentials.Uri}api/v2/translations/{requestId}");
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		private static async Task<string> SendTranslationRequest(ITranslationOptions translationOptions, PairMapping pairMapping, Xliff sourceXliff)
		{
			var accessToken = translationOptions.AccessToken;

			var query = BuildQuery(pairMapping, sourceXliff);

			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, $"{accessToken.EdgeUri}api/v2/translations");
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			request.Content = new FormUrlEncodedContent(query);
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		private static Dictionary<string, string> BuildQuery(PairMapping pairMapping, Xliff sourceXliff)
		{
			const string InputFormat = "application/x-xliff";

			var input = Base64Encode(sourceXliff.ToString());
			var queryString = new Dictionary<string, string>
			{
				["languagePairId"] = pairMapping.SelectedModel.Model,
				["input"] = input,
				["inputFormat"] = InputFormat
			};

			if (pairMapping.SelectedDictionary is not null && !string.IsNullOrEmpty(pairMapping.SelectedDictionary.DictionaryId))
			{
				queryString["dictionaryIds"] = pairMapping.SelectedDictionary.DictionaryId;
			}

			if (pairMapping.LinguisticOptions is not null)
			{
				queryString["linguisticOptions"] = string.Empty;
				var linguisticOptionsDictionary = pairMapping.LinguisticOptions.ToDictionary(lo => lo.Id, lo => lo.SelectedValue);
				foreach (var linguisticOption in linguisticOptionsDictionary)
				{
					queryString["linguisticOptions"] += $"{linguisticOption.Key}:{linguisticOption.Value},";
				}

				queryString["linguisticOptions"] = queryString["linguisticOptions"].Substring(0, queryString["linguisticOptions"].Length - 1);
			}

			return queryString;
		}

		public static async Task SendFeedback(AccessToken accessToken, List<KeyValuePair<string, string>> collection)
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, $"{accessToken.EdgeUri}api/v2/feedback");
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
			var content = new FormUrlEncodedContent(collection);
			request.Content = content;
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
		}

		private static string Base64Encode(this string text)
			=> Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

		private static string Base64Decode(this string encodedText)
			=> Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
	}
}