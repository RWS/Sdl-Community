using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WebView2Test.Commands;
using WebView2Test.Service;

namespace WebView2Test.ViewModel
{
	public class MainWindowViewModel : ModelBase
	{
		private ICommand _logInOutCommand;
		private Uri _url;

		public MainWindowViewModel(AuthorizationService authorizationService)
		{
			AuthorizationService = authorizationService;
			_ = AuthorizationService.EnsureLoggedIn().Result;
		}

		public bool IsLoggedIn => AuthorizationService.Credentials is not null;
		public ICommand LogInOutCommand => _logInOutCommand ??= new CommandHandler(LogInOut, true);

		public Uri Url
		{
			get => _url;
			set
			{
				_url = value;
				OnPropertyChanged();
			}
		}

		private AuthorizationService AuthorizationService { get; }

		public async Task<bool> Login(string uriQuery)
		{
			var login = await AuthorizationService.Login(uriQuery);
			OnPropertyChanged(nameof(IsLoggedIn));
			return login;
		}

		private void GoToLoginPage()
		{
			Url = new Uri(AuthorizationService.GenerateAuthorizationRequest());
		}

		private async void LogInOut()
		{
			if (IsLoggedIn)
			{
				await Logout();
			}
			else
			{
				GoToLoginPage();
			}
		}

		private async Task Logout()
		{
			Url = new Uri(AuthorizationService.GenerateLogoutUrl());
			await AuthorizationService.Logout();

			OnPropertyChanged(nameof(IsLoggedIn));
		}
	}
}