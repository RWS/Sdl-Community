using System;
using MTEnhancedMicrosoftProvider.Model;
using MTEnhancedMicrosoftProvider.Service;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MTEnhancedMicrosoftProvider
{
    [TranslationProviderFactory(Id = "Translation_Provider_Plug_inFactory",
                                Name = "Translation_Provider_Plug_inFactory",
                                Description = "Translation_Provider_Plug_inFactory")]
	public class MTEMicrosoftProviderFactory : ITranslationProviderFactory
	{

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

			var loadOptions = new MTEMicrosoftTranslationOptions(translationProviderUri);
			var regionsProvider = new RegionsProvider();
			var htmlUtil = new HtmlUtil();
			if (loadOptions.SelectedProvider != MTEMicrosoftTranslationOptions.ProviderType.MicrosoftTranslator)
			{
				return new MTEMicrosoftProvider(loadOptions, regionsProvider, htmlUtil);
			}

			var credential = credentialStore.GetCredential(new Uri(PluginResources.UriMs))
						  ?? credentialStore.GetCredential(translationProviderUri)
						  ?? credentialStore.GetCredential(new Uri(translationProviderUri.Scheme + ":///"));
			if (credential is null)
			{
				throw new TranslationProviderAuthenticationException();
			}

			var providerCredentials = new TranslationProviderCredential(credential.Credential, credential.Persist);
			loadOptions.ClientId = providerCredentials.Credential;
			loadOptions.PersistMicrosoftCreds = providerCredentials.Persist;

			return new MTEMicrosoftProvider(loadOptions, regionsProvider, htmlUtil);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, MTEMicrosoftProvider.ListTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				TranslationMethod = MTEMicrosoftTranslationOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_NiceName
			};
		}
	}
}