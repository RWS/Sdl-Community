using System;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.Taus.Translation.Provider
{
    #region "Declaration"
    [TranslationProviderWinFormsUi(
        Id = "TausTranslationProviderWinFormsUI",
        Name = "TausTranslationProviderWinFormsUI",
        Description = "TausTranslationProviderWinFormsUI")]
    #endregion
    public class TausTranslationProviderWinFormsUi : ITranslationProviderWinFormsUI
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
        #region "Browse"
        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var dialog = new TausProviderConfDialog(new TausTranslationOptions());
            if (dialog.ShowDialog(owner) != DialogResult.OK) return null;
            var testProvider = new TausTranslationProvider(dialog.Options);
            return new ITranslationProvider[] { testProvider };
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
            var editProvider = translationProvider as TausTranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            var dialog = new TausProviderConfDialog(editProvider.Options);
            if (dialog.ShowDialog(owner) != DialogResult.OK) return false;
            editProvider.Options = dialog.Options;
            return true;
        }
        #endregion

        /// <summary>
        /// Can be used in implementations in which a user login is required, e.g.
        /// for connecting to an online translation provider.
        /// In our implementation, however, this is not required, so we simply set
        /// this member to return always True.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="translationProviderUri"></param>
        /// <param name="translationProviderState"></param>
        /// <param name="credentialStore"></param>
        /// <returns></returns>
        #region "GetCredentialsFromUser"
        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {            
            return true;
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
            var info = new TranslationProviderDisplayInfo
            {
                Name = PluginResources.Plugin_NiceName,
                TranslationProviderIcon = PluginResources.TausIcon,
                TooltipText = PluginResources.Plugin_Tooltip,
                SearchResultImage = PluginResources.TausImage
            };


            return info;
        }
        #endregion



        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(@"URI not supported by the plug-in.");
            }
            return string.Equals(translationProviderUri.Scheme, TausTranslationProvider.TausTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
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
