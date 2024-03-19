using System;
using System.Collections.Generic;
using System.Windows.Input;
using LanguageMappingProvider.Model;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interface;
using MicrosoftTranslatorProvider.Interfaces;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class AuthenticationViewModel : BaseModel
	{
		ITranslationOptions _translationOptions;
		
		AuthenticationType _authenticationType;
		Dictionary<AuthenticationType, IAuthenticationView> _authenticationViews;

		public AuthenticationViewModel(ITranslationOptions translationOptions)
		{
			_translationOptions = translationOptions;
			InitializeCommands();
			InitializeUserControls();
		}

		public bool SaveChanges { get; private set; }

		public AuthenticationType AuthenticationType
		{
			get => _authenticationType;
			set
			{
				if (_authenticationType == value) return;
				_authenticationType = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SelectedAuthenticationView));
				OnPropertyChanged(nameof(IsAuthenticationMethodSelected));
			}
		}

		public IAuthenticationView SelectedAuthenticationView => _authenticationViews[_authenticationType];

		public bool IsAuthenticationMethodSelected => SelectedAuthenticationView is not null;

		public ICommand SelectMicrosoftServiceCommand { get; private set; }

		public ICommand ExitApplicationCommand { get; private set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			SelectMicrosoftServiceCommand = new RelayCommand(SelectMicrosoftService);
			ExitApplicationCommand = new RelayCommand(ExitApplication);
		}

		private void InitializeUserControls()
		{
			var microsoftAuthenticationViewModel = new MicrosoftAuthenticationViewModel(_translationOptions);
			microsoftAuthenticationViewModel.CloseRequested += CloseCredentialsViewRequest;

			_authenticationViews = new()
			{
				{ AuthenticationType.None, null },
				{ AuthenticationType.Microsoft, microsoftAuthenticationViewModel },
				{ AuthenticationType.PrivateEndpoint, null }
			};
		}

		private void SelectMicrosoftService(object parameter)
		{
			if (parameter is not AuthenticationType authenticationType)
			{
				return;
			}

			AuthenticationType = authenticationType;
		}

		private void ExitApplication(object parameter)
		{
			SaveChanges = false;
			CloseEventRaised?.Invoke();
		}

		private void CloseCredentialsViewRequest(object sender, EventArgs e)
		{
			SaveChanges = true;
			CloseEventRaised?.Invoke();
		}
	}
}