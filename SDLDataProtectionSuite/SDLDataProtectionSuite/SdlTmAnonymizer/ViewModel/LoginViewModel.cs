using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
	public class LoginViewModel : ViewModelBase, IDataErrorInfo, IDisposable
	{
		private string _url;
		private string _userName;
		private ICommand _okCommand;
		private readonly Window _window;
		private Credentials _credentials;
		private string _message;
		private readonly BackgroundWorker _backgroundWorker;
		private readonly string _messageColorError;
		private readonly string _messageColorInformation;
		private bool _hasText;
		private string _visibility;

		public LoginViewModel(Window window, Credentials credentials)
		{
			_window = window;
			
			_messageColorError = "#DF4762";
			_messageColorInformation = "#3EA691";
			_visibility = "Hidden";
			_message = string.Empty;
			_window = window;
			_hasText = false;

			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

			if (credentials == null)
			{
				credentials = new Credentials();
			}

			Url = credentials.Url;
			UserName = credentials.UserName;			

			Credentials = credentials;
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{			
			if (e.Error != null)
			{				
				Credentials.IsAuthenticated = false;
				Error = e.Error.Message;

				MessageColor = _messageColorError;
				Message = e.Error.InnerException?.Message ?? e.Error.Message;
			}
			else
			{				
				Credentials.IsAuthenticated = true;
				TranslationProviderServer = e.Result as TranslationProviderServer;
				_window.DialogResult = true;
			}
		}

		private static void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var isValid = Login(e.Argument as Credentials, out var server);
			if (isValid)
			{				
				e.Result = server;
			}
			else
			{
				throw new Exception(StringResources.Login_Not_Authorized);
			}
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		public Credentials Credentials
		{
			get => _credentials;
			set
			{
				_credentials = value;
				OnPropertyChanged(nameof(Credentials));
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

		public bool HasText
		{
			get => _hasText;
			set
			{
				_hasText = value;
				OnPropertyChanged(nameof(HasText));
			}
		}

		public string Visibility
		{
			get => _visibility;
			set
			{
				_visibility = value;
				OnPropertyChanged(nameof(Visibility));
			}
		}

		public string this[string columnName]
		{
			get
			{
				if (columnName == "Url")
				{
					if (string.IsNullOrEmpty(Url))
					{
						return StringResources.Url_is_required;
					}
				}
				if (columnName == "UserName" && string.IsNullOrEmpty(UserName))
				{

					return StringResources.User_name_is_rquired;

				}
				if (columnName == "HasText" && string.IsNullOrEmpty(Credentials.Password))
				{

					return StringResources.Password_is_rquired;

				}
				return null;
			}
		}

		public string Url
		{
			get => _url;

			set
			{
				if (Equals(value, _url))
				{
					return;
				}
				_url = value;
				OnPropertyChanged(nameof(Url));
			}
		}

		public string UserName
		{
			get => _userName;

			set
			{
				if (Equals(value, _userName))
				{
					return;
				}
				_userName = value;
				OnPropertyChanged(nameof(UserName));
			}
		}

		public string MessageColor
		{
			get => _message;

			set
			{
				if (Equals(value, _message))
				{
					return;
				}
				_message = value;
				OnPropertyChanged(nameof(MessageColor));
			}
		}

		public string Error { get; private set; }

		public TranslationProviderServer TranslationProviderServer { get; set; }

		private static bool IsValid(Credentials credentials)
		{
			return !string.IsNullOrEmpty(credentials.Password) &&
			        !string.IsNullOrEmpty(credentials.Url) &&
			        !string.IsNullOrEmpty(credentials.UserName);

		}

		private static bool Login(Credentials credentials, out TranslationProviderServer translationProviderServer)
		{
			if (IsValid(credentials))
			{
				translationProviderServer = new TranslationProviderServer(new Uri(credentials.Url), false, credentials.UserName, credentials.Password);				
				return true;
			}

			translationProviderServer = null;
			return false;
		}

		private void Ok(object parameter)
		{
			if (parameter is PasswordBox passwordBox)
			{
				Credentials = new Credentials
				{
					Url = Url,
					UserName = UserName,
					Password = passwordBox.Password,
				};
				
				HasText = true;

				MessageColor = _messageColorInformation;
				Message = StringResources.Ok_Please_wait_until_we_connect_to_GroupShare;
				Visibility = "Visible";

				if (IsValid(Credentials))
				{
					_backgroundWorker.RunWorkerAsync(Credentials);
				}
				else
				{
					MessageColor = _messageColorError;
					Message = StringResources.Ok_All_fields_are_required;
				}
			}
		}

		public void Dispose()
		{
			if (_backgroundWorker != null)
			{
				_backgroundWorker.DoWork -= BackgroundWorker_DoWork;
				_backgroundWorker.RunWorkerCompleted -= BackgroundWorker_RunWorkerCompleted;
				_backgroundWorker.Dispose();
			}
		}
	}
}
