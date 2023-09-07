using System;
using System.Windows.Input;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel.Edge
{
	public class EdgeCredentialsViewModel : BaseViewModel, ICredentialsViewModel
	{
		private AuthenticationType _authenticationType;

		private string _host;
		private string _apiKey;
		private string _username;
		private string _password;

		public EdgeCredentialsViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
		}

		public ITranslationOptions TranslationOptions { get; set; }
		public AuthenticationType AuthenticationType
		{
			get => _authenticationType;
			set
			{
				if (_authenticationType == value) return;
				_authenticationType = value;
				OnPropertyChanged();
			}
		}

		public string Host
		{
			get => _host;
			set
			{
				if (_host == value) return;
				_host = value;
				OnPropertyChanged();
			}
		}

		public string ApiKey
		{
			get => _apiKey;
			set
			{
				if (_apiKey == value) return;
				_apiKey = value;
				OnPropertyChanged();
			}
		}

		public string Username
		{
			get => _username;
			set
			{
				if (_username == value) return;
				_username = value;
				OnPropertyChanged();
			}
		}

		public string Password
		{
			get => _password;
			set
			{
				if (_password == value) return;
				_password = value;
				OnPropertyChanged();
			}
		}

		public ICommand BackCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public ICommand SignInCommand { get; private set; }

		public ICommand SelectAuthenticationTypeCommand { get; private set; }

		public event EventHandler CloseRequested;
		public event EventHandler StartLoginProcess;
		public event EventHandler StopLoginProcess;

		private void InitializeCommands()
		{

		}

	}
}