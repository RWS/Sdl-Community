using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Client
{
	public class DeepLGlossaryClient : IDeepLGlossaryClient
	{
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
				return (false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));
			}
			var serializedGlossaries = await response.Content.ReadAsStringAsync();

			return ErrorHandler.WrapTryCatch(() =>
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
				return (false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));
			}

			var serializedLanguagePairs = await response.Content.ReadAsStringAsync();

			return ErrorHandler.WrapTryCatch(() => JObject.Parse(serializedLanguagePairs)["supported_languages"]
				.ToObject<List<GlossaryLanguagePair>>());
		}

		public async Task<(bool Success, GlossaryInfo result, string FailureMessage)> ImportGlossary(Glossary glossary, string apiKey)
		{
			var glossaryEntriesBuilder = new StringBuilder();
            glossary.Entries.ForEach(ge => glossaryEntriesBuilder.AppendLine($"{ge.SourceTerm}\t{ge.TargetTerm}"));

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
				return (false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));
			}

			var serializedCreatedGlossary = await response.Content.ReadAsStringAsync();
			return ErrorHandler.WrapTryCatch(() => JObject.Parse(serializedCreatedGlossary).ToObject<GlossaryInfo>());
		}
	}
}