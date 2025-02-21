using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Sdl.Community.IATETerminologyProvider.ViewModel;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System.Windows.Controls;

namespace Sdl.Community.IATETerminologyProvider.View
{
	/// <summary>
	/// Interaction logic for SearchResults.xaml
	/// </summary>
	public partial class SearchResultsControl : UserControl, IUIControl
	{

		public SearchResultsControl()
		{
			InitializeComponent();
		}
		public void Dispose()
		{
		}
		private async void WebView2Browser_OnLoaded(object sender, RoutedEventArgs e)
		{
			await InitializeWebView(ViewModel.Uri ?? "about:blank");
		}
		private BrowserViewModel ViewModel => DataContext as BrowserViewModel;
		public WebView2 Browser => WebView2Browser;

		private async Task InitializeWebView(string uri)
		{
			try
			{
				if (WebView2Browser.CoreWebView2 == null)
				{
					// Necessary due to permissions on non-development machines.
					WebView2Browser.CreationProperties = new CoreWebView2CreationProperties
					{
						UserDataFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name)
					};

					await WebView2Browser.EnsureCoreWebView2Async();
					_ = await WebView2Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(BrowserScript);
				}

				if (ViewModel.NeedsClearingCookies)
				{
					WebView2Browser.CoreWebView2.CookieManager.DeleteAllCookies();
					ViewModel.NeedsClearingCookies = false;
				}

				WebView2Browser.CoreWebView2.Navigate(uri);
			}
			catch(Exception ex)
			{

			}
			
		}
		private void WebView2Browser_OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
		{
			if (e.Uri?.StartsWith(ViewModel.RedirectUri) == true)
			{
				ViewModel?.ExtractAuthorizationCode(new Uri(e.Uri, UriKind.Absolute));
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
