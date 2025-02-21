using System;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interface;
using MicrosoftTranslatorProvider.Interfaces;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class PrivateEndpointAuthenticationViewModel : BaseViewModel, IAuthenticationView
	{
		readonly ITranslationOptions _translationOptions;
		string _endpoint;

		public PrivateEndpointAuthenticationViewModel(ITranslationOptions translationOptions)
		{
			_translationOptions = translationOptions;
			InitializeCommands();
			LoadCredentials();
		}

		public string Endpoint
		{
			get => _endpoint;
			set
			{
				_endpoint = value;
				OnPropertyChanged();
			}
		}

		public ICommand ClearCommand { get; private set; }
		public ICommand SignInCommand { get; private set; }

		public event EventHandler CloseRequested;

		private void InitializeCommands()
		{
			ClearCommand = new RelayCommand(Clear);
			SignInCommand = new RelayCommand(SignIn);
		}

		private void LoadCredentials()
		{
			if (_translationOptions.PrivateEndpoint is null
			 || _translationOptions.PrivateEndpoint.Endpoint is not string endpoint)
			{
				return;
			}

			Endpoint = endpoint;
		}

		private void Clear(object parameter)
		{
			if (parameter is not string target)
			{
				return;
			}

			switch (target)
			{
				case nameof(Endpoint):
					Endpoint = string.Empty;
					break;
			}
		}

		private void SignIn(object parameter)
		{
			_translationOptions.PrivateEndpoint ??= new();
			_translationOptions.PrivateEndpoint.Endpoint = Endpoint;
			CloseRequested.Invoke(null, EventArgs.Empty);
		}
	}
}