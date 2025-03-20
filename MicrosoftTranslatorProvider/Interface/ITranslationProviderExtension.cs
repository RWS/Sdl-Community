using System.Collections.Generic;

namespace MicrosoftTranslatorProvider.Interface
{
    public interface ITranslationProviderExtension
    {
        Dictionary<string, string> LanguagesSupported { get; set; }
    }
}
