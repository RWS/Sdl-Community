using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class LoginWindowViewModel : ViewModelBase, IDataErrorInfo, IWindowActions
	{
		private string _url;
		private string _userName;
		private ICommand _okCommand;
		private readonly LoginWindow _window;
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private Login _credentials;
		private string _message;
		private readonly BackgroundWorker _backgroundWorker;
		private readonly string _messageColor;
		private bool _hasText;
		private string _visibility;

		public LoginWindowViewModel(LoginWindow window, ObservableCollection<TmFile> tmsCollection)
		{
			_credentials = new Login();
			_messageColor = "#DF4762";
			_visibility = "Hidden";
			_message = string.Empty;
			_window = window;
			_hasText = false;
			_tmsCollection = tmsCollection;
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				if (e.Error.Message.Equals("One or more errors occurred."))
				{
					if (e.Error.InnerException != null)
					{
						Message = e.Error.InnerException.Message;
						MessageColor = "#DF4762";
					}

				}
				else
				{
					Message = e.Error.Message;
					MessageColor = "#DF4762";
				}
			}
			else
			{
				_window.Close();
			}

		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			GetServerTms(Credentials);
		}

		public ICommand OkCommand => _okCommand ??
									 (_okCommand = new RelayCommand(Ok));
		public Login Credentials
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

						return StringResources.Url_is_required;
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

		public string Error { get; }

		private void Ok(object parameter)
		{
			if (parameter is PasswordBox passwordBox)
			{
				var login = new Login
				{
					Password = passwordBox.Password,
					Url = Url,
					UserName = UserName
				};
				Credentials = login;
				HasText = true;
				if (IsValid())
				{
					_backgroundWorker.RunWorkerAsync();
					MessageColor = "#3EA691";
					Message = StringResources.Ok_Please_wait_until_we_connect_to_GroupShare;
					Visibility = "Visible";
				}
				else
				{
					Message = StringResources.Ok_All_fields_are_required_;
					MessageColor = "#DF4762";
				}
			}
		}

		/// <summary>
		/// Connects to GS and set the list of TMS
		/// </summary>
		/// <param name="login"></param>
		private void GetServerTms(Login login)
		{
			var uri = new Uri(login.Url);
			var translationProviderServer = new TranslationProviderServer(uri, false, login.UserName, login.Password);
			var translationMemories = translationProviderServer.GetTranslationMemories(TranslationMemoryProperties.None);

			foreach (var tm in translationMemories)
			{
				var tmPath = tm.ParentResourceGroupPath == "/" ? "" : tm.ParentResourceGroupPath;
				var path = tmPath + "/" + tm.Name;
				var tmAlreadyExist = _tmsCollection.Any(t => t.Path.Equals(path));
				if (!tmAlreadyExist)
				{
					var serverTm = new TmFile
					{
						Path = path,
						Name = tm.Name,
						IsServerTm = true
					};

					System.Windows.Application.Current.Dispatcher.Invoke(delegate
					{
						_tmsCollection.Add(serverTm);
					});
				}
			}
		}

		/// <summary>
		/// Validation for the form
		/// All fields are required
		/// </summary>
		/// <returns></returns>
		private bool IsValid()
		{
			return !string.IsNullOrEmpty(Credentials.Password) && !string.IsNullOrEmpty(Credentials.Url) &&
				   !string.IsNullOrEmpty(Credentials.UserName);
		}
	}

	public interface IWindowActions
	{
	}
}
