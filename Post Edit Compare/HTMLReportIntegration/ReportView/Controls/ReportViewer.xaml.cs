using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls
{
    /// <summary>
    /// Interaction logic for ReportExplorer.xaml
    /// </summary>
    public partial class ReportViewer : UserControl, IUIControl
    {
        public ReportViewer()
        {
            InitializeComponent();
        }

        public event Action<object, CoreWebView2WebMessageReceivedEventArgs> WebMessageReceived;

        public void Dispose()
        {
        }

        public async Task<string> GetLoadedReport()
        {
            try
            {
                var script = "document.documentElement.outerHTML;";
                var result = await WebView2Browser.ExecuteScriptAsync(script);

                // The returned string is JSON-encoded, so we need to decode it
                return JsonConvert.DeserializeObject<string>(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving updated HTML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<string> GetNonInteractiveReport()
        {
            try
            {
                var result = await WebView2Browser.ExecuteScriptAsync("getCleanedHTMLForExport();");
                return JsonConvert.DeserializeObject<string>(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving updated HTML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task Navigate(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var htmlContent = "<html><body><div style='display:flex;justify-content:center;align-items:center;height:100vh;font-size:24px;'>Please select a report</div></body></html>";
                WebView2Browser.CoreWebView2?.NavigateToString(htmlContent);
            }
            else
            {
                await LoadScripts();
                WebView2Browser.CoreWebView2?.Navigate(path);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            WebMessageReceived?.Invoke(sender, e);
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
            }

            Navigate(null);
        }

        private async Task LoadScripts()
        {
            var scripts = File.ReadAllText(
                @"C:\Code\SDL\GitHub repos\RWS Community\Post Edit Compare\bin\Debug\net48\HTMLReportIntegration\ReportView\Controls\scripts.js");
            try
            {
                var result = await WebView2Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(scripts);
            }
            catch
            {
                MessageBox.Show("Failed to load scripts", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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