using NLog;
using TradosStudio.API.UI;
using TradosStudio.API.UI.View;
using System;
using System.Threading.Tasks;

namespace Sdl.Community.IATETerminologyProvider.View
{
    internal class SearchResultsView : IView
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private SearchResultsControl _contentControl;

        public string Id => "IATE Results Viewer";

        public string Name => "Search Results Viewer";

        public string Description => "IATE Search Results";

        public string Icon => $"Sdl.Community.IATETerminologyProvider.Resources.{nameof(PluginResources.Iate_logo)}.ico";

        public bool Available => true;

        public bool Enabled => true;

        public static bool IsInitialized { get; set; }

        public BrowserWindow Browser => _contentControl.Browser;

        public string Url { get; set; }

        public void Activate()
        {
            
        }

        public bool Deactivate()
        {
            return true;
        }

        public void Dispose()
        {
            /// clean up resources if needed
        }

        public IUIControl GetContentControl()
        {
            if(_contentControl == null)
            {
                _contentControl = new SearchResultsControl();
            }

            return _contentControl;
        }

        public IUIControl GetExplorerBarControl()
        {
           return null;
        }

        public void OnInit()
        {
            GetContentControl();
            InitializeBrowserAsync();
        }

        private async void InitializeBrowserAsync()
        {
            try
            {
                await Browser.InitializeWebView();
                IsInitialized = true;

                if (!string.IsNullOrWhiteSpace(Url))
                {
                    await Browser.NavigateAsync(Url);
                }
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                Logger.Error(ex, "Failed to initialize the IATE results WebView.");
            }
        }

        public void NavigateTo(string url)
        {
            Url = url;
            NavigateAsync();
        }

        private async void NavigateAsync()
        {
            try
            {
                await Browser.NavigateAsync(Url);
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to navigate the IATE results WebView.");
            }
        }
    }
}
