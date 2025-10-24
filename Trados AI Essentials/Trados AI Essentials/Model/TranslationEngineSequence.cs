using System.Collections.Generic;
using Newtonsoft.Json;

namespace Trados_AI_Essentials.Model
{
    public class TranslationEngineSequence
    {
        [JsonProperty("llm")]
        public List<string> Llm { get; set; }
    }
}