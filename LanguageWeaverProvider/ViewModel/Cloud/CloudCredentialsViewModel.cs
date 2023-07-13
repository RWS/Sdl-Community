using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model.Options.Interface;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudCredentialsViewModel : BaseViewModel
	{
		private string _userId;
		private string _userPassword;
		private string _clientId;
		private string _clientSecret;

		private ICommand _signInCommand;
		private ICommand _signOutCommand;

		public CloudCredentialsViewModel(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
			InitializeCommands();
		}

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

		}

		private void SignOut(object parameter)
		{

		}
	}
}