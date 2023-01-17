using System;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
	[TranslationProviderFactory(Id = "MicrosoftTranslatorProviderPlugin_Factory",
                                Name = "MicrosoftTranslatorProviderPlugin_Factory",
                                Description = "MicrosoftTranslatorProviderPlugin_Factory")]
	public class ProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

			var credential = credentialStore.GetCredential(new Uri(Constants.MicrosoftProviderFullScheme))
						  ?? credentialStore.GetCredential(translationProviderUri)
						  ?? credentialStore.GetCredential(new Uri(translationProviderUri.Scheme + ":///"));
			if (credential is null)
			{
				throw new TranslationProviderAuthenticationException();
			}

			var providerCredentials = new TranslationProviderCredential(credential.Credential, credential.Persist);
			var loadOptions = new MTETranslationOptions(translationProviderUri)
			{
				ClientID = providerCredentials.Credential,
				PersistMicrosoftCredentials = providerCredentials.Persist
			};

			return new Provider(loadOptions, new RegionsProvider(), new HtmlUtil());
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Constants.MicrosoftProviderScheme, StringComparison.OrdinalIgnoreCase);
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