using System.Collections.Generic;
using System.Diagnostics;
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

		public LoginViewModel(BeGlobalTranslationOptions options)
		{
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
						LoginMethod = "User";
					}
					else
					{
						LoginMethod = "Client";
					}
				}
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
	}
}