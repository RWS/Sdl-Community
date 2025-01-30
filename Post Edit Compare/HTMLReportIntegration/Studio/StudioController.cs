using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio.Components;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Studio
{
    public class StudioController
    {
        public const string FileId = "fileId";
        public const string ProjectId = "projectId";
        public const string SegmentId = "segmentId";
        public const string Status = "status";
        public const string Comment = "comment";
        public const string Severity = "severity";

        private StudioActionExecutor Studio { get; } = new();

        public void HandleReportRequest(JObject messageObject)
        {
            switch (messageObject["action"].ToString())
            {
                case "addComment":
                    AddComment(messageObject);
                    break;

                case "navigate":
                    NavigateToSegment(messageObject);
                    break;

                case "updateStatus":
                    UpdateStatus(messageObject);
                    break;
            }
        }

        public void NavigateToSegment(JObject jsonMessage)
        {
            var segmentId = jsonMessage[SegmentId]?.ToString();
            var fileId = jsonMessage[FileId]?.ToString();
            var projectId = jsonMessage[ProjectId]?.ToString();

            Studio.NavigateToSegment(segmentId, fileId, projectId);
        }

        public void UpdateStatus(JObject jsonMessage)
        {
            var status = jsonMessage[Status]?.ToString();
            var segmentId = jsonMessage[SegmentId]?.ToString();
            var fileId = jsonMessage[FileId]?.ToString();
            var projectId = jsonMessage[ProjectId]?.ToString();

            Studio.ChangeStatusOfSegment(status,
                segmentId,
                fileId,
                projectId);
        }

        private void AddComment(JObject jsonMessage)
        {
            var segmentId = jsonMessage[SegmentId]?.ToString();
            var fileId = jsonMessage[FileId]?.ToString();
            var projectId = jsonMessage[ProjectId]?.ToString();
            var comment = jsonMessage[Comment]?.ToString();
            var severity = jsonMessage[Severity]?.ToString();

            Studio.AddComment(comment, severity, segmentId, fileId, projectId);
        }
    }
}