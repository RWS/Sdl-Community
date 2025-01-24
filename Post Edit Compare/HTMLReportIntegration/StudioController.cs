using Newtonsoft.Json.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration
{
    public class StudioController
    {
        public const string FileId = "fileId";
        public const string ProjectId = "projectId";
        public const string SegmentId = "segmentId";
        public const string Status = "status";

        private Studio Studio { get; } = new();

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
    }
}