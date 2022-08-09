using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
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
		private Uri _url;
		private ICommand _onNavigationStartingCommand;
		private Visibility _visibility = Visibility.Collapsed;
		private CommandHandler _onNavigationCompletedCommand;
		private bool _authenticationTerminated;
		private CommandHandler _goBackCommand;

		public Auth0ControlViewModel(AuthorizationService authorizationService)
		{
			AuthorizationService = authorizationService;
		}

		public AuthorizationService AuthorizationService { get; }
		public bool IsLoggedIn => AuthorizationService.Credentials is not null;

		public Uri Url
		{
			get => _url;
			set
			{
				_url = value;
				OnPropertyChanged();
			}
		}

		public (AuthenticationResult, Credential) TryLogin(bool showDialog = false)
		{
			_authenticationTerminated = false;

			if (showDialog) Visibility = Visibility.Visible;

			AuthenticationResult = EnsureLoggedIn();

			while (showDialog && !_authenticationTerminated)
			{
				Application.Current.DoEvents();
			}

			return (AuthenticationResult, AuthorizationService.Credentials);
		}

		public void Logout()
		{
			Visibility = Visibility.Collapsed;

			Url = new Uri(AuthorizationService.GenerateLogoutUrl());
			AuthorizationService.Logout();

			OnPropertyChanged(nameof(IsLoggedIn));

			DeleteAllCookiesCommand?.Execute(null);
		}

		public AuthenticationResult AuthenticationResult { get; set; }

		public ICommand OnNavigationStartingCommand =>
			_onNavigationStartingCommand ??= new CommandHandler(OnNavigationStarting, true);
		
		public ICommand OnNavigationCompletedCommand =>
			_onNavigationCompletedCommand ??= new CommandHandler(OnNavigationCompleted, true);

		public ICommand DeleteAllCookiesCommand
		{
			get => _goBackCommand;
			set => _goBackCommand = (CommandHandler)value;
		}

		private void OnNavigationCompleted()
		{
			if (Url.LocalPath == "/" && !string.IsNullOrWhiteSpace(Url.Query))
			{
				Visibility = Visibility.Collapsed;
				//var script = "document.body.style.overflow ='hidden'";
				//await webView.ExecuteScriptAsync(script);

				AuthenticationResult = Login(Url.Query);
				if (!AuthenticationResult.IsSuccessful) return;

				Visibility = Visibility.Collapsed;
				_authenticationTerminated = true;
			}

			if (Url.LocalPath == "/login") Visibility = Visibility.Visible;
		}

		public void OnNavigationStarting()
		{
			Visibility = Visibility.Collapsed;
		}

		public Visibility Visibility
		{
			get => _visibility;
			set
			{
				_visibility = value; 
				OnPropertyChanged();
			}
		}

		//public static Task<Auth0ControlViewModel> CreateAsync(AuthorizationService authorizationService)
		//{
		//	var ret = new Auth0ControlViewModel(authorizationService);
		//	return ret.InitializeAsync();
		//}

		internal AuthenticationResult Login(string uriQuery)
		{
			var login = AuthorizationService.Login(uriQuery);
			OnPropertyChanged(nameof(IsLoggedIn));
			return login;
		}

		private void GoToLoginPage()
		{
			Url = new Uri(AuthorizationService.GenerateAuthorizationRequest());
		}

		public AuthenticationResult EnsureLoggedIn()
		{
			var authenticationResult = AuthorizationService.AreCredentialsValid();
			if (authenticationResult.IsSuccessful) return authenticationResult;

			GoToLoginPage();
			return authenticationResult;
		}
	}
}