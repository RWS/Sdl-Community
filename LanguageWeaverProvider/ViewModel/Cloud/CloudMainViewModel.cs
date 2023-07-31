using System;
using System.Security;
using System.Security.Principal;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.NewFolder;
using LanguageWeaverProvider.ViewModel.Interface;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudMainViewModel : BaseViewModel, IMainProviderViewModel
	{
		private string _userId;
		private string _userPassword;
		private string _clientId;
		private string _clientSecret;

		private bool _isCredentialsSelected;
		private bool _isSecretSelected;

		public CloudMainViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
		}

		public ITranslationOptions TranslationOptions { get; set; }

		public bool IsCredentialsSelected
		{
			get => _isCredentialsSelected;
			set
			{
				if (_isCredentialsSelected == value) return;
				_isCredentialsSelected = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsAuthenticationTypeSelected));
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
				OnPropertyChanged(nameof(IsAuthenticationTypeSelected));
			}
		}

		public bool IsAuthenticationTypeSelected => IsCredentialsSelected || IsSecretSelected;

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

		private void InitializeCommands()
		{
			SelectAuthenticationTypeCommand = new RelayCommand(SelectAuthenticationType);
			SignInCommand = new RelayCommand(SignIn);
			ClearCommand = new RelayCommand(Clear);
		}

		private void SelectAuthenticationType(object parameter)
		{
			if (parameter is not string requestedAuthenticationType)
			{
				return;
			}

			IsCredentialsSelected = requestedAuthenticationType == "Credentials";
			IsSecretSelected = requestedAuthenticationType == "Secret";
		}

		private async void SignIn(object parameter)
		{
			var cloudCredentials = new CloudCredentials();
			if (IsCredentialsSelected)
			{
				cloudCredentials = new CloudCredentials()
				{
					UserID = _userId,
					UserPassword = _userPassword
				};
			}
			else if (IsSecretSelected)
			{

				cloudCredentials = new CloudCredentials()
				{
					ClientID = _clientId,
					ClientSecret = _clientSecret
				};
			}

			var response = await CloudService.AuthenticateUser(cloudCredentials, IsCredentialsSelected);
			cloudCredentials.AccessToken = JsonConvert.DeserializeObject<AccessToken>(response);
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
	}
}