using System;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ETSTranslationProvider
{
	[TranslationProviderFactory(
        Id = "TranslationProviderFactory",
        Name = "TranslationProviderFactory",
        Description = "ETS translation provider.")]
    public class TranslationProviderFactory : ITranslationProviderFactory
    {
		public static readonly Log Log = Log.Instance;

		public ITranslationProvider CreateTranslationProvider(
			Uri translationProviderUri,
			string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
        {
            Log.Logger.Info("Attempting to create a new translation provider with URI: {0}", translationProviderUri);

            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                Log.Logger.Error("Cannot handle URI {0}.", translationProviderUri);
                throw new Exception("Cannot handle URI.");
            }

            var credentials = credentialStore.GetCredential(translationProviderUri);
            if (credentials == null)
            {
                //Throw TranslationProviderAuthenticationException, which will cause Studio to call GetCredentialsFromUser
                throw new TranslationProviderAuthenticationException();
            }

            var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
            var genericCredentials = new GenericCredentials(credentials.Credential);

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
            Log.Logger.Trace("");
            if (translationProviderUri == null)
            {
                Log.Logger.Error("Attempted to use null translation provider URI.");
                throw new ArgumentNullException("translationProviderUri", "Translation provider URI not supported.");
            }
            return string.Equals(translationProviderUri.Scheme, TranslationProvider.TranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            Log.Logger.Trace("");
            return new TranslationProviderInfo()
            {
                TranslationMethod = TranslationOptions.ProviderTranslationMethod,
                Name = PluginResources.Plugin_NiceName
            };
        }
    }
}