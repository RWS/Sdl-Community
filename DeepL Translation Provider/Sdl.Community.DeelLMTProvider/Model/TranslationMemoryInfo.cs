using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class TranslationMemoryInfo
    {
        [JsonProperty("translation_memory_id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source_language")]
        public string SourceLanguage { get; set; }

        [JsonProperty("target_languages")]
        public List<string> TargetLanguages { get; set; }

        [JsonProperty("segment_count")]
        public int SegmentCount { get; set; }
    }
}
