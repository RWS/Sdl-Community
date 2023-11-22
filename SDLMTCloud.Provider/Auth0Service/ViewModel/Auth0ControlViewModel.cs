using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Auth0Service.Commands;
using Auth0Service.Helpers;
using Auth0Service.Model;
using Auth0Service.Service;

namespace Auth0Service.ViewModel
{
	public class Auth0ControlViewModel : ModelBase
	{
		private CommandHandler _deleteAllCookiesCommand;
		private CommandHandler _onNavigationCompletedCommand;
		private ICommand _onNavigationStartingCommand;
		private bool _showDialog;
		private Uri _url;
		private Visibility _visibility = Visibility.Collapsed;

		public Auth0ControlViewModel(AuthorizationService authorizationService)
		{
			AuthorizationService = authorizationService;
		}

		public AuthenticationResult AuthenticationResult { get; set; }
		public AuthorizationService AuthorizationService { get; }

		public ICommand DeleteAllCookiesCommand
		{
			get => _deleteAllCookiesCommand;
			set => _deleteAllCookiesCommand = (CommandHandler)value;
		}

		public ICommand OnNavigationCompletedCommand =>
			_onNavigationCompletedCommand ??= new CommandHandler(OnNavigationCompleted, true);

		public ICommand OnNavigationStartingCommand =>
			_onNavigationStartingCommand ??= new CommandHandler(OnNavigationStarting, true);

		public Uri Url
		{
			get => _url;
			set
			{
				_url = value;
				OnPropertyChanged();
			}
		}

		public Visibility Visibility
		{
			get => _visibility;
			set
			{
				if (value == _visibility) return;
				_visibility = value;
				OnPropertyChanged();
			}
		}

		public AuthenticationResult EnsureLoggedIn()
		{
			var authenticationResult = AuthorizationService.AreCredentialsValid();
			if (authenticationResult.IsSuccessful) return authenticationResult;

			GoToLoginPage();
			return authenticationResult;
		}

		public void Logout()
		{
			Visibility = Visibility.Collapsed;

			Url = new Uri(AuthorizationService.GenerateLogoutUrl());
			AuthorizationService.Logout();

			DeleteAllCookiesCommand?.Execute(null);
		}

		public void OnNavigationStarting()
		{
			Visibility = Visibility.Collapsed;
		}

		public (AuthenticationResult, Credential) TryLogin(CancellationToken cancellationToken, bool showDialog = false, Credential credentials = null)
		{
			_showDialog = showDialog;
			if (credentials is not null) AuthorizationService.Credentials = credentials;

			AuthenticationResult = EnsureLoggedIn();

			while (showDialog && (!AuthenticationResult.IsSuccessful) && !cancellationToken.IsCancellationRequested)
			{
				Application.Current.DoEvents();
			}

			return (AuthenticationResult, AuthorizationService.Credentials);
		}

		internal AuthenticationResult Login(string uriQuery)
		{
			return AuthorizationService.Login(uriQuery);
		}

		private void GoToLoginPage()
		{
			Url = new Uri(AuthorizationService.GenerateAuthorizationRequest());
		}

		private void OnNavigationCompleted()
		{
			if (Url.LocalPath == "/" && !string.IsNullOrWhiteSpace(Url.Query))
			{
				AuthenticationResult = Login(Url.Query);
				if (!AuthenticationResult.IsSuccessful) return;
			}

			if (Url.LocalPath == "/login" && _showDialog) Visibility = Visibility.Visible;
		}
	}
}