using Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider
{
    /// <summary>
    /// This is the plug-in translation provider factory class for the Google translation provider plugin.
    /// It is implements <see cref="ITranslationProviderFactory"/> to allow it to be added to a Translation Provider cascade as a plug-in.
    /// </summary>
    [TranslationProviderFactory(Id = IdValues.PluginProviderGoogleTranslatorFactory_Id, Name = IdValues.PluginProviderGoogleTranslatorFactory_Name)]
    public class GoogleTranslationProviderFactoryPlugin : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("translationProviderUri");
            }

            string[] splitScheme = translationProviderUri.Scheme.Split('.');
            if (splitScheme.Length == 0)
            {
                return false;
            }

            return string.Equals(splitScheme[0], GoogleTranslationProvider.GetUriScheme, StringComparison.OrdinalIgnoreCase);
        }

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new Exception(PluginResources.Google_UriInvalid);
            }

            var translationProviderSettings = new Settings(translationProviderUri);
            if (translationProviderSettings == null)
            {
                throw new Exception(PluginResources.Google_UriInvalid);
            }

            if (string.IsNullOrEmpty(translationProviderSettings.UserKey) || string.IsNullOrEmpty(translationProviderSettings.ApiKey))
            {
                var googleCredential = credentialStore.GetCredential(translationProviderSettings.CredentialsUri);
                if (googleCredential != null)
                {
                    translationProviderSettings.ApiKey = googleCredential.Credential;
                }

                if (string.IsNullOrEmpty(translationProviderSettings.ApiKey))
                {
                    throw new TranslationProviderAuthenticationException();
                }
            }

            var tp = new GoogleTranslationProviderPlugin(translationProviderSettings);
            if (!string.IsNullOrEmpty(translationProviderState))
            {
                tp.LoadState(translationProviderState);
            }

            if (!tp.IsAPIKeyValid())
            {
                throw new TranslationProviderAuthenticationException();
            }

            return tp;
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var translationProviderInfo = new TranslationProviderInfo();
            translationProviderInfo.Name = GoogleTranslationProvider.GetTranslationProviderName();
            translationProviderInfo.TranslationMethod = TranslationMethod.MachineTranslation;
            return translationProviderInfo;
        }

        #endregion
    }
}
