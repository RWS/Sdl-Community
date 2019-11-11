using System;
using System.Windows.Forms;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
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
				var token = string.Empty;

				var credentials = GetCredentials(credentialStore, "sdlmachinetranslationcloudprovider:///");
				AppItializer.EnsureInitializer();

				var beGlobalWindow = new BeGlobalWindow();
				var languageMappingsViewModel = new LanguageMappingsViewModel(options);
                var loginViewModel = new LoginViewModel(options, credentials, languagePairs, languageMappingsViewModel);
                loginViewModel.AuthenticationSelected += (o, e) => { SetClientOptions(beGlobalWindow, loginViewModel);};
                loginViewModel.SetAuthentications();

                var beGlobalVm = new BeGlobalWindowViewModel(options, credentials, loginViewModel, languageMappingsViewModel);
                SetAuthenticationOptions(beGlobalWindow, loginViewModel, credentials);
                beGlobalVm.OkSelected += (o, e) => { CloseWindow(beGlobalWindow); };
            
				beGlobalWindow.DataContext = beGlobalVm;
				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					var messageBoxService = new MessageBoxService();
					var beGlobalService = new BeGlobalV4Translator(beGlobalVm.Options, messageBoxService, credentials);
					beGlobalVm.Options.BeGlobalService = beGlobalService;

					var provider = new BeGlobalTranslationProvider(options)
					{
						Options = beGlobalVm.Options
					};
					if (beGlobalVm?.Options?.AuthenticationMethod == Constants.APICredentials)
					{
						SetCredentials(credentialStore, beGlobalVm.Options.ClientId, beGlobalVm.Options.ClientSecret, true);
					}
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
				var editProvider = translationProvider as BeGlobalTranslationProvider;

				if (editProvider == null)
				{
					return false;
				}

				//get saved key if there is one and put it into options
				var credentials = GetCredentials(credentialStore, "sdlmachinetranslationcloudprovider:///");
				if (credentials != null)
				{
					var splitedCredentials = !string.IsNullOrEmpty(credentials.Credential) ? credentials.Credential.Split('#') : null;
					if (splitedCredentials != null && splitedCredentials.Length == 2 && !string.IsNullOrEmpty(splitedCredentials[0]) && !string.IsNullOrEmpty(splitedCredentials[1]))
					{
						var clientId = StringExtensions.Base64Decode(splitedCredentials[0]);
						var clientSecret = StringExtensions.Base64Decode(splitedCredentials[1]);

						editProvider.Options.ClientId = clientId;
						editProvider.Options.ClientSecret = clientSecret;
					}
				}
				var beGlobalWindow = new BeGlobalWindow();
                var languageMappingsViewModel = new LanguageMappingsViewModel(editProvider.Options);
                var loginViewModel = new LoginViewModel(editProvider.Options, credentials, languagePairs, languageMappingsViewModel);
                loginViewModel.AuthenticationSelected += (e, a) => { SetClientOptions(beGlobalWindow, loginViewModel); };
                loginViewModel.SetAuthentications();

				var beGlobalVm = new BeGlobalWindowViewModel(editProvider.Options, credentials, loginViewModel, languageMappingsViewModel);
                SetAuthenticationOptions(beGlobalWindow, loginViewModel, credentials);
                beGlobalVm.OkSelected += (o, e) => { CloseWindow(beGlobalWindow); };

                beGlobalWindow.DataContext = beGlobalVm;

				beGlobalWindow.ShowDialog();
				if (beGlobalWindow.DialogResult.HasValue && beGlobalWindow.DialogResult.Value)
				{
					editProvider.Options = beGlobalVm.Options;
					if (beGlobalVm.Options?.AuthenticationMethod == Constants.APICredentials)
					{
						editProvider.Options.ClientId = beGlobalWindow.LoginTab.ClientIdBox.Password.TrimEnd().TrimStart();
						editProvider.Options.ClientSecret = beGlobalWindow.LoginTab.ClientSecretBox.Password.TrimEnd().TrimStart();
						SetCredentials(credentialStore, editProvider.Options.ClientId, beGlobalVm.Options.ClientSecret, true);
					}
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

			// Encode client credentials to Base64 (it is usefull when user credentials contains # char and the authentication is failing,
			// because the # char is used to differentiate the clientId by ClientSecret.
			clientId = StringExtensions.Base64Encode(clientId);
			clientSecret = StringExtensions.Base64Encode(clientSecret);

			var credential = $"{clientId}#{clientSecret}";
			var credentials = new TranslationProviderCredential(credential, persistKey);
			credentialStore.RemoveCredential(uri);
			credentialStore.AddCredential(uri, credentials);
		}

        private void SetClientOptions(BeGlobalWindow beGlobalWindow, LoginViewModel loginViewModel)
        {
            if (!string.IsNullOrEmpty(loginViewModel.Options.ClientId) && !string.IsNullOrEmpty(loginViewModel.Options.ClientSecret))
            {
                beGlobalWindow.LoginTab.ClientIdBox.Password = loginViewModel.Options.ClientId;
                beGlobalWindow.LoginTab.ClientSecretBox.Password = loginViewModel.Options.ClientSecret;
            }
            loginViewModel.Options.AuthenticationMethod = loginViewModel.SelectedAuthentication?.DisplayName;
        }

        private void SetAuthenticationOptions(BeGlobalWindow beGlobalWindow, LoginViewModel loginViewModel, TranslationProviderCredential credentials)
        {
            if (string.IsNullOrEmpty(loginViewModel.Options.AuthenticationMethod)) return;
            if (credentials == null) return;

            if (loginViewModel.Options.AuthenticationMethod.Equals(Constants.APICredentials))
            {
                if (!string.IsNullOrEmpty(loginViewModel.Options.ClientId) && !string.IsNullOrEmpty(loginViewModel.Options.ClientSecret))
                {
                    beGlobalWindow.LoginTab.ClientIdBox.Password = loginViewModel.Options.ClientId;
                    beGlobalWindow.LoginTab.ClientSecretBox.Password = loginViewModel.Options.ClientSecret;
                }
                else
                {
                    var allCredentials = !string.IsNullOrEmpty(credentials.Credential) ? credentials.Credential.Replace("sdlmachinetranslationcloudprovider:///", string.Empty) : string.Empty;
                    if (allCredentials.Contains("#"))
                    {
                        var splitedCredentials = allCredentials.Split('#');
                        if (splitedCredentials.Length != 2 || string.IsNullOrEmpty(splitedCredentials[0]) || string.IsNullOrEmpty(splitedCredentials[1]))
                        { 
							return;
						}
                        else
                        {
                            var clientId = StringExtensions.Base64Decode(splitedCredentials[0]);
                            var clientSecret = StringExtensions.Base64Decode(splitedCredentials[1]);

                            beGlobalWindow.LoginTab.ClientIdBox.Password = splitedCredentials.Length > 0 ? clientId : string.Empty;
                            beGlobalWindow.LoginTab.ClientSecretBox.Password = splitedCredentials.Length > 0 ? clientSecret : string.Empty;
                        }
                    }
                }
            }
            else
            {
                loginViewModel.SelectedAuthentication = loginViewModel.Authentications[1];
                loginViewModel.SelectedIndex = loginViewModel.SelectedAuthentication.Index;
            }
        }

        private void CloseWindow(BeGlobalWindow beGlobalWindow)
        {
            WindowCloser.SetDialogResult(beGlobalWindow, true);
            beGlobalWindow?.Close();
        }
    }
}