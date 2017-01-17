using System;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.Taus.Translation.Provider
{
    #region "Declaration"
    [TranslationProviderFactory(
        Id = "TausTranslationProviderFactory",
        Name = "TausTranslationProviderFactory",
        Description = "Searches in the Taus TM")]
    #endregion

    public class TausTranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        #region "CreateTranslationProvider"
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new Exception("Cannot handle URI.");
            }
            
            var tp = new TausTranslationProvider(new TausTranslationOptions(translationProviderUri));

            return tp;
        }
        #endregion

        #region "SupportsTranslationProviderUri"
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(@"Translation provider URI not supported.");
            }
            return String.Equals(translationProviderUri.Scheme, TausTranslationProvider.TausTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region "GetTranslationProviderInfo"
        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderInfo
            {
                TranslationMethod = TausTranslationOptions.ProviderTranslationMethod,
                Name = PluginResources.Plugin_NiceName
            };

            #region "TranslationMethod"

            #endregion

            #region "Name"

            #endregion

            return info;
        }
        #endregion

        #endregion
    }
}
