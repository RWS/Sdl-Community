using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{
		private Authentication _selectedOption;
		private string _email;
		private ICommand _navigateCommand;
		private string _loginMethod;
		private string _message;

		private ICommand _passwordChangedCommand;

		public LoginViewModel(BeGlobalTranslationOptions options)
		{
			AuthenticationOptions = new List<Authentication>
			{
				new Authentication
				{
					DisplayName = Constants.ClientAuthentication,
					Type = Constants.Client
				},
				new Authentication
				{   DisplayName = Constants.UserAuthentication,
					Type = Constants.User
				}
			};
			SelectedOption = options.UseClientAuthentication ? AuthenticationOptions[0] : AuthenticationOptions[1];
			LoginMethod = SelectedOption.Type;
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

		public List<Authentication> AuthenticationOptions { get; set; }

		public Authentication SelectedOption
		{
			get => _selectedOption;
			set
			{
				_selectedOption = value;
				if (_selectedOption != null)
				{
					LoginMethod = _selectedOption.Type.Equals(Constants.User) ? Constants.User : Constants.Client; 					
				}
				OnPropertyChanged();
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

		public string Email
		{
			get => _email;
			set
			{
				_email = value;
				OnPropertyChanged();
			}
		}

		public ICommand NavigateCommand => _navigateCommand ?? (_navigateCommand = new RelayCommand(Navigate));
		public ICommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = new RelayCommand(ChangePasswordAction));

		private void ChangePasswordAction(object parameter)
		{
			if (parameter.GetType().Name.Equals(Constants.PasswordBox))
			{
				var passwordBox = (PasswordBox)parameter;				
				if (passwordBox.Password.Length > 0)
				{
					Message = string.Empty;
				}
			}
			else
			{
				var textBox = (TextBox)parameter;
				if (textBox.Text.Length > 0)
				{
					Message = string.Empty;
				}
			}
		}

		private void Navigate(object obj)
		{
			Process.Start("https://translate.sdlbeglobal.com/");
		}
	}
}