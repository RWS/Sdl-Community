using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
		private SolidColorBrush _textMessageBrush;
		private ICommand _loginCommand;
		private ICommand _passwordChangedCommand;
		private readonly UserControl _view;
		private readonly WizardModel _wizardModel;

		public LoginViewModel(WizardModel wizardModel,object view): base(view)
		{
			_isValid = false;
			_view =(UserControl)view;
			_wizardModel = wizardModel;
			_textMessageVisibility = "Collapsed";
			_passwordBoxVisibility = "Visible";
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
			get => _wizardModel.UserCredentials.ServiceUrl;
			set
			{
				_wizardModel.UserCredentials.ServiceUrl = value;
				OnPropertyChanged(nameof(Url));
			}
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

	//	public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new AwaitableDelegateCommand(AuthenticateUser));
		public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new ParameterCommand(AuthenticateUser));
		public ICommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = new ParameterCommand(PasswordChanged));

		private void PasswordChanged(object parameter)
		{
			var passwordBoxText = (parameter as PasswordBox)?.Password;
			PasswordBoxVisibility = string.IsNullOrEmpty(passwordBoxText) ? "Visible" : "Collapsed";
		}

		private async void AuthenticateUser(object parameter)
		{
			var passwordBox = parameter as PasswordBox;
			var languageFlagsHelper = new LanguageFlags();
			var password = passwordBox?.Password;
			var projectService = new ProjectService();
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
						TextMessage = PluginResources.AuthenticationSuccess;
						TextMessageBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#017701");
						var projectsResponse = await projectService.GetGsProjects();
						if (projectsResponse?.Items != null)
						{
							foreach (var project in projectsResponse.Items)
							{
								var testLanguages = new ObservableCollection<TargetLanguageFlag>();
								var targetLanguages = new TargetLanguageFlag
								{
									Path = languageFlagsHelper.GetImageStudioCodeByLanguageCode(project.SourceLanguage)
								};
								testLanguages.Add(targetLanguages);
								testLanguages.Add(targetLanguages);

								var gsProject = new GsProject
								{
									Name = project.Name,
									DueDate = project.DueDate?.ToString(),
									SourceLanguageFlagUri = languageFlagsHelper.GetImageStudioCodeByLanguageCode(project.SourceLanguage),
									TargetLanguageFlagsUri = testLanguages
								};
								_wizardModel?.GsProjects.Add(gsProject);
							}
						}

						TextMessageVisibility = "Visible";
						_view.Dispatcher.Invoke(delegate { SendKeys.SendWait("{TAB}"); }, DispatcherPriority.ApplicationIdle);
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
