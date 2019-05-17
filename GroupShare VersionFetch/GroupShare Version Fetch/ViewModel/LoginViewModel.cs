using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
		private string _textMessage;
		private string _textMessageVisibility;
		private SolidColorBrush _textMessageBrush;
		private ICommand _loginCommand;

		public LoginViewModel(object view): base(view)
		{
			_isValid = false;
			_textMessageVisibility = "Collapsed";
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
		public string TextMessage
		{
			get => _textMessage;
			set
			{
				_textMessage = value;
				OnPropertyChanged(nameof(TextMessage));
			}
		}
		public string TextMessageVisibility
		{
			get => _textMessageVisibility;
			set
			{
				_textMessageVisibility = value;
				OnPropertyChanged(nameof(TextMessageVisibility));
			}
		}

		public SolidColorBrush TextMessageBrush
		{
			get => _textMessageBrush;
			set
			{
				_textMessageBrush = value;
				OnPropertyChanged(nameof(TextMessageBrush));
			}
		}

		public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new ParameterCommand(AuthenticateUser));

		private async void AuthenticateUser(object parameter)
		{
			var passwordBox = parameter as PasswordBox;
			var password = passwordBox?.Password;
			if (!string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(password))
			{
				var credentials = new Credentials
				{
					UserName = UserName,
					ServiceUrl = Url.TrimEnd().TrimStart(),
					Password = password
				};
				if (Uri.IsWellFormedUriString(Url, UriKind.Absolute))
				{
					var statusCode = await Authentication.Login(credentials);
					if (statusCode == HttpStatusCode.OK)
					{
						TextMessageVisibility = "Visible";
						TextMessage = PluginResources.AuthenticationSuccess;
						TextMessageBrush = (SolidColorBrush) new BrushConverter().ConvertFrom("#017701");
						IsValid = true;
					}
					else
					{
						ShowErrorMessage(statusCode.ToString());
					}
				}
				else
				{
					ShowErrorMessage(PluginResources.Incorrect_Url_Format);
				}
			}
			else
			{
				ShowErrorMessage(PluginResources.Required_Fields);
			}
		}

		private void ShowErrorMessage(string message)
		{
			TextMessageVisibility = "Visible";
			TextMessage = message;
			TextMessageBrush = new SolidColorBrush(Colors.Red);
		}
	}
}
