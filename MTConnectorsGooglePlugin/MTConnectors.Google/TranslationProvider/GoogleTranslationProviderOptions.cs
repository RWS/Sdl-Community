using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;

namespace Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider
{
    /// <summary>
    /// A class that contains the translation provider options that need to be persisted when saving a cascade
    /// NB: This should be a public class and would normally include a <see cref="LanguagePair"/> setting.
    /// </summary>
    public class GoogleTranslationProviderOptions
    {
        /// <summary>
        /// The language pairs to be used for the provider.
        /// </summary>
        public IList<LanguagePair> LanguageDirections { get; set; }

    }
}
