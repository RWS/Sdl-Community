using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.NewFolder;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudCredentialsViewModel : BaseViewModel, IMainProviderViewModel
	{
		private AuthenticationType _authenticationType;

		private string _userId;
		private string _userPassword;
		private string _clientId;
		private string _clientSecret;

		public CloudCredentialsViewModel(ITranslationOptions translationOptions, AuthenticationType authenticationType)
		{
			AuthenticationType = authenticationType;
			TranslationOptions = translationOptions;
			InitializeCommands();
		}

		public AuthenticationType AuthenticationType
		{
			get => _authenticationType;
			set
			{
				_authenticationType = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(AuthenticationType));
			}
		}

		public bool IsCredentialsSelected => AuthenticationType == AuthenticationType.Credentials;

		public ITranslationOptions TranslationOptions { get; set; }

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

		public ICommand SignInCommand { get; private set; }

		public ICommand SignOutCommand { get; private set; }

		private void InitializeCommands()
		{
			SignInCommand = new RelayCommand(SignIn);
			SignOutCommand = new RelayCommand(SignOut);
		}

		private void SignIn(object parameter)
		{
			var cloudCredentials = new CloudCredentials()
			{
				UserID = IsCredentialsSelected ? _userId : null,
				UserPassword = IsCredentialsSelected ? _userPassword : null,
				ClientID = IsCredentialsSelected ? null : _clientId,
				ClientSecret = IsCredentialsSelected ? null :_clientSecret
			};

			CloudService.Authenticate(cloudCredentials);
		}

		private void SignOut(object parameter)
		{

		}
	}
}