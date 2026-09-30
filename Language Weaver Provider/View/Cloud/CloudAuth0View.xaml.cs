using System;
using System.Threading.Tasks;
using System.Windows;
using LanguageWeaverProvider.Helpers;
using LanguageWeaverProvider.ViewModel.Cloud;

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
            this.FollowStudioTheme();
        }

        private async void WebView2Browser_OnLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebView(ViewModel.Auth0Config.LoginUri.ToString());
        }

        private CloudAuth0ViewModel ViewModel => DataContext as CloudAuth0ViewModel;

        private async Task InitializeWebView(string uri)
        {
            try
            {
                await WebView2ThemeHelper.InitializeThemeAsync(WebView2Browser, "CloudWebView");

                WebView2Browser.CoreWebView2.CookieManager.DeleteAllCookies();
                WebView2Browser.CoreWebView2.Navigate(uri);
            }
            catch (Exception)
            {
                if (!IsLoaded) return;
                throw;
            }
        }

        private async void WebView2Browser_OnNavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri?.StartsWith(ViewModel.Auth0Config.RedirectUri) == true)
            {
                WebView2Browser.Visibility = Visibility.Collapsed;
                await ViewModel.Navigated(e.Uri);
                WebView2Browser.Visibility = Visibility.Visible;
            }
        }
    }
}