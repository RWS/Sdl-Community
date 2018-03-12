// ---------------------------------
// <copyright file="WinFormsUI.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-10-14</date>
// ---------------------------------
namespace Sdl.Community.Trados2007
{
    using System;
    using System.Windows.Forms;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using Sdl.Community.Trados2007.UI;

    /// <summary>
    /// Encapsulates WinForms UI logic for managing Trados 2007 translation providers.
    /// </summary>
    [TranslationProviderWinFormsUi(
        Id = WinFormsUIId,
        Name = WinFormsUIName,
        Description = WinFormsUIDescription)]
    public class WinFormsUI : ITranslationProviderWinFormsUI
    {
        #region Consts

        /// <summary>
        /// Trados2007TranslationProviderWinFormsUI string.
        /// </summary>
        internal const string WinFormsUIName = "Trados2007TranslationProviderWinFormsUI";

        /// <summary>
        /// Trados2007TranslationProviderWinFormsUI string.
        /// </summary>
        internal const string WinFormsUIId = "Trados2007TranslationProviderWinFormsUI";

        /// <summary>
        /// SDL Trados 2007 TranslationProvider plug-in string.
        /// </summary>
        internal const string WinFormsUIDescription = "SDL Trados 2007 TranslationProvider plug-in";

        #endregion Consts

        #region Properties

        /// <summary>
        /// Gets the type name of the factory; e.g. "SDL &Trados 2007 translation provider..."
        /// </summary>
        public string TypeName
        {
            get
            {
                return PluginResources.Trados2007_TypeName;
            }
        }

        /// <summary>
        /// Gets the type description of the factory; e.g. "A plug-in provider to use SDL Trados 2007 translation memories."
        /// </summary>
        public string TypeDescription
        {
            get
            {
                return PluginResources.Trados2007_TypeDescription;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this supplied translation provider can be edited (i.e. whether any settings can be changed).
        /// </summary>
        /// <returns><c>true</c> if the provider's settings can be changed, and <c>false</c> otherwise.</returns>
        public bool SupportsEditing
        {
            get { return false; }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Returns true if this component supports the specified translation provider URI.
        /// </summary>
        /// <param name="translationProviderUri">The uri.</param>
        /// <returns>
        /// True if this component supports the specified translation provider URI.
        /// </returns>
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("translationProviderUri");
            }

            return string.Equals(
                FileBasedTrados2007TranslationMemory.UriScheme,
                translationProviderUri.Scheme,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets display information for the specified translation provider.
        /// </summary>
        /// <param name="translationProviderUri">A translation provider URI, representing the translation provider.</param>
        /// <param name="translationProviderState">Optional translation provider state information, which can be used to determine
        /// certain aspects of the display information.</param>
        /// <returns>
        /// A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderDisplayInfo"/> object, containing display information that allows
        /// an application to represent the translation provider without having to instantiate it.
        /// </returns>
        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var settings = new TranslationProviderSettings(translationProviderUri);

            // this have to be improved by adding a readonly property to TranslationProviderSettings class which will indicate the type of TM: file or server based.
            bool isServerBased = !string.IsNullOrEmpty(settings.Container); 

            TranslationProviderDisplayInfo info = new TranslationProviderDisplayInfo()
            {
                Name = settings.TranslationMemoryName,
                TooltipText = PluginResources.Trados2007_ToolTip,
                SearchResultImage = isServerBased ?
                    PluginResources.Trados2007_ServerBased_Image_little : PluginResources.Trados2007_FileBased_Image_little,
                TranslationProviderIcon = isServerBased ? 
                    PluginResources.Trados2007_ServerBased_Icon : PluginResources.Trados2007_FileBased_Icon
            };

            return info;
        }

        /// <summary>
        /// Gets the credentials from the user and puts these credentials in the credential store.
        /// </summary>
        /// <param name="owner">The window that will own the dialog</param>
        /// <param name="translationProviderUri">translation provider uri</param>
        /// <param name="translationProviderState">translation provider state</param>
        /// <param name="credentialStore">credential store</param>
        /// <returns>
        /// true if the user provided credentials or false if the user canceled
        /// </returns>
        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            //var settings = new TranslationProviderSettings(translationProviderUri);

            //AddEditServerDialog dialog = new AddEditServerDialog(settings.Path);

            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    var account = dialog.ServerAccount;

            //    var credentials =
            //        CredentialsUtility.CreateTranslationProviderCredential(account.Login, account.Password);

            //    credentialStore.AddCredential(settings.ConnectionPointUri, credentials);

            //    return true;
            //}

            return false;
        }

        /// <summary>
        /// Displays a dialog to interactively change any of the translation provider settings.
        /// </summary>
        /// <param name="owner">The window that will own the dialog</param>
        /// <param name="translationProvider">A translation provider descriptor, representing the translation provider to edit.</param>
        /// <param name="languagePairs">A collection of language pairs. If provided, the list of available translation providers will be filtered by these language directions.</param>
        /// <param name="credentialStore">A credential store object that can be used to retrieve credentials required.</param>
        /// <returns>
        /// True if changes were made to the translation provider; false otherwise.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown when calling this method while <see cref="P:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderWinFormsUI.SupportsEditing"/> return <code>false</code>.</exception>
        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            return false;
        }

        /// <summary>
        /// Displays a dialog to interactively browse for one or more translation providers.
        /// </summary>
        /// <param name="owner">The window that will own the dialog</param>
        /// <param name="languagePairs">A collection of language pairs. If provided, the list of available translation providers will be filtered by these language directions.</param>
        /// <param name="credentialStore">A credential store object that can be used to retrieve credentials required.</param>
        /// <returns>
        /// A collection of translation providers selected by the user, or <c>null</c> if none were selected or available or the browse was cancelled.
        /// </returns>
        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            MainDialog dialog = new MainDialog(languagePairs, credentialStore);
            var result = dialog.ShowDialog(owner);

            if (result == DialogResult.OK)
            {
                return new ITranslationProvider[] { dialog.SelectedTranslationProvider };
            }

            return null;
        }

        #endregion // Methods
    }
}
