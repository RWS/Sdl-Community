using System;
using System.Linq;
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
		private Constants _constants = new Constants();
		private readonly BeGlobalWindow _mainWindow;
		public static readonly Log Log = Log.Instance;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options,
			TranslationProviderCredential credentialStore, LanguagePair[] languagePairs)
		{
			Options = options;	
			LanguageMappingsViewModel = new LanguageMappingsViewModel(options);	

			LoginViewModel = new LoginViewModel(options, languagePairs, LanguageMappingsViewModel, this);
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
				LanguageMappingsViewModel.LoadLanguageMappings();
				var isWindowValid = IsWindowValid(false);
				if(!isWindowValid)
				{
					Message = _constants.CredentialsAndInternetValidation;
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
			Mouse.OverrideCursor = Cursors.Wait;
			var loginTab = parameter as Login;
			if (loginTab != null)
			{
				var isValid = IsWindowValid(true);
				if (isValid)
				{
					// Remove and add the settings back to SettingsGroup of .sdlproj when user presses on Ok
					LanguageMappingsViewModel.SaveLanguageMappingSettings(LanguageMappingsViewModel.LanguageMappings);

					WindowCloser.SetDialogResult(_mainWindow, true);
					_mainWindow.Close();
				}
			}
			Mouse.OverrideCursor = Cursors.Arrow;
		}

		private bool IsWindowValid(bool isOkPressed)
		{
			var loginTab = _mainWindow?.LoginTab;
			Options.ResendDrafts = LanguageMappingsViewModel.ReSendChecked;
			try
			{
				var areEnginesLoaded = LanguageMappingsViewModel.LanguageMappings[0].Engines.Any();
				if (LoginViewModel.SelectedOption.Type.Equals(_constants.User))
				{
					var password = loginTab?.UserPasswordBox.Password;
					if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(LoginViewModel.Email))
					{
						Options.ClientId = LoginViewModel?.Email.TrimEnd().TrimStart();
						Options.ClientSecret = password.TrimEnd().TrimStart();
						Options.UseClientAuthentication = false;
						Message = string.Empty;
						if (isOkPressed || !areEnginesLoaded)
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
						
						if (isOkPressed || !areEnginesLoaded)
						{
							return LoginViewModel.ValidateEnginesSetup();
						}
						return true;
					}
				}
				if (loginTab != null)
				{
					Message = _constants.CredentialsValidation;
				}				
			}
			catch (Exception e)
			{
				if (loginTab != null)
				{
					Message = (e.Message.Contains(_constants.TokenFailed) || e.Message.Contains(_constants.NullValue)) ? _constants.CredentialsNotValid : e.Message;
				}
				Log.Logger.Error($"{_constants.IsWindowValid} {e.Message}\n {e.StackTrace}");
			}
			return false;
		}
	}
}