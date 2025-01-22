using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.ReportViewer.Controls
{
    /// <summary>
    /// Interaction logic for ReportExplorer.xaml
    /// </summary>
    public partial class Report : UserControl, IUIControl
    {
        public Report()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
        }

        public void Navigate(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var htmlContent = "<html><body><div style='display:flex;justify-content:center;align-items:center;height:100vh;font-size:24px;'>Please select a report</div></body></html>";
                WebView2Browser.CoreWebView2?.NavigateToString(htmlContent);
            }
            else
            {
                WebView2Browser.CoreWebView2?.Navigate(path);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {

        }

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
                //await WebView2Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(BrowserScript);
            }

            WebView2Browser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            Navigate(null);
        }

        private async void WebView2Browser_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await InitializeWebView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}