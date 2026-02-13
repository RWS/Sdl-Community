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
                using var response = await AppInitializer.Client.SendAsync(request);
                return
                    JsonConvert.DeserializeObject<StyleRulesResponse>(
                        await response.Content.ReadAsStringAsync()).StyleRules;
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return [];
            }
        }
    }
}