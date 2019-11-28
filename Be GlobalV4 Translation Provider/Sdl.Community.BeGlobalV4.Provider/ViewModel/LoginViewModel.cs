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
		private string _clientAuthVisibility;
		private string _userAuthVisibility;
		private string _email;
		private string _message;

		private ICommand _navigateCommand;
		private ICommand _passwordChangedCommand;

		public LoginViewModel(BeGlobalTranslationOptions options)
		{
			ClientAuthVisibility = "Collapsed";
			UserAuthVisibility = "Visible";
			AuthenticationOptions = new List<Authentication>
			{
				new Authentication
				{
					DisplayName = "Client Authentication",
					Type = "Client"
				},
				new Authentication
				{   DisplayName = "User Authentication",
					Type = "User"
				}
			};
			SelectedOption = options.UseClientAuthentication ? AuthenticationOptions[0] : AuthenticationOptions[1];	  
		}

		public ICommand NavigateCommand => _navigateCommand ?? (_navigateCommand = new RelayCommand(Navigate));

		private void Navigate(object obj)
		{
			Process.Start("https://translate.sdlbeglobal.com/");
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
					if (_selectedOption.Type.Equals("User"))
					{
						ClientAuthVisibility = "Collapsed";
						UserAuthVisibility = "Visible";
					}
					else
					{
						ClientAuthVisibility = "Visible";
						UserAuthVisibility = "Collapsed";
					}
				}	
				OnPropertyChanged();  
			}
		}

		public string ClientAuthVisibility
		{
			get => _clientAuthVisibility;
			set
			{
				_clientAuthVisibility = value;
				OnPropertyChanged();
			}
		}

		public string UserAuthVisibility
		{
			get => _userAuthVisibility;
			set
			{
				_userAuthVisibility = value;
				OnPropertyChanged();
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

		public ICommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = new RelayCommand(ChangePasswordAction));

		private void ChangePasswordAction(object parameter)
		{
			if (parameter.GetType().Name.Equals("PasswordBox"))
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
	}
}
