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
			    throw new TranslationProviderAuthenticationException();
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
	        if (translationProviderUri != null)
		        return string.Equals(translationProviderUri.Scheme, TranslationProvider.TranslationProviderScheme,
			        StringComparison.OrdinalIgnoreCase);
	        _logger.Error("Attempted to use null translation provider URI.");
            throw new ArgumentNullException("translationProviderUri", @"Translation provider URI not supported.");
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            return new TranslationProviderInfo
            {
                TranslationMethod = TranslationOptions.ProviderTranslationMethod,
                Name = PluginResources.Plugin_NiceName
            };
        }
    }
}