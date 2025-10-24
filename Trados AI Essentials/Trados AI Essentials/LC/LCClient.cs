using Newtonsoft.Json;
using Sdl.LanguageCloud.IdentityApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Trados_AI_Essentials.Interface;
using Trados_AI_Essentials.Model;

namespace Trados_AI_Essentials.LC
{
    public class LCClient : ILCClient
    {
        public LCClient(IHttpClient httpClient)
        {
            HttpClient = httpClient;
            Authenticate();
        }

        private string GenerativeTranslationAddress =>
            "https://api.eu.cloud.trados.com/lc-api/generative-translation/v1";

        private IHttpClient HttpClient { get; }

        private string LCBaseAddress =>
                    "https://lc-api.sdl.com/public-api/v1";

        public void Authenticate()
        {
            //TODO - implement refresh token mechanism (for cases in which users leave Trados open for a long time)
            var authenticated = LanguageCloudIdentityApi.Instance.TryLogin(out var errorMessage);
            if (!authenticated)
                throw new Exception($"Authentication failed: {errorMessage}");

            HttpClient.DefaultHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LanguageCloudIdentityApi.Instance.AccessToken);
            HttpClient.DefaultHeaders.Add("X-LC-Tenant", LanguageCloudIdentityApi.Instance.ActiveTenantId);
        }

        public async Task<List<TranslationEngineItem>> GetLLMTranslationEngines()
        {
            var responseSerialized = await HttpClient.SendAsync(HttpMethod.Get, null, $"{LCBaseAddress}/translation-engines?fields=name,definition.sequence.llm").ConfigureAwait(false);
            var listTranslationEnginesResponse = JsonConvert.DeserializeObject<ListTranslationEnginesResponse>(responseSerialized);

            var llmTranslationEngines = new List<TranslationEngineItem>();
            if (listTranslationEnginesResponse?.Items == null) return llmTranslationEngines;

            foreach (var engine in listTranslationEnginesResponse.Items)
                if (engine.Definition?.Sequence?.Llm?.Any() ?? false)
                    llmTranslationEngines.Add(engine);

            return llmTranslationEngines;
        }

        public async Task<GenerativeTranslationResult> TranslateAsync(TranslationRequest request)
        {
            var response = await HttpClient.SendAsync(HttpMethod.Post, request, $"{GenerativeTranslationAddress}" +
                "/translate" +
                $"?includeTranslationResources={request.IncludeUserResources}");

            return JsonConvert.DeserializeObject<GenerativeTranslationResult>(response);
        }
    }
}