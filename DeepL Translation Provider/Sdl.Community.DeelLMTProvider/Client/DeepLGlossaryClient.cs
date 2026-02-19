using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Client
{
    public class DeepLGlossaryClient : IDeepLGlossaryClient
    {
        public string ApiVersion { get; set; }

        private string ChosenBaseUrl => ApiVersion?.Contains("V1") ?? true ? Constants.BaseUrlV1 : Constants.BaseUrlV2;

        public async Task<ActionResult<GlossaryInfo>> DeleteGlossary(string apiKey, string glossaryId)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{ChosenBaseUrl}/glossaries/{glossaryId}"),
                Headers =
                {
                    { "Authorization", $"DeepL-Auth-Key {apiKey}" },
                },
            };
            using var response = await AppInitializer.Client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new(false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));

            return new(true, null, null);
        }

        public async Task<ActionResult<List<GlossaryInfo>>> GetGlossaries(string apiKey, bool continueOnCapturedContext = true)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{ChosenBaseUrl}/glossaries"),
                Headers =
                {
                    { "Authorization", $"DeepL-Auth-Key {apiKey}" },
                },
            };
            using var response = await AppInitializer.Client.SendAsync(request).ConfigureAwait(continueOnCapturedContext);

            if (!response.IsSuccessStatusCode)
                return new(false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));

            var serializedGlossaries = await response.Content.ReadAsStringAsync();

            return ErrorHandler.WrapTryCatch(() =>
                JObject.Parse(serializedGlossaries)["glossaries"]?.ToObject<List<GlossaryInfo>>());
        }

        public async Task<ActionResult<List<GlossaryLanguagePair>>> GetGlossarySupportedLanguagePairs(string apiKey, bool continueOnCapturedContext = true)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{ChosenBaseUrl}/glossary-language-pairs"),
                Headers =
                {
                    { "Authorization", $"DeepL-Auth-Key {apiKey}" },
                },
            };

            using var response = await AppInitializer.Client.SendAsync(request).ConfigureAwait(continueOnCapturedContext);

            if (!response.IsSuccessStatusCode)
                return new(false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));

            var serializedLanguagePairs = await response.Content.ReadAsStringAsync();

            return ErrorHandler.WrapTryCatch(() => JObject.Parse(serializedLanguagePairs)["supported_languages"]
                .ToObject<List<GlossaryLanguagePair>>());
        }

        public async Task<ActionResult<GlossaryInfo>> ImportGlossary(Glossary glossary, string apiKey)
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
                RequestUri = new Uri($"{ChosenBaseUrl}/glossaries"),
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
                return new(false, null, ErrorHandler.GetFailureMessage($"{response.ReasonPhrase}: {await response.Content.ReadAsStringAsync()}"));

            var serializedCreatedGlossary = await response.Content.ReadAsStringAsync();
            return ErrorHandler.WrapTryCatch(
                () => JObject.Parse(serializedCreatedGlossary).ToObject<GlossaryInfo>());
        }

        public async Task<ActionResult<List<GlossaryEntry>>> RetrieveGlossaryEntries(string glossaryId, string apiKey)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{ChosenBaseUrl}/glossaries/{glossaryId}/entries"),
                Headers =
                {
                    { "Authorization", $"DeepL-Auth-Key {apiKey}" },
                }
            };

            using var response = await AppInitializer.Client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return new(false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));

            var serializedGlossaryEntries = await response.Content.ReadAsStringAsync();
            return ErrorHandler.WrapTryCatch(
                () =>
                {
                    var entriesList = serializedGlossaryEntries.Split('\n');
                    var glossaryEntries = new List<GlossaryEntry>();

                    entriesList.ForEach(e =>
                    {
                        var entryParts = e.Split('\t');
                        glossaryEntries.Add(
                            new GlossaryEntry { SourceTerm = entryParts[0], TargetTerm = entryParts[1] });
                    });

                    return glossaryEntries;
                });
        }

        public async Task<ActionResult<GlossaryInfo>> UpdateGlossary(Glossary glossary, string glossaryId, string apiKey)
        {
            var deleteGlossaryResult = await DeleteGlossary(apiKey, glossaryId);
            if (!deleteGlossaryResult.Success) return deleteGlossaryResult;

            return await ImportGlossary(glossary, apiKey);
        }
    }
}