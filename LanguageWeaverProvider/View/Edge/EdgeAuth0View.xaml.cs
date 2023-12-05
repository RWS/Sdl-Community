using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.ViewModel.Cloud;
using LanguageWeaverProvider.ViewModel.Edge;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace LanguageWeaverProvider.View.Edge
{
	/// <summary>
	/// Interaction logic for EdgeAuth0View.xaml
	/// </summary>
	public partial class EdgeAuth0View : Window
	{
		public EdgeAuth0View()
		{
			InitializeComponent();
		}

		private async void WebView2Browser_OnLoaded(object sender, RoutedEventArgs e)
		{
			await InitializeWebView();
		}

		private EdgeAuth0ViewModel ViewModel => DataContext as EdgeAuth0ViewModel;

		private async Task InitializeWebView()
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

			await ViewModel.Connect(WebView2Browser);
			Close();
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
