using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Client
{
	public class DeepLGlossaryClient : IDeepLGlossaryClient
	{
		private static readonly Logger Logger = Log.GetLogger(nameof(DeepLGlossaryClient));

		public async Task<(bool Success, List<GlossaryInfo> Result, string FailureMessage)> GetGlossaries(string apiKey)
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("https://api.deepl.com/v1/glossaries"),
				Headers =
				{
					{ "Authorization", $"DeepL-Auth-Key {apiKey}" },
				},
			};
			using var response = await AppInitializer.Client.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				return (false, null, GetFailureMessage(response.ReasonPhrase));
			}
			var serializedGlossaries = await response.Content.ReadAsStringAsync();

			return WrapTryCatch(() =>
				JObject.Parse(serializedGlossaries)["glossaries"]?.ToObject<List<GlossaryInfo>>());
		}

		public async Task<(bool Success, List<GlossaryLanguagePair> Result, string FailureMessage)> GetGlossarySupportedLanguagePairs(string apiKey)
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("https://api.deepl.com/v2/glossary-language-pairs"),
				Headers =
				{
					{ "Authorization", $"DeepL-Auth-Key {apiKey}" },
				},
			};

			using var response = await AppInitializer.Client.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				return (false, null, GetFailureMessage(response.ReasonPhrase));
			}

			var serializedLanguagePairs = await response.Content.ReadAsStringAsync();

			return WrapTryCatch(() => JObject.Parse(serializedLanguagePairs)["supported_languages"]
				.ToObject<List<GlossaryLanguagePair>>());
		}

		public async Task<(bool Success, GlossaryInfo result, string FailureMessage)> ImportGlossary(Glossary glossary, string apiKey)
		{
			var glossaryEntriesBuilder = new StringBuilder();

			foreach (var glossaryEntry in glossary.Entries)
			{
				glossaryEntriesBuilder.AppendLine($"{glossaryEntry.SourceTerm}\t{glossaryEntry.TargetTerm}");
			}

			var content = new
			{
				name = glossary.Name,
				source_lang = glossary.SourceLanguage,
				target_lang = glossary.TargetLanguage,
				entries = glossaryEntriesBuilder.ToString(),
				entries_format = "tsv"
			};

			 var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri("https://api.deepl.com/v2/glossaries"),
				Headers =
				{
					{ "Authorization", $"DeepL-Auth-Key {apiKey}" },
				},
				Content = new StringContent(JsonConvert.SerializeObject(content))
				{
					Headers =
					{
						ContentType = new MediaTypeHeaderValue("application/json")
					}
				}
			};

			using var response = await AppInitializer.Client.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				HandleError("Glossary creation", response.ReasonPhrase);
			}

			var serializedCreatedGlossary = await response.Content.ReadAsStringAsync();
			return WrapTryCatch(() => JObject.Parse(serializedCreatedGlossary).ToObject<GlossaryInfo>());
		}

		private string GetFailureMessage(string failureReason = null, [CallerMemberName] string failingMethod = null) =>
			$@"""{failingMethod}"" failed: {failureReason}";

		private void HandleError(string whatFailed, string whyItFailed) =>
			Logger.Warn($"{whatFailed} failed: {whyItFailed}");

		private (bool Success, T Result, string FailureMessage) WrapTryCatch<T>(Func<T> function, [CallerMemberName] string failingMethod = null)
		{
			try
			{
				return (true, function(), null);
			}
			catch (Exception e)
			{
				return (false, default, GetFailureMessage(failingMethod, e.Message));
			}
		}
	}
}