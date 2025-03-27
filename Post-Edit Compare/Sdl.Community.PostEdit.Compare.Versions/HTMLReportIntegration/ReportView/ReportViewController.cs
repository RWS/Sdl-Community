using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Controls.Interface;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.ViewModel;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        public bool IsDocked => ReportViewer.Visibility == Visibility.Visible;
        public ReportViewer ReportViewer { get; set; }
        private IReportViewer CurrentReportViewer => IsDocked ? ReportViewer : ReportViewerWindow;
        private ReportExplorer ReportExplorer { get; set; }
        private ReportExplorerViewModel ReportExplorerViewModel { get; set; }
        private ReportViewerWindow ReportViewerWindow { get; set; }

        private ReportViewFilterController ReportViewFilterController =>
                            SdlTradosStudio.Application.GetController<ReportViewFilterController>();

        public async Task<List<SegmentComments>> GetAllComments() => await CurrentReportViewer.GetAllComments();

        public async Task<List<ReportSegment>> GetAllSegmentsCurrentlyVisible() => await ReportViewer.GetAllSegmentsCurrentlyVisible();

        public async Task<List<StatusInfo>> GetAllStatuses()
        {
            var projectId = await GetProjectId();
            var segments = await GetAllSegments();

            return segments.Select(seg => new StatusInfo
            {
                FileId = seg.FileId,
                ProjectId = projectId,
                SegmentId = seg.SegmentId,
                Status = seg.Status
            }).ToList();
        }

        public async Task<string> GetLoadedReport() => await CurrentReportViewer.GetLoadedReport();

        public async Task<string> GetNonInteractiveReport() => await CurrentReportViewer.GetNonInteractiveReport();

        public ReportInfo GetSelectedReport() => ReportExplorer.SelectedReport;

        public async Task HandleReportRequestWithoutSync(JObject messageObject)
        {
            var syncMessage = SyncMessage.Create(messageObject);
            switch (syncMessage)
            {
                case UpdateStatusMessage updateStatusMessage:
                    await UpdateStatus(updateStatusMessage.NewStatus, updateStatusMessage.SegmentId,
                        updateStatusMessage.FileId);
                    break;

                case UpdateCommentsMessage updateCommentsMessage:
                    await UpdateCommentWoSync(updateCommentsMessage.Comment, updateCommentsMessage.Severity,
                        updateCommentsMessage.SegmentId, updateCommentsMessage.FileId);
                    break;
            }
        }

        public void HideLoadingScreen() => ReportExplorer.HideLoadingScreen();

        public void RefreshLists(List<ProjectInfo> projects, List<ReportInfo> reports)
        {
            RefreshProjectList(projects);
            RefreshReportsList(reports);
        }

        public void RefreshReportsList(List<ReportInfo> reports)
        {
            if (reports is null) return;
            ReportExplorerViewModel.SetReportsList(reports);
        }

        public async Task SelectLatestReport(ReportInfo report) => await ReportExplorerViewModel.SetSelectedReport(report);

        public void ShowLoadingScreen() => ReportExplorer.ShowLoadingScreen();

        public async Task ToggleFilter(SegmentFilter segmentFilter)
        {
            var segments = await GetAllSegments();
            if (!segmentFilter.IsEmpty)
            {
                ReportExplorer.IsEnabled = false;

                var matchingSegments = SegmentMatcher.GetAllMatchingSegments(segments, segmentFilter);
                ReportViewFilterController.SetFilteringResultCount(matchingSegments.Count, segments.Count);
                await CurrentReportViewer.ShowSegments(matchingSegments.Select(seg => (seg.SegmentId, seg.FileId)).ToList());
            }
            else
            {
                if (!Integration.IsSyncOn) ReportExplorer.IsEnabled = true;
                ReportViewFilterController.SetFilteringResultCount(segments.Count, segments.Count);
                await CurrentReportViewer.ShowAllSegments();
            }
        }

        public void ToggleReportExplorer(bool enabled) => ReportExplorer.ToggleOnOff(enabled);

        public async Task UndockReportViewer()
        {
            ReportExplorer.IsEnabled = false;
            CurrentReportViewer.Visibility = Visibility.Collapsed;

            ReportViewerWindow.Show();

            var selectedReport = GetSelectedReport();
            await ReportViewerWindow.Navigate(selectedReport?.ReportPath);
        }

        public async Task UpdateComments(List<CommentInfo> comments, string segmentId, string fileId) =>
                    await CurrentReportViewer.UpdateComments(comments, segmentId, fileId);

        public async Task UpdateStatus(string newStatus, string segmentId, string fileId)
        {
            newStatus = EnumHelper.GetFriendlyStatusString(newStatus);
            if (newStatus == "Unknown") return;
            await CurrentReportViewer.UpdateStatus(newStatus, segmentId, fileId);
        }

        protected override IUIControl GetContentControl() => CurrentReportViewer;

        protected override IUIControl GetExplorerBarControl() => ReportExplorer;

        protected override void Initialize(IViewContext context)
        {
            InitializeControls();
            AttachEvents();
            Integration.Initialize();
        }

        private void AttachEvents()
        {
            ReportExplorer.SelectedReportChanged += Explorer_SelectedReportChanged;
            ReportViewer.WebMessageReceived += WebView2Browser_WebMessageReceived;
            ReportViewerWindow.WebMessageReceived += WebView2Browser_WebMessageReceived;
            ReportViewerWindow.Hidden += ReportViewerWindow_Closed;
        }

        private async void Explorer_SelectedReportChanged()
        {
            try
            {
                var selectedReport = GetSelectedReport();

                if (selectedReport is not null)
                {
                    Integration.ToggleSyncRibbon(true);
                    ReportViewFilterController.Activate();
                    ReportViewFilterController.ReportViewFilter.IsEnabled = true;
                }
                else
                {
                    Integration.ToggleSyncRibbon(false);
                    ReportViewFilterController.ReportViewFilter.IsEnabled = false;
                }

                await CurrentReportViewer.Navigate(ReportExplorer.SelectedReport?.ReportPath);
                await Task.Delay(500);

                var projectId = await GetProjectId();
                Integration.InitializeReportFilter(projectId);
            }
            catch (Exception e)
            {
                ReportExplorer.SelectedReport = null;
                ErrorHandler.ShowError($"Error loading the selected report: {e.Message}");
            }
        }

        private async Task<List<ReportSegment>> GetAllSegments() => await CurrentReportViewer.GetAllSegments();

        private async Task<string> GetProjectId() => await CurrentReportViewer.GetProjectId();

        private void InitializeControls()
        {
            ReportViewer = new ReportViewer();
            ReportViewerWindow = new ReportViewerWindow();

            ReportExplorerViewModel = new ReportExplorerViewModel();
            ReportExplorer = new ReportExplorer { DataContext = ReportExplorerViewModel };
        }

        private void RefreshProjectList(List<ProjectInfo> projects)
        {
            if (projects is null) return;
            ReportExplorerViewModel.SetProjectsList(projects);
        }

        private async void ReportViewerWindow_Closed()
        {
            ReportViewer.Visibility = Visibility.Visible;
            await CurrentReportViewer.Navigate(ReportExplorer.SelectedReport?.ReportPath);
            ReportExplorer.IsEnabled = true;
            SdlTradosStudio.Application.GetAction<UndockReportViewer>().Enabled = true;
        }

        private async Task UpdateCommentWoSync(string comment, string severity, string segmentId, string fileId)
        {
            await CurrentReportViewer.UpdateComments([new CommentInfo
                {
                    Author = Environment.UserName,
                    Date = DateTime.Now.ToString(MessagingConstants.DateFormat),
                    Text = comment,
                    Severity = severity
                }], segmentId, fileId, AddReplace.Add);
        }

        private void WebView2Browser_WebMessageReceived(object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e) =>
            Integration.HandleReportRequest(e.WebMessageAsJson);
    }
}