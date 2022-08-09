using System;
using System.IO;
using System.Threading.Tasks;
using Auth0Service.Helpers;
using Auth0Service.Model;
using Auth0Service.Service;
using Auth0Service.ViewModel;
using Microsoft.Web.WebView2.Core;
using UserControl = System.Windows.Controls.UserControl;

namespace Auth0Service
{
	public partial class Auth0Control : UserControl
	{
		private string _webView2UserDataFolder;

		public Auth0Control()
		{
			InitializeComponent();
			DataContextChanged += webView.Initialize;

			SetViewModel();
			WebViewInitialization = InitializeAsync();
		}

		public Auth0ControlViewModel Auth0Service { get; set; }

		public Task WebViewInitialization { get; set; }

		public async Task InitializeAsync()
		{
			var options = new CoreWebView2EnvironmentOptions(null, null, null, true);
			_webView2UserDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"Trados AppStore", "Language Weaver", "WebView2Data");

			var envir = await CoreWebView2Environment.CreateAsync(null, _webView2UserDataFolder, options);
			await webView.EnsureCoreWebView2Async(envir);
		}

		private void SetViewModel()
		{
			var authorizationSettings = new AuthorizationSettings
			(
				"F4NpOGG1sBaEzk379M6ZxX3gGa0iH1Ff",
				"https://api.sdl.com",
				"https://sdl-prod.eu.auth0.com"
			);
			var authService = new AuthorizationService(new LoginGeneratorsHelper(), authorizationSettings);
			Auth0Service = new Auth0ControlViewModel(authService);

			DataContext = Auth0Service;
		}
	}
}