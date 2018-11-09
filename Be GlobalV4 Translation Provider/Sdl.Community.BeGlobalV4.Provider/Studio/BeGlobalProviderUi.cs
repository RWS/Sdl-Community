using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderWinFormsUi(
		Id = "BeGlobalProviderUi",
		Name = "BeGlobalProviderUi",
		Description = "BeGlobal4 Translation Provider")]
	public class BeGlobalProviderUi  : ITranslationProviderWinFormsUI
	{
		public string TypeName => "BeGlobal4 MT Translation Provider"; 
		public string TypeDescription => "BeGlobal4 MT Translation Provider";
		public bool SupportsEditing => true;

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			var options = new BeGlobalTranslationOptions();
			var credentials = GetCredentials(credentialStore, "beglobaltranslationprovider:///");
			var beGlobalVm = new BeGlobalWindowViewModel();
			var beGlobalWindow = new BeGlobalWindow
			{
				DataContext = beGlobalVm
			};

			beGlobalWindow.ShowDialog();

			//we need to add more code here
			return null;
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

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			throw new NotImplementedException();
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			throw new NotImplementedException();
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}  

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		
	}
}
