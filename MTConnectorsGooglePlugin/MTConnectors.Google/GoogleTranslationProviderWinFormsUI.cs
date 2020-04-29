using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Windows.Forms;
using Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider;
using Sdl.LanguagePlatform.MTConnectors.Google.UI;

namespace Sdl.LanguagePlatform.MTConnectors.Google
{
    /// <summary>
    /// This class implements <see cref="ITranslationProviderWinFormsUI"/> to allow the Google plug-in to be added using the UI in SDL Trados Studio 2009.
    /// It is also marked up with the <see cref="TranslationProviderWinFormsUiAttribute"/> attribute for registration it with the plug-in manager.
    /// </summary>
    [TranslationProviderWinFormsUi]
    public class GoogleTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members

        public string TypeName
        {
            get { return PluginResources.PluginProviderGoogleTranslatorFactory_TypeName; }
        }

        public string TypeDescription
        {
            get { return PluginResources.PluginProviderGoogleTranslatorFactory_TypeDescription; }
        }

        public bool SupportsEditing
        {
            get { return true; }
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("translationProviderUri");
            }

            string[] splitScheme = translationProviderUri.Scheme.Split('.');
            if (splitScheme.Length == 0)
            {
                return false;
            }

            return string.Equals(splitScheme[0], GoogleTranslationProvider.GetUriScheme, StringComparison.OrdinalIgnoreCase);
        }

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            DlgBrowse dlg = new DlgBrowse(new Settings(), credentialStore, DlgBrowse.DialogMode.Create);

            if (dlg.ShowDialog(owner) == DialogResult.OK)
            {
                Settings settings = dlg.GetSettings();
                if (settings == null)
                {
                    return null;
                }

                var googleProvider = new GoogleTranslationProviderPlugin(settings);

                // Store API Key in main credential store
                if (!string.IsNullOrEmpty(settings.ApiKey))
                {
                    credentialStore.AddCredential(settings.CredentialsUri, new TranslationProviderCredential(settings.ApiKey, true));
                }

                return new ITranslationProvider[] { googleProvider };
            }

            return null;
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var googleTranslationProvider = translationProvider as GoogleTranslationProviderPlugin;

            if (googleTranslationProvider != null)
            {
                var oldSettings = googleTranslationProvider.Settings;
                DlgBrowse dlg = new DlgBrowse(oldSettings, credentialStore, DlgBrowse.DialogMode.Edit);

                if (dlg.ShowDialog(owner) == DialogResult.OK)
                {
                    Settings googleSettings = dlg.GetSettings();
                    if (googleSettings == null)
                    {
                        return false;
                    }

                    googleTranslationProvider.UpdateSettings(googleSettings);

                    // Store API Key in main credential store
                    if (!string.IsNullOrEmpty(googleSettings.ApiKey))
                    {
                        credentialStore.AddCredential(googleSettings.CredentialsUri, new TranslationProviderCredential(googleSettings.ApiKey, true));
                    }

                    return true;
                }
            }

            return false;
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var translationProviderDisplayInfo = new TranslationProviderDisplayInfo();
            if (string.IsNullOrEmpty(translationProviderUri.UserInfo))
            {
                translationProviderDisplayInfo.Name = GoogleTranslationProvider.GetTranslationProviderName();
            }
            else
            {
                translationProviderDisplayInfo.Name = string.Concat(
                    GoogleTranslationProvider.GetTranslationProviderName(),
                    " (",
                    translationProviderUri.UserInfo.Substring(0, translationProviderUri.UserInfo.Length - 1),
                    ")");
            }

            translationProviderDisplayInfo.SearchResultImage = PluginResources.google_alpha_studio;

            // TODO: _translationProviderDisplayInfo.TooltipText = ???
            translationProviderDisplayInfo.TranslationProviderIcon = PluginResources.favicon;
            return translationProviderDisplayInfo;
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var translationProviderSettings = new Settings(translationProviderUri);

            var dlg = new DlgBrowse(translationProviderSettings, credentialStore, DlgBrowse.DialogMode.Credentials);

            if (dlg.ShowDialog(owner) == DialogResult.OK)
            {
                Settings googleSettings = dlg.GetSettings();
                if (googleSettings == null)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(googleSettings.ApiKey))
                {
                    credentialStore.AddCredential(googleSettings.CredentialsUri, new TranslationProviderCredential(googleSettings.ApiKey, true));
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
