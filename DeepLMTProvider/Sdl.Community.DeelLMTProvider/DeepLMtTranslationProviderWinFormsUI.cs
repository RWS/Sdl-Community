using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using Sdl.LanguagePlatform.Core;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Sdl.Community.DeepLMTProvider.WPF;
using Sdl.Community.DeepLMTProvider.WPF.Model;

namespace Sdl.Community.DeepLMTProvider
{
    [TranslationProviderWinFormsUi(
        Id = "DeepLMtTranslationProviderWinFormsUI",
        Name = "DeepLMtTranslationProviderWinFormsUI",
        Description = "DeepLMtTranslationProviderWinFormsUI")]
    public class DeepLMtTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        public string TypeName => "DeepL MT Translation Provider";

        public string TypeDescription => "DeepL MT Translation Provider";

        public bool SupportsEditing => true;

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
			var options = new DeepLTranslationOptions();

			//get credentials
			var credentials = GetCredentials(credentialStore, "deeplprovider:///");

			var dialog = new DeepLWindow(options, credentials,languagePairs);
			ElementHost.EnableModelessKeyboardInterop(dialog);
			dialog.ShowDialog();
			if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
			{
				var provider = new DeepLMtTranslationProvider(options)
				{
					Options = dialog.Options
				};
				var apiKey = dialog.Options.ApiKey;
				SetDeeplCredentials(credentialStore, apiKey, true);
				return new ITranslationProvider[] { provider };
			}
			return null;
        }

		private void SetDeeplCredentials(ITranslationProviderCredentialStore credentialStore, string apiKey, bool persistKey)
		{
			//used to set credentials
			// we are only setting and getting credentials for the uri with no parameters...kind of like a master credential
			var uri = new Uri("deeplprovider:///");
			var credentials = new TranslationProviderCredential(apiKey, persistKey);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, credentials);
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

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var editProvider = translationProvider as DeepLMtTranslationProvider;

			if (editProvider == null)
			{
				return false;
			}

			//get saved key if there is one and put it into options
			var savedCredentials = GetCredentials(credentialStore, "deeplprovider:///");
			if (savedCredentials != null)
			{
				editProvider.Options.ApiKey = savedCredentials.Credential;
			}

			var dialog = new DeepLWindow(editProvider.Options, savedCredentials,languagePairs);
			ElementHost.EnableModelessKeyboardInterop(dialog);
			dialog.ShowDialog();
			if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
			{
				editProvider.Options = dialog.Options;
				var apiKey = editProvider.Options.ApiKey;
				SetDeeplCredentials(credentialStore, apiKey, true);
				return true;
			}
			return false;
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
				TranslationProviderIcon = PluginResources.deepLIcon
				//SearchResultImage = PluginResources.DeepL
			};
			return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
			if (translationProviderUri == null)
	        {
		        throw new ArgumentNullException(nameof(translationProviderUri));
	        }

	        var supportsProvider= string.Equals(translationProviderUri.Scheme, DeepLMtTranslationProvider.ListTranslationProviderScheme,
		        StringComparison.OrdinalIgnoreCase);
	        return supportsProvider;
        }
	}
}
