using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration
{
    public class Integration
    {
        private static EditorEventListener EditorEventListener { get; } = new();
        private static ReportManager ReportManager { get; set; } = new();
        private static ReportViewController ReportViewController => ReportViewController.Instance;
        private static StudioController StudioController { get; } = new();
        private static bool SyncOn { get; set; }

        public static async Task ExportReport()
        {
            var report = await ReportViewController.GetLoadedReport();
            ReportManager.ExportReport(report);
        }

        public static void HandleReportRequest(string jsonMessage)
        {
            var messageObject = JObject.Parse(jsonMessage);
            var action = messageObject["action"]?.ToString();

            if (!SyncOn && action != "navigate") return;
            StudioController.HandleReportRequest(messageObject);
        }

        public static async Task SaveReport()
        {
            var reportFromMemory = await ReportViewController.GetLoadedReport();
            var selectedReport = ReportViewController.GetSelectedReport();

            ReportManager.SaveReport(reportFromMemory, selectedReport.ReportPath);
        }

        public static void ToggleSync(bool syncEnabled)
        {
            SyncOn = syncEnabled;
            ReportViewController.ToggleReportSelection();

            if (syncEnabled)
            {
                EditorEventListener.StartListening();
                EditorEventListener.CommentsChanged += EditorEventListener_CommentsChanged;
                EditorEventListener.StatusChanged += EditorEventListener_StatusChanged;
            }
            else
            {
                EditorEventListener.StopListening();
                EditorEventListener.CommentsChanged -= EditorEventListener_CommentsChanged;
                EditorEventListener.StatusChanged -= EditorEventListener_StatusChanged;
            }
        }

        private static void EditorEventListener_CommentsChanged(List<CommentInfo> comments, string segmentId, string fileId)
        {
            ReportViewController.UpdateComments(comments, segmentId, fileId);
        }

        private static void EditorEventListener_StatusChanged(string newStatus, string segmentId, string fileId)
        {
            ReportViewController.UpdateStatus(newStatus, segmentId, fileId);
        }

        public static void OpenReportFolder()
        {
            ReportManager.OpenReportFolder();
        }

        public static void RefreshReportList()
        {
            ReportViewController.RefreshReportList();
        }
    }
}