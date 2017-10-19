using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using System.Windows.Forms;
using Sdl.Community.DeelLMTProvider;

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
			var getCredGt = GetCredentials(credentialStore, "deeplprovider:///");
			var dialog = new DeepLMtDialog(options);
			if (dialog.ShowDialog(owner) == DialogResult.OK)
			{
				var provider = new DeepLMtTranslationProvider(options);
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
            throw new NotImplementedException();
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            throw new NotImplementedException();
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo();
            info.Name = "DeepL Translation provider";
            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return true;
        }
    }
}
