﻿using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LanguageWeaverProvider.ViewModel.Cloud;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace LanguageWeaverProvider.View.Cloud
{
	/// <summary>
	/// Interaction logic for CloudAuth0View.xaml
	/// </summary>
	public partial class CloudAuth0View : Window
	{
		public CloudAuth0View()
		{
			InitializeComponent();
		}

		private async void WebView2Browser_OnLoaded(object sender, RoutedEventArgs e)
		{
			await InitializeWebView(ViewModel.Auth0Config.LoginUri.ToString());
		}

		private CloudAuth0ViewModel ViewModel => DataContext as CloudAuth0ViewModel;

		private async Task InitializeWebView(string uri)
		{
			if (WebView2Browser.CoreWebView2 is null)
			{
				var userDataFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name);
				var options = new CoreWebView2EnvironmentOptions { AllowSingleSignOnUsingOSPrimaryAccount = true };
				var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);

				WebView2Browser.CreationProperties = new CoreWebView2CreationProperties
				{
					UserDataFolder = userDataFolder
				};

				await WebView2Browser.EnsureCoreWebView2Async(environment);
				await WebView2Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(BrowserScript);
			}

			WebView2Browser.CoreWebView2.Navigate(uri);
		}

		private async void WebView2Browser_OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
		{
			if (e.Uri?.StartsWith(ViewModel.Auth0Config.RedirectUri) == true)
			{
				WebView2Browser.Visibility = Visibility.Collapsed;
				await ViewModel.Navigated(e.Uri);
				WebView2Browser.Visibility = Visibility.Visible;
			}
		}

		internal const string BrowserScript = @"
            document.oncontextmenu = (e) => { return false; };
            document.onreadystatechange = () => {
                if (document.readyState != 'interactive') { return; }
                for (const htmlElement of document.getElementsByTagName('html')) {
                    htmlElement.style.background = '#ffffff';
                }
                for (const footerElement of document.getElementsByTagName('footer')) {
                    footerElement.style.visibility = 'collapse';
                    footerElement.style.padding = '0px';
                    footerElement.style.position = 'fixed';
                    footerElement.style.background = '#ffffff';
                }
            };";
	}
}