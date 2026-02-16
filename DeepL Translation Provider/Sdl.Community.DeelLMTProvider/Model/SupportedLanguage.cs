using Newtonsoft.Json;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class SupportedLanguage
    {
        public string Language { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// DeepL API: Languages with this set to false do not support glossaries or any model type other than quality_optimized.
        /// </summary>
        [JsonProperty("supports_formality")]
        public bool SupportsOptions { get; set; }
    }
}