using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Reports.Viewer.Plus.ViewModel;

namespace Reports.Viewer.Plus.View
{
    /// <summary>
    /// Interaction logic for ProjectFilesView.xaml
    /// </summary>
    public partial class BrowserView : UserControl
    {
        private readonly BrowserViewModel _viewModel;

        public BrowserView(BrowserViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            WebBrowser.NavigationStarting += WebBrowser_NavigationStarting;
            WebBrowser.NavigationCompleted += WebBrowser_NavigationCompleted;
        }

        public void Navigate(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            if (WebBrowser.CoreWebView2 == null)
            {
                WebBrowser.EnsureCoreWebView2Async().ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() => WebBrowser.CoreWebView2.Navigate(address));
                });
            }
            else
            {
                WebBrowser.CoreWebView2.Navigate(address);
            }
        }

        public void Print()
        {
            WebBrowser.CoreWebView2?.ShowPrintUI(CoreWebView2PrintDialogKind.System);
        }

        private void WebBrowser_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var browser = sender as WebView2;
            if (browser != null)
            {
                e.Handled = true;

                ContextMenu contextMenu = this.Resources["CustomContextMenu"] as ContextMenu;
                if (contextMenu != null)
                {
                    contextMenu.PlacementTarget = browser;
                    contextMenu.IsOpen = true;
                }
            }
        }

        private void WebBrowser_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (DataContext is BrowserViewModel viewModel)
                {
                    viewModel.IsLoading = true;
                }
            });
        }

        private void WebBrowser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (DataContext is BrowserViewModel viewModel)
                {
                    viewModel.IsLoading = false;
                }
            });
        }
    }
}
