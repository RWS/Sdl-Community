using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;
using Sdl.Community.DeepLMTProvider.UI;
using Sdl.Community.DeepLMTProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Sdl.Community.DeepLMTProvider.Studio
{
    [TranslationProviderWinFormsUi(
        Id = "DeepLMtTranslationProviderWinFormsUI",
        Name = "DeepLMtTranslationProviderWinFormsUI",
        Description = "DeepLMtTranslationProviderWinFormsUI")]
    public class DeepLMtTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        public bool SupportsEditing => true;
        public string TypeDescription => "DeepL MT Translation Provider";
        public string TypeName => "DeepL MT Translation Provider";

        private DeepLWindowViewModel DeepLWindowViewModel { get; set; }

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var options = new DeepLTranslationOptions();

            var credentials = credentialStore.GetCredentials(PluginResources.DeeplTranslationProviderScheme);

            var dialog = SetupDeepL(languagePairs, options, credentials);

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
                return null;

            var provider = new DeepLMtTranslationProvider(options, new DeepLTranslationProviderClient(options.ApiKey),
                languagePairs);

            var apiKey = DeepLWindowViewModel.Options.ApiKey;
            credentialStore.SetDeeplCredentials(apiKey, true);

            return [provider];
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var editProvider = translationProvider as DeepLMtTranslationProvider;
            if (editProvider == null) return false;

            var savedCredentials = credentialStore.GetCredentials(PluginResources.DeeplTranslationProviderScheme);
            if (savedCredentials != null) editProvider.Options.ApiKey = savedCredentials.Credential;

            var dialog = SetupDeepL(languagePairs, editProvider.Options, savedCredentials);

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
                return false;

            var apiKey = editProvider.Options.ApiKey;
            credentialStore.SetDeeplCredentials(apiKey, true);

            return true;
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri,
            string translationProviderState, ITranslationProviderCredentialStore credentialStore) =>
            throw new NotImplementedException();

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo
            {
                Name = "DeepL Translation provider",
                TooltipText = "DeepL Translation provider",
                TranslationProviderIcon = PluginResources.deepLIcon
            };
            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null) throw new ArgumentNullException(nameof(translationProviderUri));

            var supportsProvider = string.Equals(translationProviderUri.Scheme, DeepLMtTranslationProvider.ListTranslationProviderScheme,
                StringComparison.OrdinalIgnoreCase);
            return supportsProvider;
        }

        private DeepLWindow SetupDeepL(LanguagePair[] languagePairs, DeepLTranslationOptions options,
                                                            TranslationProviderCredential credentials)
        {
            var deepLGlossaryClient = new DeepLGlossaryClient();
            DeepLWindowViewModel = new DeepLWindowViewModel(options, deepLGlossaryClient, credentials, languagePairs, new MessageService());
            var dialog = new DeepLWindow(DeepLWindowViewModel);

            DeepLWindowViewModel.ManageGlossaries += ViewModel_ManageGlossaries;
            GlossaryBackupService.DeepLGlossaryClient = deepLGlossaryClient;

            ElementHost.EnableModelessKeyboardInterop(dialog);
            dialog.ShowDialog();

            DeepLWindowViewModel.ManageGlossaries -= ViewModel_ManageGlossaries;
            return dialog;
        }

        private void ViewModel_ManageGlossaries()
        {
            GlossaryBackupService.ViewModel_ManageGlossaries();
            DeepLWindowViewModel.LoadLanguagePairSettings();
        }
    }
}