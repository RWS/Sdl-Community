using System;
using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[TranslationProviderFactory(Id = "SDLMachineTranslationCloudProviderFactory",
		Name = "SDLMachineTranslationCloudProviderFactory",
		Description = "SDL Machine Translation Cloud Provider")]
	public class SdlMTCloudTranslationProviderFactory : ITranslationProviderFactory
	{
		public static readonly Log Log = Log.Instance;

		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{			
			var allForms = Application.OpenForms;
			var activeForm = allForms[allForms.Count - 1];
			var connectionService = new ConnectionService(activeForm?.Owner);
						
			var credential = connectionService.GetCredential(credentialStore);
			var connectionResult = connectionService.EnsureSignedIn(credential);

			if (!connectionResult.Item1)
			{
				throw new TranslationProviderAuthenticationException("Invalid credentials!");
			}

			connectionService.SaveCredential(credentialStore);

			var languageMappingsService = new LanguageMappingsService();
			var translationService = new TranslationService(connectionService, languageMappingsService);

			var provider = new SdlMTCloudTranslationProvider(translationProviderUri, connectionService, translationService, translationProviderState);
			

			return provider;

			//throw new TranslationProviderAuthenticationException("Invalid credentials!");
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = string.Equals(translationProviderUri.Scheme, Constants.MTCloudUriScheme, StringComparison.OrdinalIgnoreCase);
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