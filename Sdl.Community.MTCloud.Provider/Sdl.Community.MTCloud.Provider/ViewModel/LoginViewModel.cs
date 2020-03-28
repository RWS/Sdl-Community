using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{

		private Authentication _selectedOption;
		private string _email;
		private string _loginMethod;
		private readonly OptionsWindowModel _optionsWindowModel;

		private ICommand _credentialsChangedCommand;
		private ICommand _navigateCommand;

		public LoginViewModel(OptionsWindowModel optionsWindowModel)
		{
			_optionsWindowModel = optionsWindowModel;

			//AuthenticationOptions = new List<Authentication>
			//{
			//	new Authentication
			//	{
			//		DisplayName = Constants.ClientAuthentication,
			//		Type = Constants.Client
			//	},
			//	new Authentication
			//	{   DisplayName = Constants.UserAuthentication,
			//		Type = Constants.User
			//	}
			//};

			if (!string.IsNullOrEmpty(_optionsWindowModel.Options.AuthenticationMethod))
			{
				SelectedOption = _optionsWindowModel.Options.AuthenticationMethod.Equals(Constants.ClientLoginAuthentication)
					? AuthenticationOptions[0]
					: AuthenticationOptions[1];
			}
			else
			{
				SelectedOption = AuthenticationOptions[1];
			}

			//LoginMethod = SelectedOption.Type;
		}

		public ICommand NavigateCommand => _navigateCommand ?? (_navigateCommand = new RelayCommand(Navigate));

		public ICommand CredentialsChangedCommand => _credentialsChangedCommand ?? (_credentialsChangedCommand = new RelayCommand(CredentialsChangedAction));

		public bool CredentialsChanged { get; set; }

		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }

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
				if (_selectedOption == value)
				{
					return;
				}

				_selectedOption = value;
				OnPropertyChanged(nameof(SelectedOption));

				if (_selectedOption != null)
				{
					LoginMethod = _selectedOption.Type.Equals(Constants.User)
						? Constants.User
						: Constants.Client;
				}
												
				CredentialsChanged = true;				
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

		private void CredentialsChangedAction(object parameter)
		{
			CredentialsChanged = true;

			if (parameter.GetType().Name.Equals(Constants.PasswordBox))
			{
				var passwordBox = (PasswordBox)parameter;
				if (passwordBox.Password.Length > 0)
				{
					switch (passwordBox.Name)
					{
						case "ClientIdBox":
							_optionsWindowModel.Options.ClientId = passwordBox.Password;
							_optionsWindowModel.Options.AuthenticationMethod = Constants.ClientLoginAuthentication;
							break;
						case "ClientSecretBox":
							_optionsWindowModel.Options.ClientSecret = passwordBox.Password;
							_optionsWindowModel.Options.AuthenticationMethod = Constants.ClientLoginAuthentication;
							break;
						case "UserPasswordBox":
							_optionsWindowModel.Options.ClientSecret = passwordBox.Password;
							_optionsWindowModel.Options.AuthenticationMethod = Constants.UserLoginAuthentication;
							break;
					}

					_optionsWindowModel.Message = string.Empty;
				}
			}
			else
			{
				var textBox = (TextBox)parameter;
				if (textBox.Text.Length > 0)
				{
					_optionsWindowModel.Message = string.Empty;
					_optionsWindowModel.Options.ClientId = textBox.Text;
					_optionsWindowModel.Options.AuthenticationMethod = Constants.UserLoginAuthentication;
				}
			}
		}

		private void Navigate(object obj)
		{
			Process.Start(Constants.MTCloudTranslateUri);
		}
	}
}