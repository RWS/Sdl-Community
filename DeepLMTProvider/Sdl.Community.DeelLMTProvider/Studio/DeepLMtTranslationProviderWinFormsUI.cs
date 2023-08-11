using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using Sdl.Community.DeepLMTProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

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

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var options = new DeepLTranslationOptions();

            //get credentials
            var credentials = GetCredentials(credentialStore, PluginResources.DeeplTranslationProviderScheme);

            var viewModel = new DeepLWindowViewModel(options, credentials, languagePairs, new DeepLGlossaryClient());
            var dialog = new DeepLWindow(viewModel);

            ElementHost.EnableModelessKeyboardInterop(dialog);
            dialog.ShowDialog();

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value) return null;

            var provider = new DeepLMtTranslationProvider(options, new DeepLTranslationProviderClient(options.ApiKey), languagePairs)
            {
	            Options = viewModel.Options
            };
            var apiKey = viewModel.Options.ApiKey;
            SetDeeplCredentials(credentialStore, apiKey, true);

            return new ITranslationProvider[] { provider };
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var editProvider = translationProvider as DeepLMtTranslationProvider;

            if (editProvider == null)
            {
                return false;
            }

            //get saved key if there is one and put it into options
            var savedCredentials = GetCredentials(credentialStore, PluginResources.DeeplTranslationProviderScheme);
            if (savedCredentials != null)
            {
                editProvider.Options.ApiKey = savedCredentials.Credential;
            }

            var viewModel = new DeepLWindowViewModel(editProvider.Options, savedCredentials, languagePairs, new DeepLGlossaryClient());
			var dialog = new DeepLWindow(viewModel);

            ElementHost.EnableModelessKeyboardInterop(dialog);
            dialog.ShowDialog();

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value) return false;

            var apiKey = editProvider.Options.ApiKey;
            SetDeeplCredentials(credentialStore, apiKey, true);

            return true;
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            throw new NotImplementedException();
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo
            {
                Name = "DeepL Translation provider",
                TooltipText = "DeepL Translation provider",
                //SearchResultImage = PluginResources.DeepL,
                TranslationProviderIcon = PluginResources.deepLIcon
            };
            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(nameof(translationProviderUri));
            }

            var supportsProvider = string.Equals(translationProviderUri.Scheme, DeepLMtTranslationProvider.ListTranslationProviderScheme,
                StringComparison.OrdinalIgnoreCase);
            return supportsProvider;
        }

        private TranslationProviderCredential GetCredentials(ITranslationProviderCredentialStore credentialStore, string uri)
        {
            var providerUri = new Uri(uri);
            TranslationProviderCredential cred = null;

            if (credentialStore.GetCredential(providerUri) != null)
            {
                //get the credential to return
                cred = new TranslationProviderCredential(credentialStore.GetCredential(providerUri).Credential, credentialStore.GetCredential(providerUri).Persist);
            }

            return cred;
        }

        private void SetDeeplCredentials(ITranslationProviderCredentialStore credentialStore, string apiKey, bool persistKey)
        {
            //used to set credentials
            // we are only setting and getting credentials for the uri with no parameters...kind of like a master credential
            var uri = new Uri(PluginResources.DeeplTranslationProviderScheme);
            var credentials = new TranslationProviderCredential(apiKey, persistKey);
            credentialStore.RemoveCredential(uri);
            credentialStore.AddCredential(uri, credentials);
        }
    }
}