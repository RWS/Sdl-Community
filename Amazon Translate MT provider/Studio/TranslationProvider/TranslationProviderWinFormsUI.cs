using Newtonsoft.Json;
using Sdl.Community.AmazonTranslateTradosPlugin.Model;
using Sdl.Community.AmazonTranslateTradosPlugin.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Studio.TranslationProvider
{

    [TranslationProviderWinFormsUi(
        Id = "MtTranslationProviderWinFormsUI",
        Name = "MtTranslationProviderWinFormsUI",
        Description = "MtTranslationProviderWinFormsUI")]

    public class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {

        /// <summary>
        /// Show the plug-in settings form when the user is adding the translation provider plug-in
        /// through the GUI of Trados Studio
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>

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

        private void SetCredentials(ITranslationProviderCredentialStore credentialStore, GenericCredentials creds, bool persistCred)
        { //used to set credentials
            // we are only setting and getting credentials for the uri with no parameters...kind of like a master credential
            var myUri = new Uri("amazontranslateprovider:///");

            var cred = new TranslationProviderCredential(creds.ToCredentialString(), persistCred);
            credentialStore.RemoveCredential(myUri);
            credentialStore.AddCredential(myUri, cred);
        }

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
			ApplicationInitializer.CredentialStore = credentialStore;
            //construct options to send to form
            var loadOptions = new TranslationOptions();
            //get saved key if there is one and put it into options

            //get credentials
            var getCredAmz = GetMyCredentials(credentialStore, "amazontranslateprovider:///");
            if (getCredAmz != null)
            {
                try
                {
                    var creds = new GenericCredentials(getCredAmz.Credential); //parse credential into username and password
                    loadOptions.AccessKey = creds.UserName;
                    loadOptions.SecretKey = creds.Password;

                    loadOptions.PersistAWSCreds = getCredAmz.Persist;
                    loadOptions.RegionName = GetRegionName();
                }
                catch { } //swallow b/c it will just fail to fill in instead of crashing the whole program 
            }

            //construct form
            var dialog = new MtProviderConfDialog(loadOptions, credentialStore);
            //we are letting user delete creds but after testing it seems that it's ok if the individual credentials are null, b/c our method will re-add them to the credstore based on the uri
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                var testProvider = new TranslationProvider(dialog.Options);

                //set credentials
                var creds2 = new GenericCredentials(dialog.Options.AccessKey, dialog.Options.SecretKey);
                SetCredentials(credentialStore, creds2, dialog.Options.PersistAWSCreds);
                SetRegionName(dialog.Options.RegionName);

                SetSupportedLanguages(languagePairs, loadOptions);
                return new ITranslationProvider[] { testProvider };
            }
            return null;
        }

        private static void SetSupportedLanguages(LanguagePair[] languagePairs, TranslationOptions loadOptions)
        {
            var apiConnecter = new AmazonService(loadOptions);
            foreach (var languagePair in languagePairs)
            {
                var supportedLanguages = apiConnecter.GetSupportedLanguages();
                var targetLanguage = languagePair.TargetCultureName.Substring(0, 2).ToLower();
                if (supportedLanguages.Contains(targetLanguage) && !loadOptions.LanguagesSupported.ContainsKey(targetLanguage))
                {
                    loadOptions.LanguagesSupported.Add(languagePair.TargetCultureName, "Amazon Translate");
                }
            }
        }

        /// <summary>
        /// Determines whether the plug-in settings can be changed
        /// by displaying the Settings button in Trados Studio.
        /// </summary>

        public bool SupportsEditing
        {
            get { return true; }
        }

        /// <summary>
        /// If the plug-in settings can be changed by the user,
        /// Trados Studio will display a Settings button.
        /// By clicking this button, users raise the plug-in user interface,
        /// in which they can modify any applicable settings, in our implementation
        /// the delimiter character and the list file name.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProvider"></param>
        /// <param name="languagePairs"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
			ApplicationInitializer.CredentialStore = credentialStore;
            var editProvider = translationProvider as TranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            //get credentials
            var getCredAmz = GetMyCredentials(credentialStore, "amazontranslateprovider:///");
            if (getCredAmz != null)
            {
                try
                {
                    var creds = new GenericCredentials(getCredAmz.Credential); //parse credential into username and password
                    editProvider.TranslationOptions.AccessKey = creds.UserName;
                    editProvider.TranslationOptions.SecretKey = creds.Password;
                    editProvider.TranslationOptions.PersistAWSCreds = getCredAmz.Persist;
                }
                catch { }//swallow b/c it will just fail to fill in instead of crashing the whole program 
            }

            var dialog = new MtProviderConfDialog(editProvider.TranslationOptions, credentialStore);
            //we are letting user delete creds but after testing it seems that it's ok if the individual credentials are null, b/c our method will re-add them to the credstore based on the uri
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                editProvider.TranslationOptions = dialog.Options;
                SetSupportedLanguages(languagePairs, editProvider.TranslationOptions);
                //set mst cred
                var creds2 = new GenericCredentials(dialog.Options.AccessKey, dialog.Options.SecretKey);
                SetCredentials(credentialStore, creds2, dialog.Options.PersistAWSCreds);
                return true;
            }

            return false;
        }

        /// <summary>
        /// This gets called when a TranslationProviderAuthenticationException is thrown
        /// Since Trados Studio doesn't pass the provider instance here and even if we do a workaround...
        /// any new options set in the form that comes up are never saved to the project XML...
        /// so there is no way to change any options, only to provide the credentials
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProviderUri"></param>
        /// <param name="translationProviderState"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {

            var options = new TranslationOptions(translationProviderUri);
            var caption = PluginResources.PromptForCredentialsCaption;
            var dialog = new MtProviderConfDialog(options, caption, credentialStore);
            dialog.DisableForCredentialsOnly(); //only show controls for setting credentials, as that is the only thing that will end up getting saved

            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {

                //set mst cred
                var creds2 = new GenericCredentials(dialog.Options.AccessKey, dialog.Options.SecretKey);
                SetCredentials(credentialStore, creds2, dialog.Options.PersistAWSCreds);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Used for displaying the plug-in info such as the plug-in name,
        /// tooltip, and icon.
        /// </summary>
        /// <param name="translationProviderUri"></param>
        /// <param name="translationProviderState"></param>
        /// <returns></returns>

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo
            {
                TranslationProviderIcon = PluginResources.AmazonTranslate,
                Name = PluginResources.Plugin_NiceName,
                TooltipText = PluginResources.Plugin_Tooltip,
                SearchResultImage = PluginResources.amazon_aws_small
            };
            //TODO: update icon
            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(PluginResources.UriNotSupportedMessage);
            }
            return string.Equals(translationProviderUri.Scheme, TranslationProvider.TranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
        }

        public string TypeDescription => PluginResources.Plugin_Description;

        public string TypeName => PluginResources.Plugin_NiceName;

        /// <summary>
        /// Set Region name in the .json settings file when user is adding the provider
        /// </summary>
        /// <param name="regionName">regionaName set from UI</param>
        private void SetRegionName(string regionName)
        {
            if (!Directory.Exists(Constants.JsonFilePath))
            {
                Directory.CreateDirectory(Constants.JsonFilePath);
            }
            var docPath = Path.Combine(Constants.JsonFilePath, Constants.JsonFileName);
            var jsonAmazonSettings = new JsonAmazonSettings { RegionName = regionName };
            var jsonResult = JsonConvert.SerializeObject(jsonAmazonSettings);

            if (File.Exists(docPath))
            {
                File.Delete(docPath);
            }
            File.Create(docPath).Dispose();

            using (var tw = new StreamWriter(docPath, true))
            {
                tw.WriteLine(jsonResult);
                tw.Close();
            }
        }

        /// <summary>
        /// Get region name from the.json settings file
        /// </summary>
        /// <returns>region name</returns>
        private string GetRegionName()
        {
            var docPath = Path.Combine(Constants.JsonFilePath, Constants.JsonFileName);
            if (File.Exists(docPath))
            {
                using (var r = new StreamReader(docPath))
                {
                    var json = r.ReadToEnd();
                    var item = JsonConvert.DeserializeObject<JsonAmazonSettings>(json);
                    if (!string.IsNullOrEmpty(item.RegionName))
                    {
                        return item.RegionName;
                    }
                }
            }
            return string.Empty;
        }

    }
}
