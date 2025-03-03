using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using Sdl.Community.PostEdit.Compare.Core.Reports;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.FileTypeSupport.Framework.Bilingual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
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

        private CoreWebView2Environment Environment { get; set; }

        public void Dispose()
        {
        }

        public async Task<List<ReportSegment>> GetAllSegments()
        {
            try
            {
                var result = await WebView2Browser.ExecuteScriptAsync("collectSegmentsDataFromHTML();");
                var reportSegments = JsonConvert.DeserializeObject<List<ReportSegment>>(result);
                return reportSegments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving the segments of the HTML report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task<string> GetLoadedReport()
        {
            try
            {
                var script = "document.documentElement.outerHTML;";
                var result = await WebView2Browser.RunScript(script);

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

        public async Task<string> GetProjectId()
        {
            try
            {
                var result = await WebView2Browser.ExecuteScriptAsync("getProjectId();");
                return JsonConvert.DeserializeObject<string>(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting the project ID from the HTML report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public async Task HideAllSegments()
        {
            try
            {
                await WebView2Browser.ExecuteScriptAsync("hideAllSegments();");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error hiding the segments of the HTML report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public async Task ShowAllSegments()
        {
            try
            {
                await WebView2Browser.ExecuteScriptAsync("showAllSegments();");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing the requested segments of the HTML report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task ShowSegments(List<(string, string)> segmentAndFileIds)
        {
            try
            {
                List<dynamic> segments = [];
                foreach (var segmentAndFileId in segmentAndFileIds)
                    segments.Add(new { segmentId = segmentAndFileId.Item1, fileId = segmentAndFileId.Item2 });

                var segmentsJson = JsonConvert.SerializeObject(segments);
                await WebView2Browser.ExecuteScriptAsync($"showSegments({segmentsJson});");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing the requested segments of the HTML report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task UpdateComments(List<CommentInfo> comments, string segmentId, string fileId, AddReplace addReplace = AddReplace.Replace)
        {
            var commentsJson = JsonConvert.SerializeObject(comments, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            var functionName = addReplace.HasFlag(AddReplace.Replace)
                ? "replaceCommentsForSegment"
                : "addCommentsForSegment";

            var script = $"{functionName}('{segmentId}', {commentsJson}, '{fileId}');";

            try
            {
                await WebView2Browser.Dispatcher.Invoke(async () =>
                {
                    await WebView2Browser.ExecuteScriptAsync(script);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public async Task UpdateStatus(string newStatus, string segmentId, string fileId)
        {
            object[] parameters = [segmentId, fileId, newStatus];
            var serializedParams = new List<string>();
            foreach (var param in parameters)
            {
                var paramJson = JsonConvert.SerializeObject(param, new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
                serializedParams.Add(paramJson);
            }

            var paramsJoined = string.Join(", ", serializedParams);
            var script = $"{"updateSegmentStatus"}({paramsJoined});";

            try
            {
                await WebView2Browser.Dispatcher.Invoke(async () =>
                {
                    await WebView2Browser.ExecuteScriptAsync(script);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            WebMessageReceived?.Invoke(sender, e);
        }

        private async Task EnsureBrowserIsLoaded()
        {
            await WebView2Browser.EnsureCoreWebView2Async(Environment);
        }

        private async Task InitializeWebView()
        {
            if (WebView2Browser.CoreWebView2 is null)
            {
                var userDataFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name);
                var options = new CoreWebView2EnvironmentOptions { AllowSingleSignOnUsingOSPrimaryAccount = true };
                Environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);

                WebView2Browser.CreationProperties = new CoreWebView2CreationProperties
                {
                    UserDataFolder = userDataFolder
                };

                await EnsureBrowserIsLoaded();
            }

            Navigate(null);
        }

        private async Task LoadScripts()
        {
            var scriptsBytes = PostEditResources.Scripts;
            var scripts = Encoding.UTF8.GetString(scriptsBytes);

            try
            {
                await EnsureBrowserIsLoaded();
                await WebView2Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(scripts);
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