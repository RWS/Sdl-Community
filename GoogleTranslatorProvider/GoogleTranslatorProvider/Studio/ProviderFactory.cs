using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

			//create options class based on URI passed to the method
			var loadOptions = new GTPTranslationOptions(translationProviderUri);
			var htmlUtil = new HtmlUtil();



			if (loadOptions.SelectedGoogleVersion == ApiVersion.V2)
			{
				// The credential is saved with a different URI scheme than that of the plugin!
				// We will need to make this known and/or provide a workaround in identifying the credentials
				// added from the project automation API.
				// The following is a work-around which attempts to recover the credential, given various scenarios
				var credential = credentialStore.GetCredential(new Uri(PluginResources.UriGt))
								 ?? credentialStore.GetCredential(translationProviderUri)
								 ?? credentialStore.GetCredential(new Uri(translationProviderUri.Scheme + ":///"));
				if (credential != null)
				{
					var cred = new TranslationProviderCredential(credential.Credential, credential.Persist);
					loadOptions.ApiKey = cred.Credential;
					loadOptions.PersistGoogleKey = cred.Persist;
				}
				else
				{
					throw new TranslationProviderAuthenticationException();
					//throwing this exception ends up causing Studio to call MtTranslationProviderWinFormsUI.GetCredentialsFromUser();
					//which we use to prompt the user to enter credentials
				}
			}

			//construct new provider with options..these options are going to include the cred.credential and the cred.persists
			return new Provider(loadOptions, htmlUtil);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{

			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}
			return string.Equals(translationProviderUri.Scheme, Provider.ListTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderInfo
			{
				TranslationMethod = GTPTranslationOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_NiceName
			};
			return info;
		}
	}
}