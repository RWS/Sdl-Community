using Newtonsoft.Json;

namespace Trados_AI_Essentials.Model
{
    public class TranslationEngineDefinition
    {
        [JsonProperty("sequence")]
        public TranslationEngineSequence Sequence { get; set; }
    }
}