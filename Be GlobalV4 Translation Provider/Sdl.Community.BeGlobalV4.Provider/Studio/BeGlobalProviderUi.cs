using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
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
			
			var beGlobalWindow = new BeGlobalWindow();
			var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow,options, credentials);
			beGlobalWindow.DataContext = beGlobalVm;

			beGlobalWindow.ShowDialog();
			if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
			{
				var provider = new BeGlobalTranslationProvider(options)
				{
					Options = beGlobalVm.Options
				};
				var clientId = beGlobalVm.Options.ClientId;
				var clientSecret = beGlobalVm.Options.ClientSecret;
				SetBeGlobalCredentials(credentialStore, clientId,clientSecret, true);
				return new ITranslationProvider[] { provider };
			}
			return null;
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			var editProvider = translationProvider as BeGlobalTranslationProvider;

			if (editProvider == null)
			{
				return false;
			}

			//get saved key if there is one and put it into options
			var savedCredentials = GetCredentials(credentialStore, "beglobaltranslationprovider:///");
			if (savedCredentials != null)
			{
				var splitedCredentials = savedCredentials.Credential.Split('#');
				var clientId = splitedCredentials[0];
				var clientSecret = splitedCredentials[1];

				editProvider.Options.ClientId = clientId;
				editProvider.Options.ClientSecret = clientSecret;
			}
			var beGlobalWindow = new BeGlobalWindow();
			var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow,editProvider.Options,savedCredentials);
			beGlobalWindow.DataContext = beGlobalVm;
			
			beGlobalWindow.ShowDialog();
			if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
			{
				editProvider.Options = beGlobalVm.Options;
				var clientId = editProvider.Options.ClientId;
				var clientSecret = beGlobalVm.Options.ClientSecret;	  
				SetBeGlobalCredentials(credentialStore, clientId, clientSecret, true);
				return true;
			}
			return false;
		}

		private void SetBeGlobalCredentials(ITranslationProviderCredentialStore credentialStore, string clientId, string clientSecret,bool persistKey)
		{
			var uri = new Uri("beglobaltranslationprovider:///");

			var credential = $"{clientId}#{clientSecret}";
			var credentials = new TranslationProviderCredential(credential, persistKey);
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

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			if (translationProviderUri == null)
			{
				throw new ArgumentNullException(nameof(translationProviderUri));
			}

			var supportsProvider = string.Equals(translationProviderUri.Scheme, BeGlobalTranslationProvider.ListTranslationProviderScheme,
				StringComparison.OrdinalIgnoreCase);
			return supportsProvider;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			var info = new TranslationProviderDisplayInfo
			{
				Name = "BeGlobal Translation provider",
				TooltipText = "BeGlobal Translation provider"
			};
			return info;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}  

		

		
	}
}
