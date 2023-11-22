using System;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.Studio
{
	[TranslationProviderFactory(Id = Constants.Provider_TranslationProviderFactory,
								Name = Constants.Provider_TranslationProviderFactory,
								Description = Constants.Provider_TranslationProviderFactory)]
	public class ProviderFactory : ITranslationProviderFactory
	{
		public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri,
															  string translationProviderState,
															  ITranslationProviderCredentialStore credentialStore)
		{
			_logger.Info("Attempting to create a new translation provider with URI: {0}", translationProviderUri);

			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				_logger.Error("Cannot handle URI {0}.", translationProviderUri);
				throw new Exception("Cannot handle URI.");
			}

			if (credentialStore.GetCredential(new TranslationProviderUriBuilder(Constants.TranslationProviderScheme).Uri) is not TranslationProviderCredential credentials)
			{
				throw new TranslationProviderAuthenticationException();
			}

			var genericCredentials = new GenericCredentials(credentials.Credential);
			var options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			options.ApiToken = genericCredentials["Token"];
			return new Provider(options);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				_logger.Error("Attempted to use null translation provider URI.");
				throw new ArgumentNullException("translationProviderUri", @"Translation provider URI not supported.");
			}

			var currentScheme = translationProviderUri.Scheme;
			return string.Equals(currentScheme, Constants.TranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
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