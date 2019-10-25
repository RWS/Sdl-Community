using System;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private MessageBoxService _messageBoxService;
		private TranslationProviderCredential _credentials;
		private int _selectedTabIndex;

		private ICommand _okCommand;

		public BeGlobalWindowViewModel(BeGlobalTranslationOptions options, LanguagePair[] languagePairs, TranslationProviderCredential credentials)
		{
			SelectedTabIndex = 0;
			Options = options;
			_languagePairs = languagePairs;
			_credentials = credentials;
			_messageBoxService = new MessageBoxService();
			LanguageMappingsViewModel = new LanguageMappingsViewModel(options);
			LoginViewModel = new LoginViewModel(options, _credentials, _languagePairs, LanguageMappingsViewModel);

			if (_credentials == null) return;
			var credential = _credentials.Credential.Replace("sdlmachinetranslationcloudprovider:///", string.Empty);
			if (credential.Contains("#"))
			{
				var splitedCredentials = credentials.Credential.Split('#');
				if (splitedCredentials.Length == 2 && !string.IsNullOrEmpty(splitedCredentials[0]) && !string.IsNullOrEmpty(splitedCredentials[1]))
				{
					var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
					var userInfo = beGlobalTranslator.GetUserInformation(true);
					var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(userInfo.AccountId.ToString());
					LoginViewModel.GetEngineModels(subscriptionInfo);
					LoginViewModel.SetEngineModel();
					SetAuthenticationOptions();
				}
			}
		}

		public LoginViewModel LoginViewModel { get; set; }
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }
		public BeGlobalTranslationOptions Options { get; set; }			

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				_selectedTabIndex = value;				
				OnPropertyChanged();
			}
		}
		
		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));
		
		private void Ok(object parameter)
		{
			var currentWindow = WindowsControlUtils.GetCurrentWindow() as BeGlobalWindow;
			var loginTab = parameter as Login;
			if (loginTab != null)
			{
				var isValid = IsWindowValid();
				if (isValid)
				{
					WindowCloser.SetDialogResult(currentWindow, true);
					currentWindow?.Close();
				}
			}				
		}
				
		private void SetAuthenticationOptions()
		{
			var currentWindow = WindowsControlUtils.GetCurrentWindow() as BeGlobalWindow;
			if (string.IsNullOrEmpty(Options.AuthenticationMethod)) return;
			if (Options.AuthenticationMethod.Equals(Constants.APICredentials))
			{
				if (!string.IsNullOrEmpty(Options.ClientId) && !string.IsNullOrEmpty(Options.ClientSecret))
				{
					currentWindow.LoginTab.ClientIdBox.Password = Options.ClientId;
					currentWindow.LoginTab.ClientSecretBox.Password = Options.ClientSecret;
				}
				else
				{
					var splitedCredentials = _credentials?.Credential.Split('#');
					var clientId = StringExtensions.Base64Decode(splitedCredentials[0]);
					var clientSecret = StringExtensions.Base64Decode(splitedCredentials[1]);

					currentWindow.LoginTab.ClientIdBox.Password = splitedCredentials.Length > 0 ? clientId : string.Empty;
					currentWindow.LoginTab.ClientSecretBox.Password = splitedCredentials.Length > 0 ? clientSecret : string.Empty;
				}
			}
			else
			{
				LoginViewModel.SelectedAuthentication = LoginViewModel.Authentications[1];
				LoginViewModel.SelectedIndex = LoginViewModel.SelectedAuthentication.Index;
			}
		}

		private bool IsWindowValid()
		{
			try
			{
				Options.ResendDrafts = LanguageMappingsViewModel.ReSendChecked;
				if (LoginViewModel.LoginMethod.Equals(Constants.APICredentials))
				{
					var clientIdPass = Options?.ClientId;
					var clientSecretPass = Options?.ClientSecret;
					if (string.IsNullOrEmpty(clientIdPass) || string.IsNullOrEmpty(clientSecretPass))
					{
						LoginViewModel.Message = Constants.CredentialsValidation;
						return false;
					}
				}

				var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
				var userInfo = beGlobalTranslator.GetUserInformation(false);
				if (userInfo.AccountId != 0)
				{
					if (Options?.Model == null)
					{
						var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(userInfo.AccountId.ToString());
						LoginViewModel.GetEngineModels(subscriptionInfo);
						LoginViewModel.SetEngineModel();
					}
					return true;
				}
				else
				{
					if (userInfo.Errors.Count > 0)
					{
						foreach (var error in userInfo.Errors)
						{
							LoginViewModel.Message += $@"{error.Code}: {error.Description}";
						}
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}		
	}
}