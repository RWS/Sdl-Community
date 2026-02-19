using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeeplRequestParameters
    {
        public string Formality { get; set; }

        [JsonProperty("glossary_id")]
        public string GlossaryId { get; set; }

        [JsonProperty("ignore_tags")]
        public List<string> IgnoreTags { get; set; }

        [JsonProperty("model_type")]
        public string ModelType { get; set; }

        [JsonProperty("preserve_formatting")]
        public bool PreserveFormatting { get; set; }

        [JsonProperty("source_lang")]
        public string SourceLanguage { get; set; }

        [JsonProperty("split_sentences")]
        public string SplittingSentenceHandling { get; set; }

        [JsonProperty("style_id")]
        public string StyleId { get; set; }

        [JsonProperty("tag_handling")]
        public string TagHandling { get; set; }

        [JsonProperty("tag_handling_version")]
        public string TagHandlingVersion { get; set; }

        [JsonProperty("target_lang")]
        public string TargetLanguage { get; set; }

        public List<string> Text { get; set; }
    }
}