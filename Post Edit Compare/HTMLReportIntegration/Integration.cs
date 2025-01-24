using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components;
using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration
{
    public class Integration
    {
        private static EditorEventListener EditorEventListener { get; } = new();
        private static ReportViewController ReportViewController => ReportViewController.Instance;
        private static StudioController StudioController { get; } = new();

        public static void HandleReportRequest(string jsonMessage)
        {
            StudioController.HandleReportRequest(jsonMessage);
        }

        public static void ToggleSync(bool syncEnabled)
        {
            if (syncEnabled)
            {
                EditorEventListener.StartListening();
                EditorEventListener.CommentsChanged += EditorEventListener_CommentsChanged;
            }
            else
            {
                EditorEventListener.StopListening();
                EditorEventListener.CommentsChanged -= EditorEventListener_CommentsChanged;
            }
        }

        private static void EditorEventListener_CommentsChanged(List<CommentInfo> comments, string segmentId)
        {
            ReportViewController.UpdateComments(comments, segmentId);
            
        }
    }
}