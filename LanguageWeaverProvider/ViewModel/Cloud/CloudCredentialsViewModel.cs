using System;
using System.Diagnostics;
using System.Security.Policy;
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

		string _selectedRegion;
		string _selectedRegionUIMessage;

		bool _showVerifyCredentialsWarning;

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

		public bool ShowVerifyCredentialsWarning
		{
			get => _showVerifyCredentialsWarning;
			set
			{
				_showVerifyCredentialsWarning = value;
				OnPropertyChanged();
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

		public string SelectedRegion
		{
			get => _selectedRegion;
			set
			{
				_selectedRegion = value;
				OnPropertyChanged();
				SelectedRegionUIMessage = value == Constants.CloudEUUrl ? "Selected region: EU"
										: value == Constants.CloudUSUrl ? "Selected region: US"
										: "No region selected";
			}
		}

		public string SelectedRegionUIMessage
		{
			get => _selectedRegionUIMessage;
			set
			{
				_selectedRegionUIMessage = value;
				OnPropertyChanged();
			}
		}

		public ICommand BackCommand { get; private set; }

		public ICommand ClearCommand { get; private set; }

		public ICommand SignInCommand { get; private set; }

		public ICommand Auth0SignInCommand { get; private set; }

		public ICommand OpenExternalUrlCommand { get; private set; }

		public ICommand SelectAuthenticationTypeCommand { get; private set; }

		public event EventHandler CloseRequested;

		public event EventHandler StartLoginProcess;

		public event EventHandler StopLoginProcess;

		private void InitializeCommands()
		{
			BackCommand = new RelayCommand(Back);
			ClearCommand = new RelayCommand(Clear);
			SignInCommand = new RelayCommand(SignIn);
			Auth0SignInCommand = new RelayCommand(Auth0SignIn);
			OpenExternalUrlCommand = new RelayCommand(OpenExternalUrl);
			SelectAuthenticationTypeCommand = new RelayCommand(SelectAuthenticationType);
		}

		private void LoadCredentials()
		{
			if (TranslationOptions.CloudCredentials is null)
			{
				SelectedRegion = Constants.CloudEUUrl;
				return;
			}

			SelectedRegion = TranslationOptions.CloudCredentials.AccountRegion ??= Constants.CloudEUUrl;
			UserName = TranslationOptions.CloudCredentials.UserName;
			UserPassword = TranslationOptions.CloudCredentials.UserPassword;
			ClientId = TranslationOptions.CloudCredentials.ClientID;
			ClientSecret = TranslationOptions.CloudCredentials.ClientSecret;
			ConnectionCode = TranslationOptions.CloudCredentials.ConnectionCode;
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

			StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Loading_Connecting));
			TranslationOptions.CloudCredentials ??= new();
			TranslationOptions.CloudCredentials.AccountRegion = SelectedRegion;
			TranslationOptions.AuthenticationType = AuthenticationType;
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
				Auth0SignInCommand?.Execute(ConnectionCode);
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				return;
			}

			var success = await CloudService.AuthenticateUser(TranslationOptions, AuthenticationType);
			if (!success)
			{
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				ShowVerifyCredentialsWarning = true;
				return;
			}

			CloseWindow();
		}

		private void Auth0SignIn(object parameter)
		{
			StartLoginProcess?.Invoke(this, new LoginEventArgs(PluginResources.Loading_Connecting));

			TranslationOptions.AuthenticationType = AuthenticationType.CloudSSO;
			AuthenticationType = AuthenticationType.CloudSSO;

			var auth0Config = new CloudAuth0Config(parameter as string, SelectedRegion);
			var cloudAuth0ViewModel = new CloudAuth0ViewModel(TranslationOptions, auth0Config);
			var cloudAuth0View = new CloudAuth0View() { DataContext = cloudAuth0ViewModel };
			cloudAuth0ViewModel.CloseAuth0Raised += () =>
			{
				cloudAuth0View.Close();
				StopLoginProcess?.Invoke(this, EventArgs.Empty);
				if (cloudAuth0ViewModel.IsConnected)
				{
					if (!string.IsNullOrEmpty(ConnectionCode))
					{
						TranslationOptions.CloudCredentials.ConnectionCode = ConnectionCode;
					}

					CloseWindow();
				}
			};

			cloudAuth0View.ShowDialog();
			StopLoginProcess?.Invoke(this, EventArgs.Empty);
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

		private void OpenExternalUrl(object parameter)
		{
			var targetUri = SelectedRegion.Equals(Constants.CloudEUUrl) ? Constants.LanguageWeaverEUPortal
						  : SelectedRegion.Equals(Constants.CloudUSUrl) ? Constants.LanguageWeaverUSPortal
						  : string.Empty;

			Process.Start(targetUri);
		}

		public void CloseWindow()
		{
			TranslationOptions.AuthenticationType = AuthenticationType;
			TranslationOptions.PluginVersion = PluginVersion.LanguageWeaverCloud;
			TranslationOptions.UpdateUri();
			CloseRequested?.Invoke(this, EventArgs.Empty);
		}
	}
}
