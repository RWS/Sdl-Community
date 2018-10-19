/* 

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
using Sdl.Community.AmazonTranslateProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
    #region "Declaration"
    [TranslationProviderFactory(
        Id = "MtTranslationProviderFactory",
        Name = "MtTranslationProviderFactory",
        Description = "Amazon Translate Trados Plugin")]
    #endregion

    public class MtTranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        #region "CreateTranslationProvider"
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            #region "CheckHandlesUri"
            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new Exception(PluginResources.UriNotSupportedMessage);
            }
            #endregion
            //create options class based on URI passed to the method
            var loadOptions = new MtTranslationOptions(translationProviderUri);

            var myUri = new Uri("amazontranslateprovider:///");
            if (credentialStore.GetCredential(myUri) != null && credentialStore.GetCredential(myUri).Credential != String.Empty)
            {
                var credPersists = credentialStore.GetCredential(myUri).Persist;
                var cred = new TranslationProviderCredential("", credPersists); //will this work??
                cred = credentialStore.GetCredential(myUri); //if credential is there then just get it

                var cred2 = new GenericCredentials(cred.Credential);//convert to generic credentials
                //add creds to options
                loadOptions.AccessKey = cred2.UserName;
                loadOptions.SecretKey = cred2.Password;

            }
            else if (loadOptions.SelectedAuthType == MtTranslationOptions.AWSAuthType.AccessKeys)
            {
                throw new TranslationProviderAuthenticationException();
            }
            //construct new provider with options..these options are going to include the cred.credential and the cred.persists
            var tp = new MtTranslationProvider(loadOptions);

            return tp;
        }
        #endregion

        #region "SupportsTranslationProviderUri"
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {

            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
            }
            return String.Equals(translationProviderUri.Scheme, MtTranslationProvider.TranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region "GetTranslationProviderInfo"
        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderInfo();

            #region "TranslationMethod"
            info.TranslationMethod = MtTranslationOptions.ProviderTranslationMethod;
            #endregion

            #region "Name"
            info.Name = PluginResources.Plugin_NiceName;
            #endregion

            return info;
        }
        #endregion

        #endregion
    }
}
