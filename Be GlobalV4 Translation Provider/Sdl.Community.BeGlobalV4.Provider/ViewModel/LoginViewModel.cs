using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{
		private readonly BeGlobalWindow _mainWindow;
		private Authentication _selectedOption;
		private string _clientAuthVisibility;
		private string _userAuthVisibility;
		private string _email;
		private ICommand _navigateCommand;

		public LoginViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options)
		{
			_mainWindow = mainWindow;	
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
	}
}
