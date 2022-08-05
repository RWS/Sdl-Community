using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Auth0Service.Helpers;
using Auth0Service.Model;
using Auth0Service.Service;
using Auth0Service.ViewModel;
using Microsoft.Web.WebView2.Core;

namespace Auth0Service
{
	public partial class Auth0Control : Window
	{
		private bool _authenticationTerminated;
		private string _webView2UserDataFolder;

		public Auth0Control()
		{
			InitializeComponent();

			var authorizationSettings = new AuthorizationSettings
			(
				"F4NpOGG1sBaEzk379M6ZxX3gGa0iH1Ff",
				"https://api.sdl.com",
				"https://sdl-prod.eu.auth0.com"
			);

			var authService = new AuthorizationService(new LoginGeneratorsHelper(), new CredentialRepository(), authorizationSettings);
			Initialization = InitializeAsync(authService);
		}

		public Auth0ControlViewModel Auth0 { get; set; }

		public Task Initialization { get; set; }

		private AuthenticationResult AuthenticationResult { get; set; }

		public async Task InitializeAsync(AuthorizationService authorizationService)
		{
			Auth0 = new Auth0ControlViewModel(authorizationService);
			DataContext = Auth0;


			var options = new CoreWebView2EnvironmentOptions(null, null, null, true);
			_webView2UserDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"Trados AppStore", "Language Weaver", "WebView2Data");

			var envir = await CoreWebView2Environment.CreateAsync(null, _webView2UserDataFolder, options);
			await webView.EnsureCoreWebView2Async(envir);
		}

		public async void Logout()
		{
			Visibility = Visibility.Hidden;
			Show();
			var environment = CoreWebView2Environment.CreateAsync(null, _webView2UserDataFolder).Result;
			await webView.EnsureCoreWebView2Async(environment);

			Auth0.Logout();
			webView.CoreWebView2?.CookieManager.DeleteAllCookies();
		}

		public (AuthenticationResult, Credential) TryLogin(bool showDialog = false)
		{
			_authenticationTerminated = false;

			if (showDialog) Show();

			AuthenticationResult = EnsureLoggedIn();

			while (showDialog && !_authenticationTerminated)
			{
				Application.Current.DoEvents();
			}

			return (AuthenticationResult, Auth0.AuthorizationService.Credentials);
		}

		private AuthenticationResult EnsureLoggedIn()
		{
			return Auth0.EnsureLoggedIn();
		}

		private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
		{
			var uri = webView.Source;
			if (uri.LocalPath == "/" && !string.IsNullOrWhiteSpace(uri.Query))
			{
				webView.Visibility = Visibility.Collapsed;
				var script = "document.body.style.overflow ='hidden'";
				await webView.ExecuteScriptAsync(script);

				AuthenticationResult = Auth0.Login(uri.Query);
				if (!AuthenticationResult.IsSuccessful) return;

				Close();
			}

			if (uri.LocalPath == "/login") Visibility = Visibility.Visible;
		}

		private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
		{
			Visibility = Visibility.Hidden;
		}

		private void Window_Closed(object sender, System.EventArgs e)
		{
			_authenticationTerminated = true;
		}
	}
}