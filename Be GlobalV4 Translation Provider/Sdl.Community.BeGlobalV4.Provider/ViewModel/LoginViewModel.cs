using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Application = System.Windows.Application;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{
		private Authentication _selectedAuthentication;
		private string _loginMethod;
		private readonly StudioCredentials _studioCredentials = new StudioCredentials();
		private string _message;
		private int _selectedIndex;

		private ICommand _passwordChangedCommand;

		public LoginViewModel(BeGlobalTranslationOptions options)
		{
			Options = options;

			_loginMethod =!string.IsNullOrEmpty(Options.AuthenticationMethod)? Options.AuthenticationMethod : Constants.APICredentials;
			SetAuthentications();
		}

		public BeGlobalTranslationOptions Options { get; set; }

		public List<Authentication> Authentications { get; set; }
		public Authentication SelectedAuthentication
		{
			get => _selectedAuthentication;
			set
			{
				_selectedAuthentication = value;
				OnPropertyChanged(nameof(SelectedAuthentication));
				CheckLoginMethod();
				SetClientOptions();
			}
		}

		// LoginMethod is used to display/hide the ClientId,ClientSecret fields based on which authentication mode is selected
		public string LoginMethod
		{
			get => _loginMethod;
			set
			{
				if (_loginMethod == value)
				{
					return;
				}
				_loginMethod = value;
				OnPropertyChanged(nameof(LoginMethod));				
			}
		}

		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				_selectedIndex = value;
				OnPropertyChanged(nameof(SelectedIndex));
			}
		}
		public ICommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = new RelayCommand(ChangePasswordAction));

		private void ChangePasswordAction(object parameter)
		{
			var passwordBox = (PasswordBox)parameter;
			switch(passwordBox.Name)
			{
				case "ClientIdBox":
					Options.ClientId = passwordBox.Password.TrimEnd().TrimStart();
					break;
				case "ClientSecretBox":
					Options.ClientSecret = passwordBox.Password.TrimEnd().TrimStart();
					break;
			}			
			if (passwordBox.Password.Length > 0)
			{
				Message = string.Empty;
			}
		}

		/// <summary>
		///  Set the LoginMethod based on user selection.
		///  If LoginMethod is Studio Authentication, check if user is logged-in in Studio
		/// </summary>
		private void CheckLoginMethod()
		{
			LoginMethod = SelectedAuthentication?.DisplayName;
			if (LoginMethod.Equals(Constants.StudioAuthentication))
			{
				Message = string.Empty;
				AppItializer.EnsureInitializer();
				Application.Current?.Dispatcher?.Invoke(() =>
				{
					_studioCredentials.GetToken();
				});
			}
		}

		private void SetClientOptions()
		{
			var currentWindow = WindowsControlUtils.GetCurrentWindow() as BeGlobalWindow;
			if (!string.IsNullOrEmpty(Options.ClientId) && !string.IsNullOrEmpty(Options.ClientSecret))
			{
				currentWindow.LoginTab.ClientIdBox.Password = Options.ClientId;
				currentWindow.LoginTab.ClientSecretBox.Password = Options.ClientSecret;
			}
			Options.AuthenticationMethod = SelectedAuthentication?.DisplayName;
		}

		private void SetAuthentications()
		{
			Authentications = new List<Authentication>
			{
				new Authentication
				{
					DisplayName = Constants.APICredentials,
					Type = Constants.APICredentialsType,
					Index = 0
				},
				new Authentication
				{   DisplayName = Constants.StudioAuthentication,
					Type = Constants.StudioAuthenticationType,
					Index = 1
				}
			};
			if (!string.IsNullOrEmpty(Options.AuthenticationMethod))
			{
				_selectedAuthentication = Authentications.FirstOrDefault(a => a.DisplayName.Equals(Options.AuthenticationMethod));
				SelectedIndex = _selectedAuthentication != null ? _selectedAuthentication.Index : 0;
			}
			else
			{
				// set by default APICredentials login method
				_selectedAuthentication = Authentications[0];
				SelectedIndex = Authentications[0].Index;
			}
		}
	}
}