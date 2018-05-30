using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class LoginWindowViewModel:ViewModelBase
	{
		private string _url;
		private string _userName;
		private ICommand _okCommand;
		private readonly LoginWindow _window;
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private Login _credentials;
		private string _message;
		private readonly BackgroundWorker _backgroundWorker;

		public LoginWindowViewModel(LoginWindow window, ObservableCollection<TmFile> tmsCollection)
		{
			_credentials = new Login();
			_message = string.Empty;
			_window = window;
			_tmsCollection = tmsCollection;
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			if (System.Windows.Application.Current == null)
			{
				new System.Windows.Application();
			}
			if (System.Windows.Application.Current != null)
				System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_window.Close();
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
				if (IsValid())
				{
					_backgroundWorker.RunWorkerAsync();
					Message = "Please wait until we connect to GroupShare";
				}
				else
				{
					Message = "All fields are required!";
				}
			}
		}

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
				if (exception.Message.Equals("One or more errors occurred."))
				{
					if (exception.InnerException != null)
					{
						Message = exception.InnerException.Message;
					}
				}
				else
				{
					Message = exception.Message;
				}
			}
		}
		
		private bool IsValid()
		{
			return !string.IsNullOrEmpty(Credentials.Password) && !string.IsNullOrEmpty(Credentials.Url) &&
			       !string.IsNullOrEmpty(Credentials.UserName);
		}
	}
}
