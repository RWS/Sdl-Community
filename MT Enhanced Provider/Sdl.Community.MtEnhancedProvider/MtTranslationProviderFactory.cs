/* Copyright 2015 Patrick Porter

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/

using System;
using Sdl.Community.MtEnhancedProvider.MstConnect;
using Sdl.Community.MtEnhancedProvider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider
{
	[TranslationProviderFactory(
		Id = "MtTranslationProviderFactory",
		Name = "MtTranslationProviderFactory",
		Description = "MT Enhanced Trados Plugin")]

	public class MtTranslationProviderFactory : ITranslationProviderFactory
	{

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

			//create options class based on URI passed to the method
			var loadOptions = new MtTranslationOptions(translationProviderUri);
			var regionsProvider = new RegionsProvider();
			var htmlUtil = new HtmlUtil();

			//start with MT...check if we are using MT
			if (loadOptions.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator || loadOptions.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe)
			{
				// The credential is saved with a different URI scheme than that of the plugin!
				// We will need to make this known and/or provide a workaround in identifying the credentials
				// added from the project automation API.
				// The following is a work-around which attempts to recover the credential, given various scenarios
				var credential = credentialStore.GetCredential(new Uri(PluginResources.UriMs))
								 ?? credentialStore.GetCredential(translationProviderUri)
								 ?? credentialStore.GetCredential(new Uri(translationProviderUri.Scheme + ":///"));
				if (credential != null)
				{
					var cred = new TranslationProviderCredential(credential.Credential, credential.Persist);
					loadOptions.ClientId = cred.Credential;
					loadOptions.PersistMicrosoftCreds = cred.Persist;
				}
				else
				{
					throw new TranslationProviderAuthenticationException();
				}
			}
			else //if we are using Google as the provider need to get API key
			{
				if (loadOptions.SelectedGoogleVersion == Helpers.Enums.GoogleApiVersion.V2)
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
			}

			//construct new provider with options..these options are going to include the cred.credential and the cred.persists
			return new MtTranslationProvider(loadOptions, regionsProvider, htmlUtil);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{

			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
			}
			return string.Equals(translationProviderUri.Scheme, MtTranslationProvider.ListTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderInfo
			{
				TranslationMethod = MtTranslationOptions.ProviderTranslationMethod,
				Name = PluginResources.Plugin_NiceName
			};
			return info;
		}

	}
}
