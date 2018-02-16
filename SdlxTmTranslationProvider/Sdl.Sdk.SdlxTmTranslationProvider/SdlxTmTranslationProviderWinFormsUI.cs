//-----------------------------------------------------------------------
// <copyright file="SdlxTmTranslationProviderWinFormsUI.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;
    using System.IO;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// The UI for opening SDLX translation memories which will allow the user to select different TM types
    /// </summary>
    [TranslationProviderWinFormsUi(
        Id = "Sdl.Sdk.SdlxTmTranslationProviderWinFormsUI",
        Name = "Sdl.Sdk.SdlxTmTranslationProviderWinFormsUI",
        Description = "Sdl.Sdk.SdlxTmTranslationProviderWinFormsUI")]
    public class SdlxTmTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members

        /// <summary>
        /// Gets a value indicating whether this provider supports editing.
        /// </summary>
        /// <value>Always false.</value>
        public bool SupportsEditing
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the type description.
        /// </summary>
        /// <value>The type description.</value>
        public string TypeDescription
        {
            get { return PluginResources.PluginProviderSdlxTm_TypeDescription; }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName
        {
            get { return PluginResources.PluginProviderSdlxTm_TypeName; }
        }

        /// <summary>
        /// A method to browse for a translation provider.
        /// </summary>
        /// <param name="owner">The handle of the requesting window.</param>
        /// <param name="languagePairs">The language pairs.</param>
        /// <param name="credentialStore">The credential store.</param>
        /// <returns>An array of translation providers or null.</returns>
        public ITranslationProvider[] Browse(System.Windows.Forms.IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            string translationMemoryType = string.Empty;
            string translationMemoryConnectionString = string.Empty;
            int translationMemoryReference = 0;
            int translationMemoryFlags = 0;

            using (SdlxTmTranslationProviderUiForm myForm = new SdlxTmTranslationProviderUiForm())
            {
                if (myForm.ShowDialog(owner) == System.Windows.Forms.DialogResult.Cancel)
                {
                    return null;
                }

                translationMemoryType = myForm.TranslationMemoryType;
            }

            int sdlxResult = SdlxTmTranslationProvider.TmController.Open(ref translationMemoryConnectionString, ref translationMemoryType, translationMemoryFlags, ref translationMemoryReference);

            if (sdlxResult != 0)
            {
                return null;
            }

            return new SdlxTmTranslationProviderPlugin[] { new SdlxTmTranslationProviderPlugin(translationMemoryType, translationMemoryConnectionString, translationMemoryReference) };
        }

        /// <summary>
        /// Edits the specified translation provider.
        /// </summary>
        /// <param name="owner">The handle of the requesting window.</param>
        /// <param name="translationProvider">The translation provider.</param>
        /// <param name="languagePairs">The language pairs.</param>
        /// <param name="credentialStore">The credential store.</param>
        /// <returns>Success or failure</returns>
        public bool Edit(System.Windows.Forms.IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            return false;
        }

        /// <summary>
        /// Gets the credentials from user.
        /// </summary>
        /// <param name="owner">The handle of the requesting window.</param>
        /// <param name="translationProviderUri">The translation provider URI.</param>
        /// <param name="translationProviderState">State of the translation provider.</param>
        /// <param name="credentialStore">The credential store.</param>
        /// <returns>Always true.</returns>
        public bool GetCredentialsFromUser(System.Windows.Forms.IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return true;
        }

        /// <summary>
        /// Gets the display information for a particular URI.
        /// </summary>
        /// <param name="translationProviderUri">The translation provider URI.</param>
        /// <param name="translationProviderState">State of the translation provider.</param>
        /// <returns>A <see cref="TranslationProviderDisplayInfo"/> object.</returns>
        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderDisplayInfo myInfo = new TranslationProviderDisplayInfo();
            string translationMemoryType = translationProviderUri.Host.ToUpper();
            string translationMemoryConnection = Uri.UnescapeDataString(translationProviderUri.PathAndQuery).Substring(1).Replace(@"/", @"\");
            if (translationMemoryType == "S42TM-STD")
            {
                myInfo.Name = new FileInfo(translationMemoryConnection).Name;
            }
            else
            {
                myInfo.Name = translationMemoryConnection;
            }

            myInfo.TooltipText = string.Format("{0} [{1}]", myInfo.Name, translationMemoryType.Substring(4));

            return myInfo;
        }

        /// <summary>
        /// Determines whether this UI supports the given translation provider URI.
        /// </summary>
        /// <param name="translationProviderUri">The translation provider URI.</param>
        /// <returns>True only if we are asked about "sdlxtm" URIs.</returns>
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.Equals(SdlxTmTranslationProvider.SdlxTmUriScheme);
        }

        #endregion
    }
}
