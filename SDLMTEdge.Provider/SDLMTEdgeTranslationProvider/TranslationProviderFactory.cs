using System;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.MTEdge.Provider.SDLMTEdgeApi;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider
{
	[TranslationProviderFactory(
        Id = "TranslationProviderFactory",
        Name = "TranslationProviderFactory",
        Description = "MTEdge translation provider.")]
    public class TranslationProviderFactory : ITranslationProviderFactory
    {
	    public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ITranslationProvider CreateTranslationProvider(
			Uri translationProviderUri,
			string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
        {
            _logger.Info("Attempting to create a new translation provider with URI: {0}", translationProviderUri);

            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                _logger.Error("Cannot handle URI {0}.", translationProviderUri);
                throw new Exception("Cannot handle URI.");
            }

            var credentials = credentialStore.GetCredential(translationProviderUri);
            if (credentials == null)
            {
				return new TranslationProvider(new TranslationOptions());
			}

			var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
            var genericCredentials = new GenericCredentials(credentials.Credential);

            if (options.UseBasicAuthentication)
            {
                options.ApiToken = SDLMTEdgeTranslatorHelper.GetAuthToken(options, genericCredentials);
            }
            else
            {
                options.ApiToken = genericCredentials["API-Key"];
                SDLMTEdgeTranslatorHelper.VerifyBasicAPIToken(options, genericCredentials);
            }
            return new TranslationProvider(options);
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            _logger.Trace("");
            if (translationProviderUri == null)
            {
                _logger.Error("Attempted to use null translation provider URI.");
                throw new ArgumentNullException("translationProviderUri", "Translation provider URI not supported.");
            }
            return string.Equals(translationProviderUri.Scheme, TranslationProvider.TranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            _logger.Trace("");
            return new TranslationProviderInfo()
            {
                TranslationMethod = TranslationOptions.ProviderTranslationMethod,
                Name = PluginResources.Plugin_NiceName
            };
        }
    }
}