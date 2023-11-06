using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.View.Cloud;
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

		public CloudCredentialsViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
			LoadCredentials();
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
				OnPropertyChanged(nameof(IsCredentialsSelected));
				OnPropertyChanged(nameof(IsApiKeySelected));
			}
		}

		public bool IsCredentialsSelected => AuthenticationType == AuthenticationType.CloudCredentials;

		public bool IsApiKeySelected => AuthenticationType == AuthenticationType.CloudSecret;

		public bool IsAuthenticationTypeSelected => IsApiKeySelected || IsCredentialsSelected;

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

		public ICommand SelectAuthenticationTypeCommand { get; private set; }

		public event EventHandler CloseRequested;

		public event EventHandler StartLoginProcess;

		public event EventHandler StopLoginProcess;

		private void InitializeCommands()
		{
			BackCommand = new RelayCommand(Back);
			ClearCommand = new RelayCommand(Clear);
			SignInCommand = new RelayCommand(SignIn);
			SelectAuthenticationTypeCommand = new RelayCommand(SelectAuthenticationType);
		}

		private void LoadCredentials()
		{
			if (TranslationOptions.CloudCredentials is null)
			{
				return;
			}

			UserId = TranslationOptions.CloudCredentials.UserID;
			UserPassword = TranslationOptions.CloudCredentials.UserPassword;
			ClientId = TranslationOptions.CloudCredentials.ClientID;
			ClientSecret = TranslationOptions.CloudCredentials.ClientSecret;
		}

		private void SelectAuthenticationType(object parameter)
		{
			if (parameter is not AuthenticationType authenticationType)
			{
				return;
			}

			AuthenticationType = authenticationType;
			if (AuthenticationType == AuthenticationType.CloudSSO)
			{
				Auth0SignIn();
			}
		}

		private async void SignIn(object parameter)
		{
			if (AuthenticationType == AuthenticationType.None
			 || !CredentialsAreSet())
			{
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				ErrorHandling.ShowDialog(null, PluginResources.Connection_Credentials, PluginResources.Connection_Error_NoCredentials);
				return;
			}

			StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Connection_Loading_Connecting));
			TranslationOptions.CloudCredentials ??= new();
			if (AuthenticationType == AuthenticationType.CloudCredentials)
			{
				TranslationOptions.CloudCredentials.UserID = UserId;
				TranslationOptions.CloudCredentials.UserPassword = UserPassword;
			}
			else if (AuthenticationType == AuthenticationType.CloudSecret)
			{
				TranslationOptions.CloudCredentials.ClientID = ClientId;
				TranslationOptions.CloudCredentials.ClientSecret = ClientSecret;
			}

			var response = await CloudService.AuthenticateUser(TranslationOptions.CloudCredentials, TranslationOptions, AuthenticationType);
			if (!response.Success)
			{
				StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Connection_Error_FirstFail));
				response = await CloudService.AuthenticateUser(TranslationOptions.CloudCredentials, TranslationOptions, AuthenticationType);
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				if (!response.Success)
				{
					response.Error.ShowDialog(PluginResources.Connection_Error_Failed, response.Error.Message, true);
					return;
				}
			}

			CloseWindow();
		}

		private void Auth0SignIn()
		{
			var cloudAuth0ViewModel = new CloudAuth0ViewModel(TranslationOptions);
			var cloudAuth0View = new CloudAuth0View() { DataContext = cloudAuth0ViewModel };
			cloudAuth0ViewModel.CloseAuth0Raised += () => { cloudAuth0View.Close(); CloseWindow(); };
			cloudAuth0View.ShowDialog();
		}

		private bool CredentialsAreSet()
		{
			if (IsCredentialsSelected
			&& (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(UserPassword)))
			{
				return false;
			}
			else if (IsApiKeySelected
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

		public void CloseWindow()
		{
			TranslationOptions.AuthenticationType = AuthenticationType;
			TranslationOptions.Version = PluginVersion.LanguageWeaverCloud;
			TranslationOptions.UpdateUri();
			CloseRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}