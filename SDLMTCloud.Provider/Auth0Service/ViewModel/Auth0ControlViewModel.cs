using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Auth0Service.Commands;
using Auth0Service.Model;
using Auth0Service.Service;

namespace Auth0Service.ViewModel
{
	public class Auth0ControlViewModel : ModelBase
	{
		private ICommand _logInOutCommand;
		private Uri _url;

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

		public void Logout()
		{
			Url = new Uri(AuthorizationService.GenerateLogoutUrl());
			AuthorizationService.Logout();

			OnPropertyChanged(nameof(IsLoggedIn));
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