using Newtonsoft.Json;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView
{
    [View(
        Id = "PostEdit.ReportViewer",
        Name = "Post-Edit Report Viewer",
        Description = "Post-Edit Report Viewer",
        Icon = "PostEditVersions_Icon",
        AllowViewParts = true,
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation)
    )]
    public class ReportViewController : AbstractViewController
    {
        public static ReportViewController Instance { get; set; }
        private ReportExplorer ReportExplorer { get; set; }
        private ReportExplorerViewModel ReportExplorerViewModel { get; set; }
        private ReportViewer ReportViewer { get; set; }

        public async Task<string> GetLoadedReport() => await ReportViewer.GetLoadedReport();

        public async Task<string> GetNonInteractiveReport() => await ReportViewer.GetNonInteractiveReport();

        public ReportInfo GetSelectedReport()
        {
            return ReportExplorerViewModel.SelectedReport;
        }

        public void RefreshReportList() => ReportExplorerViewModel.RefreshReportList();

        public async Task ToggleFilter(SegmentFilter segmentFilter)
        {
            if (!segmentFilter.IsEmpty)
            {
                ReportExplorer.IsEnabled = false;

                var segments = await ReportViewer.GetAllSegments();
                var matchingSegments = SegmentMatcher.GetAllMatchingSegments(segments, segmentFilter);
                await ReportViewer.ShowSegments(matchingSegments.Select(seg => (seg.SegmentId, seg.FileId)).ToList());
            }
            else
            {
                ReportExplorer.IsEnabled = true;
                await ReportViewer.ShowAllSegments();
            }
        }

        public void ToggleReportExplorer() => ReportExplorer.ToggleOnOff();

        public void UpdateComments(List<CommentInfo> comments, string segmentId, string fileId)
        {
            var commentsJson = JsonConvert.SerializeObject(comments, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            var script = $"replaceCommentsForSegment('{segmentId}', {commentsJson}, '{fileId}');";

            TryExecuteScript(script);
        }

        public void UpdateStatus(string newStatus, string segmentId, string fileId)
        {
            TryExecuteFunction("updateSegmentStatus", segmentId, fileId, newStatus);
        }

        protected override IUIControl GetContentControl() => ReportViewer;

        protected override IUIControl GetExplorerBarControl() => ReportExplorer;

        protected override void Initialize(IViewContext context)
        {
            InitializeControls();
            AttachEvents();
            Instance = this;
        }

        private void AttachEvents()
        {
            ReportExplorer.SelectedReportChanged += ExplorerOnSelectedReportChanged;
            ReportViewer.WebMessageReceived += WebView2Browser_WebMessageReceived;
        }

        private async void ExplorerOnSelectedReportChanged()
        {
            await ReportViewer.Navigate(ReportExplorerViewModel.SelectedReport?.ReportPath);
            await Task.Delay(500);

            var projectId = await ReportViewer.GetProjectId();
            if (projectId == null) return;

            Integration.InitializeReportFilter(projectId);
        }

        private void InitializeControls()
        {
            ReportViewer = new ReportViewer();

            ReportExplorerViewModel = new ReportExplorerViewModel();
            ReportExplorer = new ReportExplorer
            {
                DataContext = ReportExplorerViewModel
            };
        }

        private void TryExecuteFunction(string functionName, params object[] parameters)
        {
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
            var script = $"{functionName}({paramsJoined});";

            TryExecuteScript(script);
        }

        private void TryExecuteScript(string script)
        {
            try
            {
                ReportViewer.WebView2Browser.Dispatcher.Invoke(async () =>
                {
                    ReportViewer.WebView2Browser.ExecuteScriptAsync(script);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void WebView2Browser_WebMessageReceived(object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e) =>
            Integration.HandleReportRequest(e.WebMessageAsJson);
    }
}