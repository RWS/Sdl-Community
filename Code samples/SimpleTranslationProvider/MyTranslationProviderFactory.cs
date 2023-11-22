using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleTranslationProvider
{
    [TranslationProviderFactory(Id = "Translation_Provider_Plug_inFactory",
                                Name = "Translation_Provider_Plug_inFactory",
                                Description = "Translation_Provider_Plug_inFactory")]
    internal class MyTranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return new MyTranslationProvider();
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderInfo
            {
                TranslationMethod = TranslationMethod.MachineTranslation,
                Name = MyTranslationProviderWinFormsUI.TRANSLATION_URI,
            };

            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.StartsWith(MyTranslationProviderWinFormsUI.TRANSLATION_URI);
        }

        #endregion
    }
}
