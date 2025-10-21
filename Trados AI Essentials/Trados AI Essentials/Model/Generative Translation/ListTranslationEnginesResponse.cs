using System.Collections.Generic;
using Newtonsoft.Json;

namespace Trados_AI_Essentials.Model.Generative_Translation
{
    public class ListTranslationEnginesResponse
    {
        [JsonProperty("items")]
        public List<TranslationEngineItem> Items { get; set; }

        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }
    }
}
