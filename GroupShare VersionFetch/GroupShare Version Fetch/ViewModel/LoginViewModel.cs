using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class LoginViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private string _url;
		private string _userName;
		private ICommand _loginCommand;

		public LoginViewModel(object view): base(view)
		{
			_isValid = true;
		}

		public override string DisplayName => "Login";
		public override bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid == value)
					return;

				_isValid = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}
		public string Url
		{
			get => _url;
			set
			{
				_url = value;
				OnPropertyChanged(nameof(Url));
			}
		}
		public string UserName
		{
			get => _userName;
			set
			{
				_userName = value;
				OnPropertyChanged(nameof(UserName));
			}
		}
		public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new ParameterCommand(LoginUser));

		private async void LoginUser(object parameter)
		{
			var passwordBox = parameter as PasswordBox;
			var password = passwordBox?.Password;
			if (!string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(password))
			{
				var credentials = new Credentials
				{
					UserName = UserName,
					ServiceUrl = Url,
					Password = password
				};
				var token = await Authentication.Login(credentials);
			}
		}
	}
}
