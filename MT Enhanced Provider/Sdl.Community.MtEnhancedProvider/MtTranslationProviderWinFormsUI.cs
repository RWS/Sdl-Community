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
using System.Reflection;
using System.Windows.Forms;
using NLog;
using Sdl.Community.MtEnhancedProvider.Service;
using Sdl.Community.MtEnhancedProvider.View;
using Sdl.Community.MtEnhancedProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider
{

    [TranslationProviderWinFormsUi(
        Id = "MtTranslationProviderWinFormsUI",
        Name = "MtTranslationProviderWinFormsUI",
        Description = "MtTranslationProviderWinFormsUI")]

    public class MtTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Show the plug-in settings form when the user is adding the translation provider plug-in
		/// through the GUI of SDL Trados Studio
		/// </summary>
		private TranslationProviderCredential GetMyCredentials(ITranslationProviderCredentialStore credentialStore, string uri)
        {
			var myUri = new Uri(uri);
            TranslationProviderCredential cred = null;

            if (credentialStore.GetCredential(myUri) != null)
            {
                //get the credential to return
                cred = new TranslationProviderCredential(credentialStore.GetCredential(myUri).Credential, credentialStore.GetCredential(myUri).Persist);
            }

            return cred;
        }

        private void SetMstCredentials(ITranslationProviderCredentialStore credentialStore, string clientId, bool persistCred)
        { 
            var myUri = new Uri(PluginResources.UriMs);

            var cred = new TranslationProviderCredential(clientId, persistCred);
            credentialStore.RemoveCredential(myUri);
            credentialStore.AddCredential(myUri, cred);
        }

        private void SetGoogleCredentials(ITranslationProviderCredentialStore credentialStore, string apiKey, bool persistKey)
        { 
            var myUri = new Uri(PluginResources.UriGt);
            var cred = new TranslationProviderCredential(apiKey, persistKey);
            credentialStore.RemoveCredential(myUri);
            credentialStore.AddCredential(myUri, cred);
        }

	    public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs,
		    ITranslationProviderCredentialStore credentialStore)
	    {
		    //construct options to send to form
		    var loadOptions = new MtTranslationOptions();

		    //get saved key if there is one and put it into options
		    //get google credentials
		    var getCredGt = GetMyCredentials(credentialStore, PluginResources.UriGt);
		    if (getCredGt != null)
		    {
			    loadOptions.ApiKey = getCredGt.Credential;
			    loadOptions.PersistGoogleKey = getCredGt.Persist;
		    }

		    //get microsoft credentials
		    var getCredMt = GetMyCredentials(credentialStore, PluginResources.UriMs);
		    if (getCredMt != null)
		    {
			    try
			    {
				    loadOptions.ClientId = getCredMt.Credential;
					loadOptions.PersistMicrosoftCreds = getCredMt.Persist;
			    }
			    catch (Exception ex) //swallow b/c it will just fail to fill in instead of crashing the whole program
			    {
				    _logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");
			    }
		    }

		    //WPF LOGIC
		    var dialogService = new OpenFileDialogService();
		    var providerControlVm = new ProviderControlViewModel(loadOptions, credentialStore);

		    var settingsControlVm = new SettingsControlViewModel(loadOptions, dialogService);
		    var mainWindowVm = new MainWindowViewModel(loadOptions, providerControlVm, settingsControlVm, languagePairs);

		    var mainWindow = new MainWindow
		    {
			    DataContext = mainWindowVm
		    };

		    mainWindowVm.CloseEventRaised += () => mainWindow.Close();

		    mainWindow.ShowDialog();

		    if (!mainWindowVm.DialogResult) return null;

		    var options = mainWindowVm.Options;

		    var provider = new MtTranslationProvider(options);

		    if (options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
		    {
			    SetGoogleCredentials(credentialStore, options.ApiKey, options.PersistGoogleKey);
		    }

		    if (options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
		    {
			    //set mst cred
			    SetMstCredentials(credentialStore, options.ClientId, options.PersistMicrosoftCreds);
		    }
		    return new ITranslationProvider[] {provider};
	    }

	    /// <summary>
        /// Determines whether the plug-in settings can be changed
        /// by displaying the Settings button in SDL Trados Studio.
        /// </summary>

        public bool SupportsEditing => true;

		/// <summary>
		/// If the plug-in settings can be changed by the user,
		/// SDL Trados Studio will display a Settings button.
		/// By clicking this button, users raise the plug-in user interface,
		/// in which they can modify any applicable settings, in our implementation
		/// the delimiter character and the list file name.
		/// </summary>
		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var editProvider = translationProvider as MtTranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            //get saved key if there is one and put it into options
            //get google credentials
			//TODO: Refactor this part and move in a new method
            var getCredGt = GetMyCredentials(credentialStore, PluginResources.UriGt);
            if (getCredGt != null)
            {
                editProvider.Options.ApiKey = getCredGt.Credential;
                editProvider.Options.PersistGoogleKey = getCredGt.Persist;
            }

            //get microsoft credentials
            var getCredMt = GetMyCredentials(credentialStore, PluginResources.UriMs);
            if (getCredMt != null)
            {
                try
                {
					editProvider.Options.ClientId = getCredMt.Credential;
					editProvider.Options.PersistMicrosoftCreds = getCredMt.Persist;
                }
                catch(Exception ex) //swallow b/c it will just fail to fill in instead of crashing the whole program 
				{
					_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
				}
			}

			//WPF logic
	        var dialogService = new OpenFileDialogService();
	        var providerControlVm = new ProviderControlViewModel(editProvider.Options, credentialStore);

	        var settingsControlVm = new SettingsControlViewModel(editProvider.Options, dialogService);
	        var mainWindowVm = new MainWindowViewModel(editProvider.Options, providerControlVm, settingsControlVm, languagePairs);
	        var mainWindow = new MainWindow
	        {
		        DataContext = mainWindowVm
	        };
	        mainWindowVm.CloseEventRaised += () => mainWindow.Close();

			mainWindow.ShowDialog();
	        if (!mainWindowVm.DialogResult) return false;

	        editProvider.Options = mainWindowVm.Options;

	        if (editProvider.Options.SelectedProvider == MtTranslationOptions.ProviderType.GoogleTranslate)
	        {
		        SetGoogleCredentials(credentialStore, editProvider.Options.ApiKey, editProvider.Options.PersistGoogleKey);
	        }

	        if (editProvider.Options.SelectedProvider == MtTranslationOptions.ProviderType.MicrosoftTranslator)
	        {
		        SetMstCredentials(credentialStore, editProvider.Options.ClientId, editProvider.Options.PersistMicrosoftCreds);
	        }
	        return true;
        }

        /// <summary>
        /// This gets called when a TranslationProviderAuthenticationException is thrown
        /// Since SDL Studio doesn't pass the provider instance here and even if we do a workaround...
        /// any new options set in the form that comes up are never saved to the project XML...
        /// so there is no way to change any options, only to provide the credentials
        /// </summary>
        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
			throw new NotImplementedException();
		}

        /// <summary>
        /// Used for displaying the plug-in info such as the plug-in name,
        /// tooltip, and icon.
        /// </summary>
        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo();
            var options = new MtTranslationOptions(translationProviderUri);
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

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
            }
            return string.Equals(translationProviderUri.Scheme, MtTranslationProvider.ListTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
        }

        public string TypeDescription => PluginResources.Plugin_Description;

        public string TypeName => PluginResources.Plugin_NiceName;

    }
}
