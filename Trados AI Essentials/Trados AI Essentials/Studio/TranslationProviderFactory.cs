using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace Trados_AI_Essentials.Studio
{
    [TranslationProviderFactory(Id = "Translation_Provider_Plug_inFactory",
                                Name = "Translation_Provider_Plug_inFactory",
                                Description = "Translation_Provider_Plug_inFactory")]
    internal class TranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return new TranslationProvider();
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri,
            string translationProviderState) =>
            new TranslationProviderInfo
            {
                TranslationMethod = TranslationMethod.MachineTranslation,
                Name = PluginResources.Plugin_Name
            };

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return true;
        }

        #endregion ITranslationProviderFactory Members
    }
}