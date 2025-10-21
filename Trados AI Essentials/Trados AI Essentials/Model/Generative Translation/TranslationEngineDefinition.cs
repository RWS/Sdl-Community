using Newtonsoft.Json;

namespace Trados_AI_Essentials.Model.Generative_Translation
{
    public class TranslationEngineDefinition
    {
        [JsonProperty("sequence")]
        public TranslationEngineSequence Sequence { get; set; }
    }
}