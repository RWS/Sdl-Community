using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Client
{
    public class TMClient : ITMClient
    {
        private const int PageSize = 25;
        private static readonly Logger Logger = Log.GetLogger(nameof(TMClient));

        private static string BaseUrl => Constants.BaseUrl;

        public async Task<ActionResult<List<TranslationMemoryInfo>>> GetTranslationMemoriesAsync(string apiKey, bool continueOnCapturedContext = true)
        {
            try
            {
                var allMemories = new List<TranslationMemoryInfo>();
                int page = 0;
                int totalCount;

                do
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"{BaseUrl}/v3/translation_memories?page={page}&page_size={PageSize}"),
                        Headers =
                        {
                            { "Authorization", $"DeepL-Auth-Key {apiKey}" },
                        },
                    };

                    Logger.Info($"Requesting translation memories page {page} from: {request.RequestUri}");

                    using var response = await AppInitializer.Client.SendAsync(request).ConfigureAwait(continueOnCapturedContext);

                    Logger.Info($"Response status: {response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext);
                        Logger.Error($"TMClient failed with {response.StatusCode}: {errorContent}");
                        return new(false, null, ErrorHandler.GetFailureMessage(response.ReasonPhrase));
                    }

                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext);
                    var json = JObject.Parse(content);

                    var memories = json["translation_memories"]?.ToObject<List<TranslationMemoryInfo>>() ?? new List<TranslationMemoryInfo>();
                    totalCount = json["total_count"]?.Value<int>() ?? 0;

                    allMemories.AddRange(memories);
                    page++;
                }
                while (allMemories.Count < totalCount);

                Logger.Info($"Loaded {allMemories.Count} translation memories");

                return new(true, allMemories, null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new(false, null, ErrorHandler.GetFailureMessage(ex.Message));
            }
        }
    }
}
