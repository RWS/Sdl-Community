using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.ViewModel.Interface;

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
				_authenticationType = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsAuthenticationTypeSelected));
				IsCredentialsSelected = value == AuthenticationType.CloudCredentials;
				IsSecretSelected = value == AuthenticationType.CloudSecret;
			}
		}

		public bool IsCredentialsSelected
		{
			get => _isCredentialsSelected;
			set
			{
				_isCredentialsSelected = value;
				OnPropertyChanged();
			}
		}

		public bool IsSecretSelected
		{
			get => _isSecretSelected;
			set
			{
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
				_userId = value;
				OnPropertyChanged();
			}
		}

		public string UserPassword
		{
			get => _userPassword;
			set
			{
				_userPassword = value;
				OnPropertyChanged();
			}
		}

		public string ClientId
		{
			get => _clientId;
			set
			{
				_clientId = value;
				OnPropertyChanged();
			}
		}

		public string ClientSecret
		{
			get => _clientSecret;
			set
			{
				_clientSecret = value;
				OnPropertyChanged();
			}
		}

		public ICommand BackCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public ICommand SignInCommand { get; private set; }

		public ICommand SignLoggedUserCommand { get; private set; }

		public ICommand SelectAuthenticationTypeCommand { get; private set; }

		public event EventHandler CloseRequested;

		public event EventHandler StartLoginProcess;

		public event EventHandler StopLoginProcess;

		public void CloseWindow() => CloseRequested?.Invoke(this, EventArgs.Empty);

		private void InitializeCommands()
		{
			BackCommand = new RelayCommand(Back);
			ClearCommand = new RelayCommand(Clear);
			SignInCommand = new RelayCommand(SignIn);
			SignLoggedUserCommand = new RelayCommand(SignLoggedUser);
			SelectAuthenticationTypeCommand = new RelayCommand(SelectAuthenticationType);
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
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				ErrorHandling.ShowDialog(null, PluginResources.Connection_Credentials, PluginResources.Connection_Error_NoCredentials);
				return;
			}

			StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Connection_Loading_Connecting));
			var cloudCredentials = new CloudCredentials()
			{
				UserID = _userId,
				UserPassword = _userPassword,
				ClientID = _clientId,
				ClientSecret = _clientSecret
			};

			var response = await CloudService.AuthenticateUser(cloudCredentials, TranslationOptions, AuthenticationType);
			if (!response.Success)
			{
				StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Connection_Error_FirstFail));
				response = await CloudService.AuthenticateUser(cloudCredentials, TranslationOptions, AuthenticationType);
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				if (!response.Success)
				{
					response.Error.ShowDialog(PluginResources.Connection_Error_Failed, response.Error.Message, true);
					return;
				}
			}

			TranslationOptions.CloudCredentials = cloudCredentials;
			TranslationOptions.AuthenticationType = AuthenticationType;
			CloseWindow();
		}

		private async void SignLoggedUser(object parameter)
		{
			StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Connection_Loading_Connecting));
			var response = await CloudService.AuthenticateUser(TranslationOptions.CloudCredentials, TranslationOptions, AuthenticationType.CloudCredentials);
			StopLoginProcess?.Invoke(this, EventArgs.Empty);
			if (!response.Success)
			{
				response.Error.ShowDialog(PluginResources.Connection_Error_Failed, response.Error.Message, true);
				return;
			}

			TranslationOptions.CloudCredentials = TranslationOptions.CloudCredentials;
			TranslationOptions.AuthenticationType = AuthenticationType.CloudCredentials;
			CloseWindow();
		}

		private bool CredentialsAreSet()
		{
			if (IsCredentialsSelected
			&& (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(UserPassword)))
			{
				return false;
			}
			else if (IsSecretSelected
				 && (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ClientSecret)))
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
				case nameof(UserId):
					UserId = string.Empty;
					break;

				case nameof(ClientId):
					ClientId = string.Empty;
					break;

				default:
					break;
			}
		}

		private void Back(object parameter)
		{
			AuthenticationType = AuthenticationType.None;
		}
	}
}