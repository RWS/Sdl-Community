using Newtonsoft.Json.Linq;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Client
{
    public class DeepLGlossaryClient : IDeepLGlossaryClient
    {
        public string ApiVersion { get; set; }

        private string ChosenBaseUrl => Constants.BaseUrl;

        public async Task<ActionResult<List<GlossaryInfo>>> GetGlossaries(string apiKey, bool continueOnCapturedContext = true)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{ChosenBaseUrl}/v3/glossaries"),
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
            {
                var multilingualGlossaries = JObject.Parse(serializedGlossaries)["glossaries"]?.ToObject<List<GlossaryInfo>>();
                if (multilingualGlossaries == null) return null;

                return multilingualGlossaries
                    .SelectMany(g => g.Dictionaries ?? Enumerable.Empty<GlossaryLanguagePair>(),
                        (g, dict) => new GlossaryInfo
                        {
                            Id = g.Id,
                            Name = g.Name,
                            SourceLanguage = dict.SourceLanguage,
                            TargetLanguage = dict.TargetLanguage
                        })
                    .ToList();
            });
        }
    }
}