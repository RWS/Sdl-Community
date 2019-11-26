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

            //start with MT...check if we are using MT
            if (loadOptions.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
            {
                var myUri = new Uri("mtenhancedprovidermst:///");
                if (credentialStore.GetCredential(myUri) != null)
                {
                    var credPersists = credentialStore.GetCredential(myUri).Persist;
                    var cred = new TranslationProviderCredential("", credPersists); //will this work??

                    cred = credentialStore.GetCredential(myUri); //if credential is there then just get it

                    var cred2 = new GenericCredentials(cred.Credential);//convert to generic credentials
                    //add creds to options
                    loadOptions.ClientId = cred2.UserName;
                    loadOptions.ClientSecret = cred2.Password;
                }
                else
                {
                    throw new TranslationProviderAuthenticationException();
                }
            }
            else //if we are using Google as the provider need to get API key
            {
                var myUri = new Uri("mtenhancedprovidergt:///");
                if (credentialStore.GetCredential(myUri) != null)
                {
                    var credPersists = credentialStore.GetCredential(myUri).Persist;
                    var cred = new TranslationProviderCredential("", credPersists); //will this work??

                    cred = credentialStore.GetCredential(myUri); //if credential is there then just get it
                    //add gt key to options
                    loadOptions.ApiKey = cred.Credential;
                }
                else
                {
                    throw new TranslationProviderAuthenticationException(); 
                    //throwing this exception ends up causing Studio to call MtTranslationProviderWinFormsUI.GetCredentialsFromUser();
                    //which we use to prompt the user to enter credentials
                }
            }
            
            //construct new provider with options..these options are going to include the cred.credential and the cred.persists
            var tp = new MtTranslationProvider(loadOptions);

            return tp;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {

            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
            }
            return String.Equals(translationProviderUri.Scheme, MtTranslationProvider.ListTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderInfo();

            info.TranslationMethod = MtTranslationOptions.ProviderTranslationMethod;

            info.Name = PluginResources.Plugin_NiceName;

            return info;
        }

    }
}
