using System;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider.Studio
{
	[TranslationProviderFactory(Id = "DeepLMtTranslationProviderFactory",
        Name = "DeepLMtTranslationProviderFactory",
        Description = "DeepL Mt Translation Provider")]
    public class DeepLMtTranslationProviderFactory : ITranslationProviderFactory
    {
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var originalUri = new Uri(PluginResources.DeeplTranslationProviderScheme);
            var options = new DeepLTranslationOptions(translationProviderUri, translationProviderState);

            if (credentialStore.GetCredential(originalUri) != null)
            {
                var credentials = credentialStore.GetCredential(originalUri);
                options.ApiKey = credentials.Credential;
            }
            else
            {
                credentialStore.AddCredential(originalUri, new TranslationProviderCredential(Guid.NewGuid().ToString(), true));
            }

            return new DeepLMtTranslationProvider(options, new DeepLTranslationProviderClient(options.ApiKey));
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

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(nameof(translationProviderUri));
            }

            var supportsProvider = string.Equals(translationProviderUri.Scheme, DeepLMtTranslationProvider.ListTranslationProviderScheme,
                StringComparison.OrdinalIgnoreCase);
            return supportsProvider;
        }
    }
}