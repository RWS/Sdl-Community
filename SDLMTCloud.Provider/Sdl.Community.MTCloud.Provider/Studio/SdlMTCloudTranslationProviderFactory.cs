﻿using System;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[TranslationProviderFactory(Id = "SDLMachineTranslationCloudProviderFactory",
		Name = "SDLMachineTranslationCloudProviderFactory",
		Description = "SDL Machine Translation Cloud")]
	public class SdlMTCloudTranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var connectionService = new ConnectionService(StudioInstance.GetActiveForm(), new VersionService(),
				StudioInstance.GetLanguageCloudIdentityApi(), MtCloudApplicationInitializer.Client);

			var credential = connectionService.GetCredential(credentialStore);
			var connectionResult = connectionService.EnsureSignedIn(credential);

			if (!connectionResult.Item1)
			{
				throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
			}
			connectionService.SaveCredential(credentialStore);

			MtCloudApplicationInitializer.SetTranslationService(connectionService);

			var languageProvider = new LanguageProvider();
			var provider = new SdlMTCloudTranslationProvider(translationProviderUri, translationProviderState,
				MtCloudApplicationInitializer.TranslationService, languageProvider);

			return provider;
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			MtCloudApplicationInitializer.RefreshQeStatus();
			var info = new TranslationProviderInfo
			{
				TranslationMethod = TranslationMethod.MachineTranslation,
				Name = PluginResources.Plugin_NiceName
			};

			return info;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = translationProviderUri.Scheme.StartsWith(Constants.MTCloudUriScheme);
			return supportsProvider;
		}
	}
}