using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration
{
    public class Integration
    {
        private static ReportViewController _reportViewController;
        public static bool SyncOn { get; set; }
        private static EditorEventListener EditorEventListener { get; } = new();
        private static ReportManager ReportManager { get; } = new();

        private static ReportViewController ReportViewController => _reportViewController ??=
            SdlTradosStudio.Application.GetController<ReportViewController>();
        private static StudioController StudioController { get; } = new();

        public static async Task ExportReport()
        {
            var report = await ReportViewController.GetNonInteractiveReport();
            ReportManager.ExportReport(report);
        }

        public static void FilterSegments(SegmentFilter segmentFilter) =>
            ReportViewController.ToggleFilter(segmentFilter);

        public static void HandleReportRequest(string jsonMessage)
        {
            var messageObject = JObject.Parse(jsonMessage);
            var action = messageObject["action"]?.ToString();

            if (!SyncOn && action != "navigate") return;
            StudioController.HandleReportRequest(messageObject);
        }

        public static void InitializeReportFilter(string projectId)
        {
            var reportFilter = SdlTradosStudio.Application.GetController<ReportViewFilterController>();
            reportFilter.Activate();

            var ranges = FuzzyRange.GetFuzzyRangesFromProjectId(projectId);
            ranges.Remove("All");
            reportFilter.InitializeReportFilter(ranges);
        }

        public static void OpenReportBackupFolder()
        {
            var selectedReport = ReportViewController.GetSelectedReport();
            ReportManager.OpenReportBackupFolder(selectedReport);
        }

        public static void OpenReportFolder() => ReportManager.OpenReportFolder();

        public static void RefreshReportList() => ReportViewController.RefreshReportList();

        public static async Task SaveReport()
        {
            var reportFromMemory = await ReportViewController.GetLoadedReport();
            var selectedReport = ReportViewController.GetSelectedReport();

            ReportManager.SaveReport(reportFromMemory, selectedReport.ReportPath);
        }

        public static void ShowLatestReport()
        {
            var dialogResult = MessageBox.Show(
                "Would you like to navigate to the newly created report? This will stop the current project-report synchronization.",
                "Report Created",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (dialogResult == DialogResult.No) return;

            ReportViewController.Activate();

            ToggleReportProjectSync(false);

            ReportViewController.RefreshReportList();
            ReportViewController.SelectLatestReport();
        }

        public static void ShowReportsView() => ReportViewController.Activate();

        public static void ToggleReportProjectSync(bool syncEnabled)
        {
            SyncOn = syncEnabled;
            ReportViewController.ToggleReportExplorer();

            if (syncEnabled)
            {
                ReportManager.BackUpReport(ReportViewController.GetSelectedReport());
                ConnectEditorListener();
            }
            else
                DisconnectEditorListener();
        }

        private static void ConnectEditorListener()
        {
            EditorEventListener.StartListening();
            EditorEventListener.CommentsChanged += EditorEventListener_CommentsChanged;
            EditorEventListener.StatusChanged += EditorEventListener_StatusChanged;
        }

        private static void DisconnectEditorListener()
        {
            EditorEventListener.StopListening();
            EditorEventListener.CommentsChanged -= EditorEventListener_CommentsChanged;
            EditorEventListener.StatusChanged -= EditorEventListener_StatusChanged;
        }

        private static void EditorEventListener_CommentsChanged(List<CommentInfo> comments, string segmentId,
            string fileId) => ReportViewController.UpdateComments(comments, segmentId, fileId);

        private static void EditorEventListener_StatusChanged(string newStatus, string segmentId, string fileId) =>
            ReportViewController.UpdateStatus(newStatus, segmentId, fileId);
    }
}