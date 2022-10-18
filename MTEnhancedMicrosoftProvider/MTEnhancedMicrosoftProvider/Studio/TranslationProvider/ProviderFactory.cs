using System;
using MTEnhancedMicrosoftProvider.Model;
using MTEnhancedMicrosoftProvider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MTEnhancedMicrosoftProvider
{
	[TranslationProviderFactory(Id = "Translation_Provider_Plug_inFactory",
                                Name = "Translation_Provider_Plug_inFactory",
                                Description = "Translation_Provider_Plug_inFactory")]
	public class ProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

			var credential = credentialStore.GetCredential(new Uri(PluginResources.UriMs))
						  ?? credentialStore.GetCredential(translationProviderUri)
						  ?? credentialStore.GetCredential(new Uri(translationProviderUri.Scheme + ":///"));
			if (credential is null)
			{
				throw new TranslationProviderAuthenticationException();
			}

			var providerCredentials = new TranslationProviderCredential(credential.Credential, credential.Persist);
			var loadOptions = new MTETranslationOptions(translationProviderUri)
			{
				ClientId = providerCredentials.Credential,
				PersistMicrosoftCreds = providerCredentials.Persist
			};

			return new Provider(loadOptions, new RegionsProvider(), new HtmlUtil());
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Provider.ListTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				TranslationMethod = MTETranslationOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_NiceName
			};
		}
	}
}