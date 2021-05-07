using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using NLog;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
	public class Helpers
	{
		public const string DeeplTranslationProviderScheme = "deepltranslationprovider:///";
		private static readonly Logger _logger = Log.GetLogger(nameof(Helpers));
		private string _apiKey;
		public HttpResponseMessage IsApiKeyValidResponse { get; private set; }

		private static List<string> FormalityIncompatibleTargetLanguages { get; set; }

		private string ApiKey
		{
			get => _apiKey;
			set
			{
				_apiKey = value;
				OnApiKeyChanged();
			}
		}

		public static List<string> GetSupportedSourceLanguages(string apiKey)
		{
			var supportedLanguages = new List<string>();
			try
			{
				var response = GetSupportedLanguages("source", apiKey);
				supportedLanguages = JArray.Parse(response).Select(item => item["language"].ToString().ToUpperInvariant()).ToList();
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex}");
			}

			return supportedLanguages;
		}

		public static Dictionary<string, bool> GetSupportedTargetLanguages(string apiKey)
		{
			var supportedLanguages = new Dictionary<string, bool>();
			try
			{
				var response = GetSupportedLanguages("target", apiKey);
				supportedLanguages =
					JArray.Parse(response).ToDictionary(
						item => item["language"].ToString().ToUpperInvariant(), item => bool.Parse(item["supports_formality"].ToString()));
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex}");
			}

			return supportedLanguages;
		}

		public List<string> GetFormalityIncompatibleLanguages(List<CultureInfo> targetLanguages)
		{
			if (FormalityIncompatibleTargetLanguages is null) return null;
			return targetLanguages.Where(tl => !IsLanguageCompatible(tl)).Select(tl => tl.Name).ToList();
		}

		public void SetApiKey(string apiKey)
		{
			ApiKey = apiKey;
		}

		private static string GetSupportedLanguages(string type, string apiKey)
		{
			var content = new StringContent($"type={type}" + $"&auth_key={apiKey}", Encoding.UTF8,
				"application/x-www-form-urlencoded");

			var response = AppInitializer.Client.PostAsync("https://api.deepl.com/v1/languages", content).Result;
			response.EnsureSuccessStatusCode();

			return response.Content?.ReadAsStringAsync().Result;
		}

		private static bool IsLanguageCompatible(CultureInfo targetLanguage)
		{
			return !FormalityIncompatibleTargetLanguages.Contains(targetLanguage.ToString().ToLower()) &&
				   !FormalityIncompatibleTargetLanguages.Contains(targetLanguage.TwoLetterISOLanguageName.ToLower());
		}

		private static HttpResponseMessage IsValidApiKey(string apiKey)
		{
			return AppInitializer.Client.GetAsync($"https://api.deepl.com/v1/usage?auth_key={apiKey}").Result;
		}

		private void OnApiKeyChanged()
		{
			IsApiKeyValidResponse = IsValidApiKey(ApiKey);

			if (IsApiKeyValidResponse.IsSuccessStatusCode)
			{
				SetFormalityIncompatibleLanguages();
			}
		}

		private void SetFormalityIncompatibleLanguages()
		{
			FormalityIncompatibleTargetLanguages =
				GetSupportedTargetLanguages(ApiKey).Where(sl => !sl.Value).Select(sl => sl.Key.ToLower()).ToList();
		}
	}
}