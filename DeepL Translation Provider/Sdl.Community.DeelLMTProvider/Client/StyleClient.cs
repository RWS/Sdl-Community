using Newtonsoft.Json;
using NLog;
using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Client
{
    public class StyleClient
    {
        private static readonly Logger Logger = Log.GetLogger(nameof(StyleClient));

        public static async Task<List<DeepLStyle>> GetStyles(string apiKey)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://api.deepl.com/v3/style_rules"),
                Headers =
                {
                    { "Authorization", $"DeepL-Auth-Key {apiKey}" },
                },
            };

            try
            {
                Logger.Info($"Requesting styles from: {request.RequestUri}");

                using var response = await AppInitializer.Client.SendAsync(request);

                Logger.Info($"Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Logger.Error($"StyleClient failed with {response.StatusCode}: {errorContent}");
                    return [];
                }

                var content = await response.Content.ReadAsStringAsync();
                Logger.Info($"Response Content: {content}");

                var styleRules = JsonConvert.DeserializeObject<StyleRulesResponse>(content).StyleRules;
                Logger.Info($"Loaded {styleRules?.Count ?? 0} styles");

                return styleRules ?? [];
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return [];
            }
        }
    }
}