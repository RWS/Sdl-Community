using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.RecordSourceTU
{
    [TranslationProviderWinFormsUi(
        Id = "Sdl.Community.RecordSourceTUWinFormsUI",
        Name = "Sdl.Community.RecordSourceTUWinFormsUI",
        Description = "Sdl.Community.RecordSourceTUWinFormsUI")]
    public class RecordSourceTuTranslationProviderWinFormsUi : ITranslationProviderWinFormsUI
    {
        private ITranslationProviderWinFormsUI FileBasedTranslationProviderUi
        {
            get
            {
                return TranslationProviderManager.GetTranslationProviderWinFormsUI(new Uri("sdltm.file://test.sdltm"));
            }
        }

        #region ITranslationProviderWinFormsUI Members

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var translationProviders = FileBasedTranslationProviderUi.Browse(owner, languagePairs, credentialStore);
            if (translationProviders.Length == 0) return null;
            var translationProvider = translationProviders[0];
            using (var sourceConfigurationForm = new SourceTmConfiguration(translationProvider.Uri))
            {
                sourceConfigurationForm.ShowDialog();
            }
            

            return new ITranslationProvider[] {new RecordSourceTuTmTranslationProvider(translationProvider)};
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var tProvider = translationProvider as RecordSourceTuTmTranslationProvider;
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
            displayInfo.Name = "Record Source TU: " + displayInfo.Name;
            displayInfo.TooltipText = "Record Source TU: " + displayInfo.TooltipText;
            displayInfo.TranslationProviderIcon = PluginResources.Approved;
            return displayInfo;
        }

        public bool SupportsEditing
        {
            get { return FileBasedTranslationProviderUi.SupportsEditing; }
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.StartsWith(RecordSourceTuTmTranslationProvider.ProviderUriScheme,
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
            get { return "Record Source TU ..."; }
        }

        #endregion
    }
}
