using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudMainViewModel : BaseViewModel, IMainProviderViewModel
	{
		private IMainProviderViewModel _authenticationView;

		private bool _isCredentialsSelected;
		private bool _isSecretSelected;

		public CloudMainViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
		}

		public ITranslationOptions TranslationOptions { get; set; }

		private bool IsCredentialsSelected
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

		private bool IsSecretSelected
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

		public IMainProviderViewModel AuthenticationView
		{
			get => _authenticationView;
			set
			{
				_authenticationView = value;
				OnPropertyChanged();
			}
		}

		public ICommand SelectAuthenticationTypeCommand { get; private set; }

		private void InitializeCommands()
		{
			SelectAuthenticationTypeCommand = new RelayCommand(SelectAuthenticationType);
		}

		private void SelectAuthenticationType(object parameter)
		{
			if (parameter is not string requestedAuthenticationType)
			{
				return;
			}

			IsCredentialsSelected = requestedAuthenticationType == "Credentials";
			IsSecretSelected = requestedAuthenticationType == "Secret";
			AuthenticationView = new CloudCredentialsViewModel(TranslationOptions, IsCredentialsSelected ? AuthenticationType.Credentials : AuthenticationType.Secret);
		}
	}
}