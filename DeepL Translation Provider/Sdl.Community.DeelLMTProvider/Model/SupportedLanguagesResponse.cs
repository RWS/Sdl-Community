using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class SupportedLanguagesResponse
    {
        public List<SupportedLanguage> SupportedLanguages { get; set; }
    }
    public class SupportedLanguage
    {
        public string Language { get; set; }
        public string Name { get; set; }
        public bool SupportsFormality { get; set; }
    }

    
}