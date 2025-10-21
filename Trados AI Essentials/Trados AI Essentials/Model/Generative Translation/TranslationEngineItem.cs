using Newtonsoft.Json;

namespace Trados_AI_Essentials.Model.Generative_Translation
{
    public class TranslationEngineItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("definition")]
        public TranslationEngineDefinition Definition { get; set; }
    }
}