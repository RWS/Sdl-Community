using System;
using System.Threading.Tasks;
using System.Windows;
using LanguageWeaverProvider.Helpers;
using LanguageWeaverProvider.ViewModel.Edge;

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
            this.FollowStudioTheme();
        }

        private async void WebView2Browser_OnLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebView();
        }

        private EdgeAuth0ViewModel ViewModel => DataContext as EdgeAuth0ViewModel;

        private async Task InitializeWebView()
        {
            try
            {
                await WebView2ThemeHelper.InitializeThemeAsync(WebView2Browser, "EdgeWebView");
                await ViewModel.Connect(WebView2Browser);

                Close();
            }
            catch (Exception)
            {
                if (IsLoaded)
                {
                    throw;
                }
            }       
        }
    }
}