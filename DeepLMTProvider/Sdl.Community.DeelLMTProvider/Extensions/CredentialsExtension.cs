using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace Sdl.Community.DeepLMTProvider.Extensions
{
    public static class CredentialsExtension
    {
        public static TranslationProviderCredential GetCredentials(this ITranslationProviderCredentialStore credentialStore, string uri)
        {
            var providerUri = new Uri(uri);
            TranslationProviderCredential cred = null;

            if (credentialStore.GetCredential(providerUri) != null)
                cred = new TranslationProviderCredential(credentialStore.GetCredential(providerUri).Credential,
                    credentialStore.GetCredential(providerUri).Persist);

            return cred;
        }

        public static void SetDeeplCredentials(this ITranslationProviderCredentialStore credentialStore, string apiKey, bool persistKey)
        {
            var uri = new Uri(PluginResources.DeeplTranslationProviderScheme);
            var credentials = new TranslationProviderCredential(apiKey, persistKey);
            credentialStore.RemoveCredential(uri);
            credentialStore.AddCredential(uri, credentials);
        }
    }
}