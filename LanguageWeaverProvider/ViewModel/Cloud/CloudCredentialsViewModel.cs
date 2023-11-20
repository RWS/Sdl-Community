using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.View.Cloud;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudCredentialsViewModel : BaseViewModel, ICredentialsViewModel
	{
		AuthenticationType _authenticationType;

		string _connectionCode;
		string _userName;
		string _userPassword;
		string _clientId;
		string _clientSecret;

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
				OnPropertyChanged(nameof(IsSSOSelected));
			}
		}

		public bool IsAuthenticationTypeSelected => IsApiKeySelected || IsCredentialsSelected || IsSSOSelected;

		public bool IsCredentialsSelected => AuthenticationType == AuthenticationType.CloudCredentials;

		public bool IsApiKeySelected => AuthenticationType == AuthenticationType.CloudAPI;

		public bool IsSSOSelected => AuthenticationType == AuthenticationType.CloudSSO;

		public string ConnectionCode
		{
			get => _connectionCode;
			set
			{
				_connectionCode = value;
				OnPropertyChanged();
			}
		}

		public string UserName
		{
			get => _userName;
			set
			{
				_userName = value;
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

			UserName = TranslationOptions.CloudCredentials.UserName;
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
		}

		private async void SignIn(object parameter)
		{
			if (!CredentialsAreSet())
			{
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				ErrorHandling.ShowDialog(null, PluginResources.Connection_Credentials, PluginResources.Connection_Error_NoCredentials);
				return;
			}

			StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Connection_Loading_Connecting));
			TranslationOptions.CloudCredentials ??= new();
			if (AuthenticationType == AuthenticationType.CloudCredentials)
			{
				TranslationOptions.CloudCredentials.UserName = UserName;
				TranslationOptions.CloudCredentials.UserPassword = UserPassword;
			}
			else if (AuthenticationType == AuthenticationType.CloudAPI)
			{
				TranslationOptions.CloudCredentials.ClientID = ClientId;
				TranslationOptions.CloudCredentials.ClientSecret = ClientSecret;
			}
			else if (AuthenticationType == AuthenticationType.CloudSSO)
			{
				Auth0SignIn();
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				return;
			}

			var response = await CloudService.AuthenticateUser(TranslationOptions.CloudCredentials, TranslationOptions, AuthenticationType, SelectedRegion);
			if (!response.Success)
			{
				StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Connection_Error_FirstFail));
				response = await CloudService.AuthenticateUser(TranslationOptions.CloudCredentials, TranslationOptions, AuthenticationType, SelectedRegion);
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
			var auth0Config = new Auth0Config(ConnectionCode, SelectedRegion);
			var cloudAuth0ViewModel = new CloudAuth0ViewModel(TranslationOptions, auth0Config);
			var cloudAuth0View = new CloudAuth0View() { DataContext = cloudAuth0ViewModel };
			cloudAuth0ViewModel.CloseAuth0Raised += () =>
			{
				cloudAuth0View.Close();
				if (cloudAuth0ViewModel.IsConnected)
				{
					CloseWindow();
				}
			};

			cloudAuth0View.ShowDialog();
		}

		private bool CredentialsAreSet()
		{
			if (IsCredentialsSelected
			&& (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword)))
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
				case nameof(UserName):
					UserName = string.Empty;
					break;

				case nameof(ClientId):
					ClientId = string.Empty;
					break;

				case nameof(ConnectionCode):
					ConnectionCode = string.Empty;
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
			TranslationOptions.PluginVersion = PluginVersion.LanguageWeaverCloud;
			TranslationOptions.UpdateUri();
			CloseRequested?.Invoke(this, EventArgs.Empty);
		}

		private string _selectedRegion;

		public string SelectedRegion
		{
			get { return _selectedRegion; }
			set
			{
				if (_selectedRegion != value)
				{
					_selectedRegion = value;
					OnPropertyChanged(nameof(SelectedRegion));
				}
			}
		}
	}
}