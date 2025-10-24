using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Windows.Forms;

namespace Trados_AI_Essentials.Studio
{
    [TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
                                   Name = "Translation_Provider_Plug_inWinFormsUI",
                                   Description = "Translation_Provider_Plug_inWinFormsUI")]
    internal class TranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members

        public bool SupportsEditing => true;

        public string TypeDescription
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeName => "Trados AI Essentials";

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs,
            ITranslationProviderCredentialStore credentialStore) =>
            new[] { StudioIntegration.GetTranslationProviderFromUserSettings() };

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore) => StudioIntegration.EditTranslationProvider();

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            throw new NotImplementedException();
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo
            {
                Name = PluginResources.Plugin_Name,
                TooltipText = PluginResources.Plugin_Name,
                TranslationProviderIcon = PluginResources.Logo
            };
            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri) => true;

        #endregion ITranslationProviderWinFormsUI Members
    }
}