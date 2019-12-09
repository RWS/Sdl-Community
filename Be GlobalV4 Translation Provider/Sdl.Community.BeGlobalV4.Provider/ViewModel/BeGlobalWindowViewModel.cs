using System;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		public BeGlobalTranslationOptions Options { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }
		private ICommand _okCommand;
		private int _selectedTabIndex;
		private string _message;
		private readonly BeGlobalWindow _mainWindow;

		public static readonly Log Log = Log.Instance;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options,
			TranslationProviderCredential credentialStore, LanguagePair[] languagePairs)
		{
			LanguageMappingsViewModel = new LanguageMappingsViewModel(options);
			LoginViewModel = new LoginViewModel(options, languagePairs, LanguageMappingsViewModel, this);
			Options = options;
			_mainWindow = mainWindow;

			if (credentialStore == null) return;
			if (options.UseClientAuthentication || options.AuthenticationMethod.Equals("ClientLogin"))
			{
				_mainWindow.LoginTab.ClientIdBox.Password = options.ClientId;
				_mainWindow.LoginTab.ClientSecretBox.Password = options.ClientSecret;
				LoginViewModel.SelectedOption = LoginViewModel.AuthenticationOptions[0];
			}
			else
			{
				LoginViewModel.Email = options.ClientId;
				_mainWindow.LoginTab.UserPasswordBox.Password = options.ClientSecret;
				LoginViewModel.SelectedOption = LoginViewModel.AuthenticationOptions[1];
			}
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				Mouse.OverrideCursor = Cursors.Wait;
				_selectedTabIndex = value;
				var isWindowValid = IsWindowValid(false);
				if(!isWindowValid)
				{
					Message = Constants.CredentialsAndInternetValidation;
				}
				OnPropertyChanged();
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}
		
		public string Message
		{
			get => _message;
			set
			{
				if (_message == value)
				{
					return;
				}
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		private void Ok(object parameter)
		{
			var loginTab = parameter as Login;
			if (loginTab != null)
			{
				var isValid = IsWindowValid(true);
				if (isValid)
				{
					WindowCloser.SetDialogResult(_mainWindow, true);
					_mainWindow.Close();
				}
			}
		}

		private bool IsWindowValid(bool isOkPressed)
		{
			var loginTab = _mainWindow?.LoginTab;
			Options.ResendDrafts = LanguageMappingsViewModel.ReSendChecked;
			Options.Model = LanguageMappingsViewModel.SelectedModelOption?.Model;
			try
			{
				if (LoginViewModel.SelectedOption.Type.Equals(Constants.User))
				{
					var password = loginTab?.UserPasswordBox.Password;
					if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(LoginViewModel.Email))
					{
						Options.ClientId = LoginViewModel?.Email.TrimEnd().TrimStart();
						Options.ClientSecret = password.TrimEnd().TrimStart();
						Options.UseClientAuthentication = false;
						Message = string.Empty;
						if (isOkPressed || string.IsNullOrEmpty(Options?.Model))
						{
							return LoginViewModel.ValidateEnginesSetup();
						}
						return true;
					}
				}
				else
				{
					var clientId = loginTab?.ClientIdBox.Password;
					var clientSecret = loginTab?.ClientSecretBox.Password;
					if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
					{
						Options.ClientId = clientId.TrimEnd().TrimStart();
						Options.ClientSecret = clientSecret.TrimEnd().TrimStart();
						Options.UseClientAuthentication = true;
						Message = string.Empty;

						if (isOkPressed || string.IsNullOrEmpty(Options?.Model))
						{
							return LoginViewModel.ValidateEnginesSetup();
						}
						return true;
					}
				}
				if (loginTab != null)
				{
					Message = Constants.CredentialsValidation;
				}				
			}
			catch (Exception e)
			{
				if (loginTab != null)
				{
					Message = (e.Message.Contains(Constants.TokenFailed) || e.Message.Contains(Constants.NullValue)) ? Constants.CredentialsNotValid : e.Message;
				}
				Log.Logger.Error($"{Constants.IsWindowValid} {e.Message}\n {e.StackTrace}");
			}
			return false;
		}
	}
}