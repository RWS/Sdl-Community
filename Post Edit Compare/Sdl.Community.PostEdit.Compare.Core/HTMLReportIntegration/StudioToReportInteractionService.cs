using Sdl.Community.PostEdit.Compare.Core.HTMLReportIntegration.Components;

namespace Sdl.Community.PostEdit.Compare.Core.HTMLReportIntegration
{
    public class StudioToReportInteractionService
    {
        private void AddCommentToReport(string segmentId, string comment)
        {
            ReportEditor.AddComment(segmentId, comment);
        }
    }
}