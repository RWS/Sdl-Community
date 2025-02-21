using Sdl.Community.AmazonTranslateTradosPlugin.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Studio.TranslationProvider
{

    [TranslationProviderFactory(
        Id = "MtTranslationProviderFactory",
        Name = "MtTranslationProviderFactory",
        Description = "Amazon Translate Trados Plugin")]

    public class TranslationProviderFactory : ITranslationProviderFactory
    {

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
			ApplicationInitializer.CredentialStore = credentialStore;

            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new Exception(PluginResources.UriNotSupportedMessage);
            }

            //create options class based on URI passed to the method
            var loadOptions = new TranslationOptions(translationProviderUri);

            ApplicationInitializer.TranslationOptions ??= new Dictionary<string, TranslationOptions>();
            ApplicationInitializer.TranslationOptions[loadOptions.Id] = loadOptions;

            var myUri = new Uri("amazontranslateprovider:///");
            if (credentialStore.GetCredential(myUri) != null && credentialStore.GetCredential(myUri).Credential != string.Empty)
            {
                var cred = credentialStore.GetCredential(myUri);
                var cred2 = new GenericCredentials(cred.Credential);
                loadOptions.AccessKey = cred2.UserName;
                loadOptions.SecretKey = cred2.Password;

            }
            else if (loadOptions.SelectedAuthType == TranslationOptions.AWSAuthType.AccessKeys)
            {
                throw new TranslationProviderAuthenticationException();
            }
            //construct new provider with options..these options are going to include the cred.credential and the cred.persists
            var tp = new TranslationProvider(loadOptions);

            return tp;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {

            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
            }
            return String.Equals(translationProviderUri.Scheme, TranslationProvider.TranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderInfo
            {
                TranslationMethod = TranslationOptions.ProviderTranslationMethod,
                Name = PluginResources.Plugin_NiceName
            };

            return info;
        }

    }
}
