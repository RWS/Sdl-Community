using NLog;
using System;
using System.ComponentModel;
using TradosStudio.API.UI;
using TradosStudio.API.UI.View;

namespace Sdl.Community.IATETerminologyProvider.View
{
    public class SearchResultsViewPart : ViewPartBase, IViewPart
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private SearchResultsControl _contentControl;

        public static SearchResultsViewPart ActiveInstance { get; private set; }
        public string Url { get; set; }
        public static bool IsInitialized { get; set; }
        public BrowserWindow Browser => _contentControl.Browser;
              
        public IUIControl GetControl()
        {
            if (_contentControl == null)
            {
                _contentControl = new SearchResultsControl();
            }

            return _contentControl;
        }

        public void OnActivate()
        {
            // Implement activation logic here
        }

        public bool OnDeactivate()
        {
            // Implement deactivation logic here
            return true;
        }

        public void OnDispose()
        {
            // Implement disposal logic here
            ClearActiveInstance();
        }

        public bool OnHide()
        {
            // Implement hide logic here
            return true;
        }

        public void OnInit()
        {
            // Implement initialization logic here
        }

        public bool OnRemove()
        {
            // Implement remove logic here
            ClearActiveInstance();
            return true;
        }

        public void OnShow()
        {
            ActiveInstance = this;
            GetControl();
            InitializeBrowserAsync();
        }

        private void ClearActiveInstance()
        {
            if (ReferenceEquals(ActiveInstance, this))
            {
                ActiveInstance = null;
            }
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
                if (_contentControl == null)
                {
                    GetControl();
                }
                _contentControl.Show();
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
