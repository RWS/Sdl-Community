using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class LoginWindowViewModel: ViewModelBase,IDataErrorInfo, IWindowActions
	{
		private string _url;
		private string _userName;
		private ICommand _okCommand;
		private readonly LoginWindow _window;
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private Login _credentials;
		private string _message;
		private readonly BackgroundWorker _backgroundWorker;
		private string _messageColor;
		private bool _hasText;

		public LoginWindowViewModel(LoginWindow window, ObservableCollection<TmFile> tmsCollection)
		{
			_credentials = new Login();
			_messageColor = "#DF4762";
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
		public string this[string columnName]
		{
			get
			{
				if (columnName == "Url")
				{
					if (string.IsNullOrEmpty(Url))

						return "Url is required";
				}
				if (columnName == "UserName" && string.IsNullOrEmpty(UserName))
				{

					return "User name is rquired";

				}
				if (columnName == "HasText" && string.IsNullOrEmpty(Credentials.Password))
				{

					return "Password is rquired";

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
			var passwordBox = parameter as PasswordBox;
			if (passwordBox != null)
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
					Message = "Please wait until we connect to GroupShare";
				}
				else
				{
					Message = "All fields are required!";
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
			try
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
			catch (Exception exception)
			{
				throw ;
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
