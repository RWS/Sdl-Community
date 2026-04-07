using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class LanguagePairValidationResult
    {
        public bool IsSourceLanguageSupported { get; set; }
        public bool IsTargetLanguageSupported { get; set; }
        public bool SupportsFormality { get; set; }
        public bool SupportsAdvancedModelTypes { get; set; }
        public bool SupportsGlossaries { get; set; }
        public List<string> Messages { get; set; } = new();
    }
}
