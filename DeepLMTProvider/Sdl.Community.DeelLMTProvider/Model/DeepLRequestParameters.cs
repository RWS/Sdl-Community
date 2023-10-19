using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeeplRequestParameters
    {
        public string Formality { get; set; }

        [JsonProperty("glossary_id")]
        public string GlossaryId { get; set; }

        [JsonProperty("source_lang")]
        public string SourceLanguage { get; set; }

        [JsonProperty("target_lang")]
        public string TargetLanguage { get; set; }

        public List<string> Text { get; set; }
    }
}