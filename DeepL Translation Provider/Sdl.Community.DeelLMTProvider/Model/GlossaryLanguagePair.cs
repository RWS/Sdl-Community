using Newtonsoft.Json;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryLanguagePair
    {
        public string Label { get; set; } = null;

        [JsonProperty("source_lang")]
        public string SourceLanguage { get; set; }

        [JsonProperty("target_lang")]
        public string TargetLanguage { get; set; }

        public override string ToString() => Label ?? $"{SourceLanguage} -> {TargetLanguage}";
    }
}