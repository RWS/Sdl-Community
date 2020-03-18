using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[TranslationProviderWinFormsUi(
		Id = "SDLMachineTranslationCloudProviderUi",
		Name = "SDLMachineTranslationCloudProviderUi",
		Description = "SDL Machine Translation Cloud Provider")]
	public class BeGlobalProviderUi : ITranslationProviderWinFormsUI
	{
		
		public string TypeName => Constants.PluginName;
		public string TypeDescription => Constants.PluginName;
		public bool SupportsEditing => true;
		public static readonly Log Log = Log.Instance;

		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var options = new BeGlobalTranslationOptions();
				var credentials = SplitCredentials(credentialStore, options);
				var beGlobalWindow = new BeGlobalWindow();
				var languages = new Languages.Provider.Languages();


				var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow, options, credentials, languagePairs, languages);


				beGlobalWindow.DataContext = beGlobalVm;

				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					var clientId = beGlobalVm.Options.ClientId;
					var clientSecret = beGlobalVm.Options.ClientSecret;
					var resendDraft = beGlobalVm.Options.ResendDrafts;

					var provider = new BeGlobalTranslationProvider(options)
					{
						Options = beGlobalVm.Options
					};

					SetCredentials(credentialStore, clientId, clientSecret, resendDraft, true);
					return new ITranslationProvider[] { provider };
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.Browse} {e.Message}\n {e.StackTrace}");
			}
			return null;
		}
		
        [STAThread]
		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				if (!(translationProvider is BeGlobalTranslationProvider editProvider))
				{
					return false;
				}

				//get saved key if there is one and put it into options
				var credentials = SplitCredentials(credentialStore, editProvider.Options);
				var beGlobalWindow = new BeGlobalWindow();
				var languages = new Languages.Provider.Languages();

				var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow, editProvider.Options, credentials, languagePairs, languages);


				beGlobalWindow.DataContext = beGlobalVm;

				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					editProvider.Options = beGlobalVm.Options;
					var clientId = editProvider.Options.ClientId;
					var clientSecret = beGlobalVm.Options.ClientSecret;
					var resendDraft = beGlobalVm.Options.ResendDrafts;
					SetCredentials(credentialStore, clientId, clientSecret, resendDraft, true);
					return true;
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.EditWindow} {e.Message}\n {e.StackTrace}");
			}
			return false;
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
				Name = Constants.PluginName,
				TooltipText = Constants.PluginName,
				TranslationProviderIcon = PluginResources.global,
				SearchResultImage = PluginResources.global1,
			};
			return info;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState,
			ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		private TranslationProviderCredential GetCredentials(ITranslationProviderCredentialStore credentialStore, string uri)
		{
			var providerUri = new Uri(uri);
			TranslationProviderCredential cred = null;
			var credential = credentialStore.GetCredential(providerUri);
			if (credential != null)
			{
				//get the credential to return				
				cred = new TranslationProviderCredential(credential?.Credential, credential.Persist);
			}
			return cred;
		}

		private void SetCredentials(ITranslationProviderCredentialStore credentialStore, string clientId, string clientSecret, bool resendDraft, bool persistKey)
		{
			var uri = new Uri("sdlmachinetranslationcloudprovider:///");
			string credential;
			
			// Validate if the entered clientId is an email address.
			// If corresponds to email standards, it means that authentication is done through User email and User password.		
			var isEmailValid = IsEmailValid(clientId);

			// Encrypt client credentials to Base64 (it is usefull when user credentials contains # char and the authentication is failing,
			// because the # char is used to differentiate the clientId by ClientSecret.
			clientId = StringExtensions.EncryptData(clientId);
			clientSecret = StringExtensions.EncryptData(clientSecret);

			if (isEmailValid)
			{
				credential = $"{clientId}#{clientSecret}#UserLogin";
			}
			else
			{
				credential = $"{clientId}#{clientSecret}#ClientLogin";
			}
			credential = $"{credential}#{resendDraft}";

			var credentials = new TranslationProviderCredential(credential, persistKey);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, credentials);
		}

		/// <summary>
		/// Validate the user input: it might be Email or ClientId.
		/// Based on this validation the authentication method is saved in the CredentialStore.
		/// </summary>
		/// <param name="input">Email or ClientId</param>
		/// <returns></returns>
		private bool IsEmailValid(string input)
		{
			try
			{
				return new EmailAddressAttribute().IsValid(input);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.IsEmailValid} {ex.Message}\n {ex.StackTrace}");
				return false;
			}
		}

		private TranslationProviderCredential SplitCredentials(ITranslationProviderCredentialStore credentialStore, BeGlobalTranslationOptions options)
		{
			var savedCredentials = GetCredentials(credentialStore, "sdlmachinetranslationcloudprovider:///");
			if (savedCredentials != null)
			{
				var splitedCredentials = savedCredentials?.Credential?.Split('#');
				options.ClientId = splitedCredentials?.Length > 2? StringExtensions.Decrypt(splitedCredentials[0]) : string.Empty;
				options.ClientSecret = splitedCredentials?.Length > 2 ? StringExtensions.Decrypt(splitedCredentials[1]) : string.Empty;
				options.AuthenticationMethod = splitedCredentials?.Length == 4 ? splitedCredentials[2] : string.Empty;
				var resendDraft = splitedCredentials?.Length == 4 ? splitedCredentials[3] : string.Empty;
				if (!string.IsNullOrEmpty(resendDraft))
				{
					options.ResendDrafts = resendDraft.Equals("True") ? true : false;
				}
			}
			return savedCredentials;
		}
    }
}