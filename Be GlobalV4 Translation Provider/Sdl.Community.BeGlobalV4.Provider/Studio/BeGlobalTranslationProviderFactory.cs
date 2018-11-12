using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderFactory(Id = "BeGlobalTranslationProviderFactory",
		Name = "BeGlobalTranslationProviderFactory",
		Description = "Be Global Translation Provider")]
	public class BeGlobalTranslationProviderFactory	: ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			var originalUri = new Uri("beglobaltranslationprovider:///");
			var options = new BeGlobalTranslationOptions(translationProviderUri);

			if (credentialStore.GetCredential(originalUri) != null)
			{
				var credentials = credentialStore.GetCredential(originalUri);
				options.ApiKey = credentials.Credential;
			}
			else
			{
				credentialStore.AddCredential(originalUri, new TranslationProviderCredential(originalUri.ToString(), true));	
			}
			return  new BeGlobalTranslationProvider(options);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = string.Equals(translationProviderUri.Scheme, BeGlobalTranslationProvider.ListTranslationProviderScheme,
				StringComparison.OrdinalIgnoreCase);
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
