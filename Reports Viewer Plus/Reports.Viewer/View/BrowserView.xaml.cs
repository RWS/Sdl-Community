using System.Windows.Controls;
using System.Windows.Input;
using CefSharp.Wpf;
using CefSharp;
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
            WebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
        }

        private void WebBrowser_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var browser = sender as ChromiumWebBrowser;
            if (browser != null)
            {
                e.Handled = true;

                ContextMenu contextMenu = this.Resources["CustomContextMenu"] as ContextMenu;
                if (contextMenu != null)
                {
                    // Show the context menu at the mouse position
                    contextMenu.PlacementTarget = browser;
                    contextMenu.IsOpen = true;
                }
            }
        }

        private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (DataContext is BrowserViewModel viewModel)
                {
                    viewModel.IsLoading = e.IsLoading;
                }
            });
        }
    }
}
