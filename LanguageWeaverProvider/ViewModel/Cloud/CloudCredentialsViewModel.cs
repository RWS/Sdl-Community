using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.NewFolder;
using LanguageWeaverProvider.ViewModel.Interface;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudCredentialsViewModel : BaseViewModel, ICredentialsViewModel
	{
		private AuthenticationType _authenticationType;

		private string _userId;
		private string _userPassword;
		private string _clientId;
		private string _clientSecret;

		private bool _isSecretSelected;
		private bool _isCredentialsSelected;

		public CloudCredentialsViewModel(ITranslationOptions translationOptions)
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
				OnPropertyChanged(nameof(IsAuthenticationTypeSelected));
				IsCredentialsSelected = value == AuthenticationType.Credentials;
				IsSecretSelected = value == AuthenticationType.Secret;
			}
		}

		public bool IsCredentialsSelected
		{
			get => _isCredentialsSelected;
			set
			{
				if (_isCredentialsSelected == value) return;
				_isCredentialsSelected = value;
				OnPropertyChanged();
			}
		}

		public bool IsSecretSelected
		{
			get => _isSecretSelected;
			set
			{
				if (_isSecretSelected == value) return;
				_isSecretSelected = value;
				OnPropertyChanged();
			}
		}

		public bool IsAuthenticationTypeSelected => AuthenticationType != AuthenticationType.None;

		public string UserId
		{
			get => _userId;
			set
			{
				if (_userId == value) return;
				_userId = value;
				OnPropertyChanged();
			}
		}

		public string UserPassword
		{
			get => _userPassword;
			set
			{
				if (_userPassword == value) return;
				_userPassword = value;
				OnPropertyChanged();
			}
		}

		public string ClientId
		{
			get => _clientId;
			set
			{
				if (_userId == value) return;
				_clientId = value;
				OnPropertyChanged();
			}
		}

		public string ClientSecret
		{
			get => _clientSecret;
			set
			{
				if (_clientSecret == value) return;
				_clientSecret = value;
				OnPropertyChanged();
			}
		}

		public ICommand SelectAuthenticationTypeCommand { get; private set; }

		public ICommand SignInCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public ICommand BackCommand { get; private set; }

		public event EventHandler CloseRequested;

		private void InitializeCommands()
		{
			SelectAuthenticationTypeCommand = new RelayCommand(SelectAuthenticationType);
			SignInCommand = new RelayCommand(SignIn);
			ClearCommand = new RelayCommand(Clear);
			BackCommand = new RelayCommand(Back);
		}

		private void SelectAuthenticationType(object parameter)
		{
			if (parameter is not AuthenticationType authenticationType)
			{
				return;
			}

			AuthenticationType = authenticationType;
		}

		private async void SignIn(object parameter)
		{
			if (AuthenticationType == AuthenticationType.None
			 ||!CredentialsAreSet())
			{
				return;
			}

			var cloudCredentials = new CloudCredentials()
			{
				UserID = _userId,
				UserPassword = _userPassword,
				ClientID = _clientId,
				ClientSecret = _clientSecret
			};

			var (response, success) = await CloudService.AuthenticateUser(cloudCredentials, AuthenticationType);
			if (!success)
			{
				// TO DO: Implement error/bad request/exceptions handling
				return;
			}

			cloudCredentials.AccessToken = JsonConvert.DeserializeObject<AccessToken>(response);
			TranslationOptions.CloudCredentials = cloudCredentials;
			CloseWindow();
		}

		private bool CredentialsAreSet()
		{
			if (IsCredentialsSelected
			 && string.IsNullOrEmpty(UserId)
			 && string.IsNullOrEmpty(UserPassword))
			{
				return false;
			}
			else if (IsSecretSelected
				  && string.IsNullOrEmpty(ClientId)
				  && string.IsNullOrEmpty(ClientSecret))
			{
				return false;
			}

			return AuthenticationType != AuthenticationType.None;
		}

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case "UserID":
					UserId = string.Empty;
					break;

				default:
					break;
			}
		}

		private void Back(object parameter)
		{
			AuthenticationType = AuthenticationType.None;
		}

		private void CloseWindow()
		{
			CloseRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}