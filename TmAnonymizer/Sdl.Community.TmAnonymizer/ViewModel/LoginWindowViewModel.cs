using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

		public LoginWindowViewModel(LoginWindow window, ObservableCollection<TmFile> tmsCollection)
		{
			_window = window;
			_tmsCollection = tmsCollection;
		}
	
		public ICommand OkCommand => _okCommand ??
		                             (_okCommand = new RelayCommand(Ok));

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

				GetServerTms(login);
			}
		}

		private void GetServerTms(Login login)
		{
			var uri = new Uri(login.Url);
			var translationProviderServer = new TranslationProviderServer(uri, false, login.UserName, login.Password);
			var translationMemories = translationProviderServer.GetTranslationMemories(TranslationMemoryProperties.None);

			foreach (var tm in translationMemories)
			{
				var serverTm = new TmFile
				{
					Path = tm.Uri.AbsoluteUri,
					Name = tm.Name,
					IsServerTm = true
				};
				_tmsCollection.Add(serverTm);
			}
			_window.Close();
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
	}
}
