using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio
{
    public class StudioController
    {
        private StudioActionExecutor Studio { get; } = new();

        public void HandleReportRequest(JObject messageObject)
        {
            var message = SyncMessage.Create(messageObject);
            switch (message)
            {
                case NavigateMessage navigateMessage:
                    NavigateToSegment(navigateMessage);
                    break;

                case UpdateStatusMessage updateStatusMessage:
                    UpdateStatus(updateStatusMessage);
                    break;

                case UpdateCommentsMessage updateCommentsMessage:
                    AddComment(updateCommentsMessage);
                    break;
            }
        }

        public void NavigateToSegment(NavigateMessage msg) =>
            Studio.NavigateToSegment(msg.SegmentId, msg.FileId, msg.ProjectId);

        public void UpdateStatus(UpdateStatusMessage msg) =>
            Studio.ChangeStatusOfSegment(msg.NewStatus, msg.SegmentId, msg.FileId, msg.ProjectId);

        private void AddComment(UpdateCommentsMessage msg) =>
            Studio.AddComment(msg.Comment, msg.Severity, msg.SegmentId, msg.FileId, msg.ProjectId);
    }
}