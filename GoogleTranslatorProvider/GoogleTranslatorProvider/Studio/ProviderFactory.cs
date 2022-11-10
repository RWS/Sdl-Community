using System;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleTranslatorProvider.Studio
{
	[TranslationProviderFactory(Id = "GoogleTranslatorProviderPlugin_Factory",
								Name = "GoogleTranslatorProviderPlugin_Factory",
								Description = "GoogleTranslatorProviderPlugin_Factory")]
	public class ProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

			var htmlUtil = new HtmlUtil();
			var translationOptions = new GTPTranslationOptions(translationProviderUri);
			if (translationOptions.SelectedGoogleVersion is not ApiVersion.V2)
			{
				return new Provider(translationOptions, htmlUtil);
			}

			if ((credentialStore.GetCredential(translationProviderUri) ??
				 credentialStore.GetCredential(new Uri(Constants.GoogleTranslationFullScheme)))
				is not TranslationProviderCredential credentials)
			{
				throw new TranslationProviderAuthenticationException();
			}

			credentials = new TranslationProviderCredential(credentials.Credential, credentials.Persist);
			translationOptions.ApiKey = credentials.Credential;
			translationOptions.PersistGoogleKey = credentials.Persist;
			return new Provider(translationOptions, htmlUtil);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri is null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}

			return string.Equals(translationProviderUri.Scheme, Constants.GoogleTranslationScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return new TranslationProviderInfo
			{
				TranslationMethod = GTPTranslationOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_NiceName
			};
		}
	}
}