using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Auth0Service.Helpers;
using Auth0Service.Model;
using Auth0Service.Service;
using Auth0Service.ViewModel;
using Microsoft.Web.WebView2.Core;
using UserControl = System.Windows.Controls.UserControl;

namespace Auth0Service
{
	public partial class Auth0Control : UserControl, IDisposable
	{
		private HttpHelper _httpHelper;
		private string _webView2UserDataFolder;

		public Auth0Control()
		{
			InitializeComponent();
			DataContextChanged += webView.Initialize;
			_httpHelper = new HttpHelper();

			SetViewModel();
			WebViewInitialization = InitializeAsync();
		}

		public Auth0ControlViewModel Auth0Service { get; set; }

		public Task WebViewInitialization { get; set; }

		public void Dispose()
		{
			_httpHelper?.Dispose();
		}

		public async Task InitializeAsync()
		{
			var options = new CoreWebView2EnvironmentOptions(null, null, null, true);
			_webView2UserDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"Trados AppStore", "Language Weaver", "WebView2Data");

			var envir = await CoreWebView2Environment.CreateAsync(null, _webView2UserDataFolder, options);
			await webView.EnsureCoreWebView2Async(envir);

			webView.IsVisibleChanged += CustomWebView_IsVisibleChanged;
		}

		public void SetViewModel()
		{
			var path = Assembly.GetExecutingAssembly().Location;
			var config = ConfigurationManager.OpenExeConfiguration(path);

			var authorizationSettings = new AuthorizationSettings
				(
					config.AppSettings.Settings["ClientId"].Value,
					config.AppSettings.Settings["Audience"].Value,
					config.AppSettings.Settings["Auth0Url"].Value
				);

			var authService = new AuthorizationService(new LoginGeneratorsHelper(), authorizationSettings, _httpHelper);
			Auth0Service = new Auth0ControlViewModel(authService);

			DataContext = Auth0Service;
		}

		private void CustomWebView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue && IsInitialized) RemoveScrolls();
		}

		private async void RemoveScrolls()
		{
			var script = "document.body.style.overflow ='hidden'";
			await webView.ExecuteScriptAsync(script);
		}
	}
}