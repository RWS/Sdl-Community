using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using WebView2Test.Helpers;
using WebView2Test.Model;
using WebView2Test.Service;
using WebView2Test.ViewModel;

namespace WebView2Test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string LogIn = "Log in";
		private const string LogOut = "Log out";

		public MainWindow()
		{
			InitializeComponent();
			Initialize();
		}

		private async Task Initialize()
		{
			await webView.EnsureCoreWebView2Async();
			webView.NavigationCompleted += WebView_NavigationCompleted;
		}

		private async void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
		{
			var uri = webView.Source;
			if (uri.LocalPath == "/" && !string.IsNullOrWhiteSpace(uri.Query))
			{
				webView.Visibility = Visibility.Collapsed;
				var script = "document.body.style.overflow ='hidden'";
				await webView.ExecuteScriptAsync(script);

				var loggedIn = await ((MainWindowViewModel)DataContext).Login(uri.Query);
				if (!loggedIn) return;

				webView.NavigateToString("Logged in");
			}

			if (uri.LocalPath == "/login") webView.Visibility = Visibility.Visible;
		}

		private void LogInOutButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Content.ToString() == LogOut) webView.CoreWebView2.CookieManager.DeleteAllCookies();
		}
	}
}