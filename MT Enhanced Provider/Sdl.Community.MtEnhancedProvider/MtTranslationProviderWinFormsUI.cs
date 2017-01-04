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
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider
{
    #region "Declaration"
    [TranslationProviderWinFormsUi(
        Id = "MtTranslationProviderWinFormsUI",
        Name = "MtTranslationProviderWinFormsUI",
        Description = "MtTranslationProviderWinFormsUI")]
    #endregion
    public class MtTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members


        /// <summary>
        /// Show the plug-in settings form when the user is adding the translation provider plug-in
        /// through the GUI of SDL Trados Studio
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>

        private TranslationProviderCredential GetMyCredentials(ITranslationProviderCredentialStore credentialStore, string uri)
        {
            Uri myUri = new Uri(uri);
            TranslationProviderCredential cred = null;

            if (credentialStore.GetCredential(myUri) != null)
            {

                //get the credential to return
                cred = new TranslationProviderCredential(credentialStore.GetCredential(myUri).Credential, credentialStore.GetCredential(myUri).Persist);
            }

            return cred;

        }

        private void SetMstCredentials(ITranslationProviderCredentialStore credentialStore, GenericCredentials creds, bool persistCred)
        { //used to set credentials
            // we are only setting and getting credentials for the uri with no parameters...kind of like a master credential
            Uri myUri = new Uri("mtenhancedprovidermst:///");

            TranslationProviderCredential cred = new TranslationProviderCredential(creds.ToCredentialString(), persistCred);
            credentialStore.RemoveCredential(myUri);
            credentialStore.AddCredential(myUri, cred);
        }

        private void SetGoogleCredentials(ITranslationProviderCredentialStore credentialStore, string apiKey, bool persistKey)
        { //used to set credentials
            // we are only setting and getting credentials for the uri with no parameters...kind of like a master credential
            Uri myUri = new Uri("mtenhancedprovidergt:///");
            TranslationProviderCredential cred = new TranslationProviderCredential(apiKey, persistKey);
            credentialStore.RemoveCredential(myUri);
            credentialStore.AddCredential(myUri, cred);
        }

        #region "Browse"
        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            //construct options to send to form
            MtTranslationOptions loadOptions = new MtTranslationOptions();
            //get saved key if there is one and put it into options
            //get google credentials
            TranslationProviderCredential getCredGt = GetMyCredentials(credentialStore, "mtenhancedprovidergt:///");
            if (getCredGt != null)
            {
                loadOptions.apiKey = getCredGt.Credential;
                loadOptions.persistGoogleKey = getCredGt.Persist;
            }
            
            //get microsoft credentials
            TranslationProviderCredential getCredMt = GetMyCredentials(credentialStore, "mtenhancedprovidermst:///");
            if (getCredMt != null)
            {
                try
                {
                    GenericCredentials creds = new GenericCredentials(getCredMt.Credential); //parse credential into username and password
                    loadOptions.ClientID = creds.UserName;
                    loadOptions.ClientSecret = creds.Password;
                    loadOptions.persistMicrosoftCreds = getCredMt.Persist;
                }
                catch { } //swallow b/c it will just fail to fill in instead of crashing the whole program 
            }

            //construct form
            MtProviderConfDialog dialog = new MtProviderConfDialog(loadOptions, credentialStore);
            //we are letting user delete creds but after testing it seems that it's ok if the individual credentials are null, b/c our method will re-add them to the credstore based on the uri
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                MtTranslationProvider testProvider = new MtTranslationProvider(dialog.Options);
                string apiKey = dialog.Options.apiKey;
                
                //we are setting credentials selectively based on the chosen provider to avoid saving the other if it is blank
                if (dialog.Options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
                {
                    //set google credential
                    SetGoogleCredentials(credentialStore, apiKey, dialog.Options.persistGoogleKey);
                }
                else if (dialog.Options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
                {
                    //set mst cred
                    GenericCredentials creds2 = new GenericCredentials(dialog.Options.ClientID, dialog.Options.ClientSecret);
                    SetMstCredentials(credentialStore, creds2, dialog.Options.persistMicrosoftCreds);
                }

                return new ITranslationProvider[] { testProvider };
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Determines whether the plug-in settings can be changed
        /// by displaying the Settings button in SDL Trados Studio.
        /// </summary>
        #region "SupportsEditing"
        public bool SupportsEditing
        {
            get { return true; }
        }
        #endregion

        /// <summary>
        /// If the plug-in settings can be changed by the user,
        /// SDL Trados Studio will display a Settings button.
        /// By clicking this button, users raise the plug-in user interface,
        /// in which they can modify any applicable settings, in our implementation
        /// the delimiter character and the list file name.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProvider"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        #region "Edit"
        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            MtTranslationProvider editProvider = translationProvider as MtTranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            //get saved key if there is one and put it into options
            //get google credentials
            TranslationProviderCredential getCredGt = GetMyCredentials(credentialStore, "mtenhancedprovidergt:///");
            if (getCredGt != null)
            {
                editProvider.Options.apiKey = getCredGt.Credential;
                editProvider.Options.persistGoogleKey = getCredGt.Persist;
            }

            //get microsoft credentials
            TranslationProviderCredential getCredMt = GetMyCredentials(credentialStore, "mtenhancedprovidermst:///");
            if (getCredMt != null)
            {
                try
                {
                    GenericCredentials creds = new GenericCredentials(getCredMt.Credential); //parse credential into username and password
                    editProvider.Options.ClientID = creds.UserName;
                    editProvider.Options.ClientSecret = creds.Password;
                    editProvider.Options.persistMicrosoftCreds = getCredMt.Persist;
                }
                catch { }//swallow b/c it will just fail to fill in instead of crashing the whole program 
            }

            MtProviderConfDialog dialog = new MtProviderConfDialog(editProvider.Options, credentialStore);
            //we are letting user delete creds but after testing it seems that it's ok if the individual credentials are null, b/c our method will re-add them to the credstore based on the uri
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                editProvider.Options = dialog.Options;

                string apiKey = editProvider.Options.apiKey;
                
                //we are setting credentials selectively based on the chosen provider to avoid saving the other if it is blank
                if (dialog.Options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
                {
                    //set google credential
                    SetGoogleCredentials(credentialStore, apiKey, dialog.Options.persistGoogleKey);
                }
                else if (dialog.Options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
                {
                    //set mst cred
                    GenericCredentials creds2 = new GenericCredentials(dialog.Options.ClientID, dialog.Options.ClientSecret);
                    SetMstCredentials(credentialStore, creds2, dialog.Options.persistMicrosoftCreds);
                }
                return true;
            }

            return false;
        }
        #endregion


        /// <summary>
        /// This gets called when a TranslationProviderAuthenticationException is thrown
        /// Since SDL Studio doesn't pass the provider instance here and even if we do a workaround...
        /// any new options set in the form that comes up are never saved to the project XML...
        /// so there is no way to change any options, only to provide the credentials
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProviderUri"></param>
        /// <param name="translationProviderState"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        #region "GetCredentialsFromUser"
        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            
            MtTranslationOptions options = new MtTranslationOptions(translationProviderUri);
            string caption = "Credentials"; //default in case any problem retrieving localized resource below
            if (options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
                caption = PluginResources.PromptForCredentialsCaption_Google;
            else if (options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
                caption = PluginResources.PromptForCredentialsCaption_Microsoft;
            
            MtProviderConfDialog dialog = new MtProviderConfDialog(options, caption, credentialStore);
            dialog.DisableForCredentialsOnly(); //only show controls for setting credentials, as that is the only thing that will end up getting saved

            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                string apiKey = dialog.Options.apiKey;

                if (options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
                {
                    //set google credential
                    SetGoogleCredentials(credentialStore, apiKey, dialog.Options.persistGoogleKey);
                }
                else if (options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
                {
                    //set mst cred
                    GenericCredentials creds2 = new GenericCredentials(dialog.Options.ClientID, dialog.Options.ClientSecret);
                    SetMstCredentials(credentialStore, creds2, dialog.Options.persistMicrosoftCreds);
                }
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Used for displaying the plug-in info such as the plug-in name,
        /// tooltip, and icon.
        /// </summary>
        /// <param name="translationProviderUri"></param>
        /// <param name="translationProviderState"></param>
        /// <returns></returns>
        #region "GetDisplayInfo"
        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {

            TranslationProviderDisplayInfo info = new TranslationProviderDisplayInfo();
            MtTranslationOptions options = new MtTranslationOptions(translationProviderUri);
            info.TranslationProviderIcon = PluginResources.my_icon;
            
            if (options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
            {
                info.Name = PluginResources.Google_NiceName;
                info.TooltipText = PluginResources.Google_Tooltip;
                info.SearchResultImage = PluginResources.my_image;
            }
            else if (options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
            {
                info.Name = PluginResources.Microsoft_NiceName;
                info.TooltipText = PluginResources.Microsoft_Tooltip;
                info.SearchResultImage = PluginResources.microsoft_image;
            }
            else
            {
                info.Name = PluginResources.Plugin_NiceName;
                info.TooltipText = PluginResources.Plugin_Tooltip;
            }
            return info;
        }
        #endregion

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
            }
            return String.Equals(translationProviderUri.Scheme, MtTranslationProvider.ListTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
        }

        public string TypeDescription
        {
            get { return PluginResources.Plugin_Description; }
        }

        public string TypeName
        {
            get { return PluginResources.Plugin_NiceName; }
        }

        #endregion
    }
}
