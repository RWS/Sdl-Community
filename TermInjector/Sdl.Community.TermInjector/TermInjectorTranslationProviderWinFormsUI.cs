using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TermInjector
{
    #region "Declaration"
    [TranslationProviderWinFormsUi(
        Id = "TermInjectorTranslationProviderWinFormsUI",
        Name = "TermInjectorTranslationProviderWinFormsUI",
        Description = "TermInjectorTranslationProviderWinFormsUI")]
    #endregion
    public class TermInjectorTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
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
            TermInjectorTranslationOptions defaultTranslationOptions = new TermInjectorTranslationOptions();

            //These are the default advanced settings, they can be changed in the dialog
            defaultTranslationOptions.TokenBoundaryCharacters = " .,!?\"'-;:(){}<>|";
            defaultTranslationOptions.TermAdditionSeparator = "¤";
            defaultTranslationOptions.Delimiter = "Tab";
            defaultTranslationOptions.UseBoundaryCharacters = "true";

            TermInjectorTranslationProviderConfDialog dialog = new TermInjectorTranslationProviderConfDialog(defaultTranslationOptions, owner);
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                if (dialog.Options.TMFileName == "")
                {
                    MessageBox.Show("Translation memory field was empty. TermInjector translation provider was not added, as it requires an underlying translation memory to function.");
                    return null;
                }

                if (!File.Exists(dialog.Options.TMFileName))
                {
                    MessageBox.Show("Specified translation memory does not exist", "TermInjector");
                    return null;
                }

                if (languagePairs.Length > 1)
                {
                    MessageBox.Show("TermInjector should not be used with multiple language pairs. If there are several language pairs in the project, select a language pair, select \"Use different translation providers for this language pair\" and add TermInjector as a translation provider only for the selected language pair.", "TermInjector");
                    return null;
                }

                //Assign GUID to prevent duplicate instances
                dialog.Options.InstanceGUID = Guid.NewGuid().ToString();

                TermInjectorTranslationProvider testProvider = new TermInjectorTranslationProvider(dialog.Options);

                if (!testProvider.FileTM.SupportsLanguageDirection(languagePairs[0]))
                {
                    MessageBox.Show("Selected translation memory does not support the selected language direction ("
                        + languagePairs[0] + "). TermInjector will not function unless a TM supporting the selected language direction is selected in the options", "TermInjector");
                    return null;
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
            TermInjectorTranslationProvider editProvider = translationProvider as TermInjectorTranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            string oldFileName = editProvider.Options.GlossaryFileName;
            string oldRegexFileName = editProvider.Options.RegexFileName;
            string oldMatchCase = editProvider.Options.MatchCase;
            string oldDelimiter = editProvider.Options.Delimiter;
            string oldTMName = editProvider.Options.TMFileName;

            TermInjectorTranslationProviderConfDialog dialog = new TermInjectorTranslationProviderConfDialog(editProvider.Options, editProvider, owner);
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                if (dialog.Options.TMFileName == "")
                {
                    MessageBox.Show("Translation memory field is empty. Previous translation memory will be used.");
                    return false;
                }

                //If the TM has been changed, check that the new TM exists and has the correct languages
                if (oldTMName != dialog.Options.TMFileName)
                {
                    if (!File.Exists(dialog.Options.TMFileName))
                    {
                        MessageBox.Show("Specified translation memory does not exist", "TermInjector");
                        return false;
                    }
                    var fileTM = new FileBasedTranslationMemory(dialog.Options.TMFileName);

                    if (!fileTM.SupportsLanguageDirection(languagePairs[0]))
                    {
                        MessageBox.Show("Selected translation memory does not support the selected language direction ("
                            + languagePairs[0] + "). Previous translation memory will be used", "TermInjector");
                        return false;
                    }
                    //The checks passed, so update the TM in the provider
                    editProvider.loadTM();
                }

                //If the glossary file, case matching setting, regex setting or delimiter setting is changed, the tries should be reloaded
                if (oldFileName != dialog.Options.GlossaryFileName ||
                    oldMatchCase != dialog.Options.MatchCase ||
                    oldRegexFileName != dialog.Options.RegexFileName ||
                    oldDelimiter != dialog.Options.Delimiter)
                {
                    try
                    {
                        editProvider.loadTries();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    editProvider.initializeVisitors();
                }

                //Everything is fine, so we can update the provider options
                editProvider.Options = dialog.Options;

                return true;
            }

            return false;
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
            TranslationProviderDisplayInfo info = new TranslationProviderDisplayInfo();
            info.Name = PluginResources.Plugin_NiceName;
            info.TranslationProviderIcon = PluginResources.band_aid_icon;
            info.TooltipText = PluginResources.Plugin_Tooltip;

            info.SearchResultImage = PluginResources.band_aid_symbol;

            return info;
        }
        #endregion

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("URI not supported by the plug-in.");
            }
            return String.Equals(translationProviderUri.Scheme, TermInjectorTranslationProvider.TermInjectorTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
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