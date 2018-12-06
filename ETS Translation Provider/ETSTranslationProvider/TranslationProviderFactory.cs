using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace ETSTranslationProvider
{
    [TranslationProviderFactory(
        Id = "TranslationProviderFactory",
        Name = "TranslationProviderFactory",
        Description = "ETS translation provider.")]

    public class TranslationProviderFactory : ITranslationProviderFactory
    {
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            Log.logger.Info("Attempting to create a new translation provider with URI: {0}", translationProviderUri);

            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                Log.logger.Error("Cannot handle URI {0}.", translationProviderUri);
                throw new Exception("Cannot handle URI.");
            }

            TranslationProviderCredential credentials = credentialStore.GetCredential(translationProviderUri);
            if (credentials == null)
            {
                //Throw TranslationProviderAuthenticationException, which will cause Studio to call GetCredentialsFromUser
                throw new TranslationProviderAuthenticationException();
            }

            TranslationOptions options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
            GenericCredentials genericCredentials = new GenericCredentials(credentials.Credential);

            if (options.UseBasicAuthentication)
            {
                options.ApiToken = ETSApi.ETSTranslatorHelper.GetAuthToken(options, genericCredentials);
            }
            else
            {
                options.ApiToken = genericCredentials["API-Key"];
                ETSApi.ETSTranslatorHelper.VerifyBasicAPIToken(options, genericCredentials);
            }

            return new TranslationProvider(options);
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            Log.logger.Trace("");
            if (translationProviderUri == null)
            {
                Log.logger.Error("Attempted to use null translation provider URI.");
                throw new ArgumentNullException("translationProviderUri", "Translation provider URI not supported.");
            }
            return string.Equals(translationProviderUri.Scheme, TranslationProvider.TranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            Log.logger.Trace("");
            return new TranslationProviderInfo()
            {
                TranslationMethod = TranslationOptions.ProviderTranslationMethod,
                Name = PluginResources.Plugin_NiceName
            };
        }
    }
}
