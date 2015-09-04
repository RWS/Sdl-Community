using System;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.AddSourceTM.Source_Configurtion;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AddSourceTM
{
    [TranslationProviderWinFormsUi(
        Id = "Sdl.Community.AddSourceTMWinFormsUI",
        Name = "Sdl.Community.AddSourceTMWinFormsUI",
        Description = "Sdl.Community.AddSourceTMWinFormsUI")]
    public class AddSourceTmTranslationProviderWinFormsUi : ITranslationProviderWinFormsUI
    {
        private ITranslationProviderWinFormsUI FileBasedTranslationProviderUi
        {
            get
            {
                return TranslationProviderManager.GetTranslationProviderWinFormsUIs().First(x => x.TypeName.Equals("File-based translation memory"));
            }
        }

        #region ITranslationProviderWinFormsUI Members

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var translationProviders = FileBasedTranslationProviderUi.Browse(owner, languagePairs, credentialStore);
            var translationProvider = translationProviders[0];
            using (var sourceConfigurationForm = new SourceTmConfiguration(translationProvider.Uri))
            {
                sourceConfigurationForm.ShowDialog();
            }
            

            return new ITranslationProvider[] {new AddSourceTmTranslationProvider(translationProvider)};
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var tProvider = translationProvider as AddSourceTmTranslationProvider;
            bool result = false;
            if (tProvider != null)
            {
                result = FileBasedTranslationProviderUi.Edit(owner, tProvider.FileBasedTranslationProvider, languagePairs, credentialStore);
            }

            using (var sourceConfigurationForm = new SourceTmConfiguration(translationProvider.Uri))
            {
                sourceConfigurationForm.ShowDialog();
            }

            return result;
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return false;
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            
            var displayInfo = FileBasedTranslationProviderUi.GetDisplayInfo(translationProviderUri.GetInnerProviderUri(), 
                translationProviderState);
            displayInfo.Name = "AddSourceTm: " + displayInfo.Name;
            displayInfo.TooltipText = "AddSourceTm: " + displayInfo.TooltipText;
            return displayInfo;
        }

        public bool SupportsEditing
        {
            get { return FileBasedTranslationProviderUi.SupportsEditing; }
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.StartsWith(AddSourceTmTranslationProvider.ProviderUriScheme,
                StringComparison.InvariantCultureIgnoreCase)
                   &&
                   FileBasedTranslationProviderUi.SupportsTranslationProviderUri(translationProviderUri.GetInnerProviderUri());
        }

        public string TypeDescription
        {
            get { return "Translation provider that will create or update \"Source File\" custom filed of the TM"; }
        }

        public string TypeName
        {
            get { return "AddSourceTM ..."; }
        }

        #endregion
    }
}
