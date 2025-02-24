using Sdl.Community.PostEdit.Compare.Helpers;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

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
        private ReportExplorer ReportExplorer { get; set; }
        private ReportExplorerViewModel ReportExplorerViewModel { get; set; }
        private ReportViewer ReportViewer { get; set; }

        public async Task<string> GetLoadedReport() => await ReportViewer.GetLoadedReport();

        public async Task<string> GetNonInteractiveReport() => await ReportViewer.GetNonInteractiveReport();

        public ReportInfo GetSelectedReport() => ReportExplorerViewModel.SelectedReport;

        public void RefreshReportsList(List<ProjectInfo> projects, List<ReportInfo> reports)
        {
            ReportExplorerViewModel.SetProjectsList(projects);
            ReportExplorerViewModel.SetReportsList(reports);
        }

        public void SelectLatestReport() => ReportExplorer.SelectLatestReport();

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
                if (!Integration.SyncOn) ReportExplorer.IsEnabled = true;
                await ReportViewer.ShowAllSegments();
            }
        }

        public void ToggleReportExplorer() => ReportExplorer.ToggleOnOff();

        public void UpdateComments(List<CommentInfo> comments, string segmentId, string fileId) =>
            ReportViewer.UpdateComments(comments, segmentId, fileId);

        public void UpdateStatus(string newStatus, string segmentId, string fileId) =>
            ReportViewer.UpdateStatus(newStatus, segmentId, fileId);

        protected override IUIControl GetContentControl() => ReportViewer;

        protected override IUIControl GetExplorerBarControl() => ReportExplorer;

        protected override void Initialize(IViewContext context)
        {
            InitializeControls();
            AttachEvents();
        }

        private void AttachEvents()
        {
            ReportExplorer.SelectedReportChanged += Explorer_SelectedReportChanged;
            ReportViewer.WebMessageReceived += WebView2Browser_WebMessageReceived;
        }

        private async void Explorer_SelectedReportChanged()
        {
            try
            {
                var selectedReport = GetSelectedReport();
                Integration.ToggleSyncRibbon(selectedReport is not null);

                await ReportViewer.Navigate(ReportExplorerViewModel.SelectedReport?.ReportPath);
                await Task.Delay(500);

                var projectId = await ReportViewer.GetProjectId();
                Integration.InitializeReportFilter(projectId);
            }
            catch (Exception e)
            {
                ReportExplorerViewModel.SelectedReport = null;
                ErrorHandler.ShowError($"Error loading the selected report: {e.Message}");
            }
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

        private void WebView2Browser_WebMessageReceived(object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e) =>
            Integration.HandleReportRequest(e.WebMessageAsJson);
    }
}