using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		public BeGlobalTranslationOptions Options { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		public SettingsViewModel SettingsViewModel { get; set; }
		

		private ICommand _okCommand;
		private readonly BeGlobalWindow _mainWindow;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options, TranslationProviderCredential credentialStore)
		{
			LoginViewModel = new LoginViewModel(mainWindow);
			SettingsViewModel = new SettingsViewModel(mainWindow);
			Options = options;
			_mainWindow = mainWindow;	
			if (credentialStore != null)
			{
				_mainWindow.LoginTab.ClientKeyBox.Password = options.ClientId;
				_mainWindow.LoginTab.ClientSecretBox.Password = options.ClientSecret;
			}
		}	

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		private void Ok(object parameter)
		{
			var loginTab = parameter as Login;
			if (loginTab != null)
			{
				var isValid = IsWindowValid(loginTab);
				if (isValid)
				{
					WindowCloser.SetDialogResult(_mainWindow, true);
					_mainWindow.Close(); 
				}	
			} 
		}

		private bool IsWindowValid(Login loginTab)
		{
			if (LoginViewModel.SelectedOption.Type.Equals("User"))
			{
				var password = loginTab.PasswordBox.Password;
				if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(LoginViewModel.Email))
				{
					Options.ClientId = LoginViewModel.Email;
					Options.ClientSecret = password;
					loginTab.ValidationBlock.Visibility = Visibility.Collapsed;
					Options.UseClientAuthentication = false;
					return true;
				}
			}
			else
			{
				var clientId = loginTab.ClientKeyBox.Password;
				var clientSecret = loginTab.ClientSecretBox.Password;
				if (!string.IsNullOrEmpty(clientId.TrimEnd()) && !string.IsNullOrEmpty(clientSecret.TrimEnd()))
				{
					Options.ClientId = clientId;
					Options.ClientSecret = clientSecret;
					Options.UseClientAuthentication = true;
					loginTab.ValidationBlock.Visibility = Visibility.Collapsed;
					return true;
				}	
			}
			loginTab.ValidationBlock.Visibility = Visibility.Visible;
			return false;
		}
		
	}
}
