using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.DeelLMTProvider;

namespace Sdl.Community.DeepLMTProvider
{
    [TranslationProviderFactory(Id = "DeepLMtTranslationProviderFactory",
                             Name = "DeepLMtTranslationProviderFactory",
                             Description = "DeepL Mt Translation Provider")]
    public class DeepLMtTranslationProviderFactory : ITranslationProviderFactory
    {
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			var uri = new Uri("deeplprovider:///");
			var options = new DeepLTranslationOptions();

			if (credentialStore.GetCredential(uri) != null)
			{
				var credPersist = credentialStore.GetCredential(uri).Persist;
				var credentials = credentialStore.GetCredential(uri);
				options.ApiKey = credentials.Credential;
			}
			return new DeepLMtTranslationProvider(options);
		}

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
			var info = new TranslationProviderInfo()
			{
				TranslationMethod = TranslationMethod.MachineTranslation,
				Name = PluginResources.Plugin_NiceName
			};
			return info;
		}

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            //here see MT enhanced for the actual  value
            return true;
        }
    }
}
