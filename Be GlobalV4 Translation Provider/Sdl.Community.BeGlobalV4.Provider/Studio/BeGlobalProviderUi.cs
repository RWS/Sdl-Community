using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	[TranslationProviderWinFormsUi(
		Id = "SDLMachineTranslationCloudProviderUi",
		Name = "SDLMachineTranslationCloudProviderUi",
		Description = "SDL Machine Translation Cloud Provider")]
	public class BeGlobalProviderUi : ITranslationProviderWinFormsUI
	{
		private Constants _constants = new Constants();

		public string TypeName => _constants.PluginName;
		public string TypeDescription => _constants.PluginName;
		public bool SupportsEditing => true;
		public static readonly Log Log = Log.Instance;

		[STAThread]
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var options = new BeGlobalTranslationOptions();
				var credentials = SplitCredentials(credentialStore, options);
				if(options.LanguageMappings == null)
				{
					options.LanguageMappings = new ObservableCollection<LanguageMappingModel>();
				}
				var beGlobalWindow = new BeGlobalWindow();
				var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow, options, credentials, languagePairs);
				beGlobalWindow.DataContext = beGlobalVm;

				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					var clientId = beGlobalVm.Options.ClientId;
					var clientSecret = beGlobalVm.Options.ClientSecret;

					var provider = new BeGlobalTranslationProvider(options)
					{
						Options = beGlobalVm.Options
					};

					SetCredentials(credentialStore, clientId, clientSecret, true);
					return new ITranslationProvider[] { provider };
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{_constants.Browse} {e.Message}\n {e.StackTrace}");
			}
			return null;
		}
		
        [STAThread]
		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs,
			ITranslationProviderCredentialStore credentialStore)
		{
			try
			{
				var editProvider = translationProvider as BeGlobalTranslationProvider;

				if (editProvider == null)
				{
					return false;
				}

				//get saved key if there is one and put it into options
				var credentials = SplitCredentials(credentialStore, editProvider.Options);
				var beGlobalWindow = new BeGlobalWindow();
				var beGlobalVm = new BeGlobalWindowViewModel(beGlobalWindow, editProvider.Options, credentials, languagePairs);
				beGlobalWindow.DataContext = beGlobalVm;

				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					editProvider.Options = beGlobalVm.Options;
					var clientId = editProvider.Options.ClientId;
					var clientSecret = beGlobalVm.Options.ClientSecret;
					SetCredentials(credentialStore, clientId, clientSecret, true);
					return true;
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{_constants.EditWindow} {e.Message}\n {e.StackTrace}");
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
				Name = _constants.PluginName,
				TooltipText = _constants.PluginName,
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
			if (credentialStore.GetCredential(providerUri) != null)
			{
				//get the credential to return				
				cred = new TranslationProviderCredential(credentialStore.GetCredential(providerUri)?.Credential, credentialStore.GetCredential(providerUri).Persist);
			}
			return cred;
		}

		private void SetCredentials(ITranslationProviderCredentialStore credentialStore, string clientId, string clientSecret, bool persistKey)
		{
			var uri = new Uri("sdlmachinetranslationcloudprovider:///");
			string credential;
			
			// Validate if the entered clientId is an email address.
			// If corresponds to email standards, it means that authentication is done through User email and User password.		
			var isEmailValid = IsEmailValid(clientId);

			// Encode client credentials to Base64 (it is usefull when user credentials contains # char and the authentication is failing,
			// because the # char is used to differentiate the clientId by ClientSecret.
			clientId = StringExtensions.Base64Encode(clientId);
			clientSecret = StringExtensions.Base64Encode(clientSecret);

			if (isEmailValid)
			{
				credential = $"{clientId}#{clientSecret}#UserLogin";
			}
			else
			{
				credential = $"{clientId}#{clientSecret}#ClientLogin";
			}
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
				Log.Logger.Error($"{_constants.IsEmailValid} {ex.Message}\n {ex.StackTrace}");
				return false;
			}
		}

		private TranslationProviderCredential SplitCredentials(ITranslationProviderCredentialStore credentialStore, BeGlobalTranslationOptions options)
		{
			var savedCredentials = GetCredentials(credentialStore, "sdlmachinetranslationcloudprovider:///");
			if (savedCredentials != null)
			{
				var splitedCredentials = savedCredentials.Credential.Split('#');
				options.ClientId = splitedCredentials.Length> 2? StringExtensions.Base64Decode(splitedCredentials[0]) : string.Empty;
				options.ClientSecret = splitedCredentials.Length > 2 ? StringExtensions.Base64Decode(splitedCredentials[1]) : string.Empty;
				options.AuthenticationMethod = splitedCredentials.Length == 3 ? splitedCredentials[2] : string.Empty;
			}
			return savedCredentials;
		}
    }
}