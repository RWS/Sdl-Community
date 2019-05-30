using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Core.Globalization;
using UserControl = System.Windows.Controls.UserControl;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class LoginViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private string _textMessage;
		private string _textMessageVisibility;
		private string _passwordBoxVisibility;
		private string _displayName;
		private SolidColorBrush _textMessageBrush;
		private ICommand _loginCommand;
		private ICommand _passwordChangedCommand;
		private readonly UserControl _view;
		private readonly WizardModel _wizardModel;
		public static readonly Log Log = Log.Instance;

		public LoginViewModel(WizardModel wizardModel,object view): base(view)
		{
			_isValid = false;
			_displayName = "Login";
			_view =(UserControl)view;
			_wizardModel = wizardModel;
			_textMessageVisibility = "Collapsed";
			_passwordBoxVisibility = "Visible";
		}

		public override string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName == value)
				{
					return;
				}

				_displayName = value;
				OnPropertyChanged(nameof(DisplayName));
			}
		}
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
			get => _wizardModel.UserCredentials.ServiceUrl;
			set
			{
				_wizardModel.UserCredentials.ServiceUrl = value;
				OnPropertyChanged(nameof(Url));
			}
		}
		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;

			var pagePosition = PageIndex - 1;
			if (position == pagePosition)
			{
				return false;
			}

			if (!IsValid && position > pagePosition)
			{
				message = PluginResources.UnableToNavigateToSelectedPage + Environment.NewLine + Environment.NewLine +
				          string.Format(PluginResources.The_data_on__0__is_not_valid, _displayName);
				return false;
			}

			return true;
		}
		public string UserName
		{
			get => _wizardModel.UserCredentials.UserName;
			set
			{
				_wizardModel.UserCredentials.UserName = value;
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
		public string PasswordBoxVisibility
		{
			get => _passwordBoxVisibility;
			set
			{
				_passwordBoxVisibility = value;
				OnPropertyChanged(nameof(PasswordBoxVisibility));
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
		public ICommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = new ParameterCommand(PasswordChanged));

		private void PasswordChanged(object parameter)
		{
			var passwordBoxText = (parameter as PasswordBox)?.Password;
			PasswordBoxVisibility = string.IsNullOrEmpty(passwordBoxText) ? "Visible" : "Collapsed";
		}

		private async void AuthenticateUser(object parameter)
		{
			try
			{
				var passwordBox = parameter as PasswordBox;
				var password = passwordBox?.Password;
				if (!string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(password))
				{
					_wizardModel.UserCredentials.UserName = UserName.TrimEnd().TrimStart();
					_wizardModel.UserCredentials.Password = password.TrimEnd().TrimStart();
					_wizardModel.UserCredentials.ServiceUrl = Url.TrimEnd().TrimStart();

					if (Uri.IsWellFormedUriString(Url, UriKind.Absolute))
					{
						var statusCode = await Authentication.Login(_wizardModel.UserCredentials);
						if (statusCode == HttpStatusCode.OK)
						{
							IsValid = true;
							ShowMessage(PluginResources.AuthenticationSuccess,"#00A8EB");

							var utils = new Utils();
							await utils.SetGsProjectsToWizard(_wizardModel, 0);

							_view.Dispatcher.Invoke(delegate { SendKeys.SendWait("{TAB}"); }, DispatcherPriority.ApplicationIdle);
						}
						else
						{
							ShowMessage(statusCode.ToString(), "#FF2121");
						}
					}
					else
					{
						ShowMessage(PluginResources.Incorrect_Url_Format, "#FF2121");
					}
				}
				else
				{
					ShowMessage(PluginResources.Required_Fields, "#FF2121");
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"AuthenticateUser method {e.Message}\n {e.StackTrace}");
			}
		}

		private void ShowMessage(string message, string color)
		{
			TextMessage = message;
			TextMessageVisibility = "Visible";
			TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
		}
	}
}
