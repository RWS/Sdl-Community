using System;
using System.Net.Http;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using HttpClient = Sdl.Community.MTCloud.Provider.Service.HttpClient;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[TranslationProviderFactory(Id = "SDLMachineTranslationCloudProviderFactory",
		Name = "SDLMachineTranslationCloudProviderFactory",
		Description = "SDL Machine Translation Cloud Provider")]
	public class SdlMTCloudTranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var connectionService = new ConnectionService(StudioInstance.GetActiveForm(), new VersionService(),
				StudioInstance.GetLanguageCloudIdentityApi(), MTCloudApplicationInitializer.Client);

			var credential = connectionService.GetCredential(credentialStore);
			var connectionResult = connectionService.EnsureSignedIn(credential);

			if (!connectionResult.Item1)
			{
				throw new TranslationProviderAuthenticationException(PluginResources.Message_Invalid_credentials);
			}
			connectionService.SaveCredential(credentialStore);

			var editorController = StudioInstance.GetEditorController();

			var translationService = new TranslationService(connectionService, MTCloudApplicationInitializer.Client);
			var languageProvider = new LanguageProvider();
			var projectsController = StudioInstance.GetProjectsController();

			var provider = new SdlMTCloudTranslationProvider(translationProviderUri, translationProviderState,
				translationService, languageProvider, editorController, projectsController);
			return provider;
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

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderInfo
			{
				TranslationMethod = TranslationMethod.MachineTranslation,
				Name = PluginResources.Plugin_NiceName
			};

			return info;
		}	
	}
}