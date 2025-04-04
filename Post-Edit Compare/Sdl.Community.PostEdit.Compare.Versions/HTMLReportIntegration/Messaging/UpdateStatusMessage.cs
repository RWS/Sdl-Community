using Newtonsoft.Json.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging
{
    public class UpdateStatusMessage(JObject message) : ISyncMessage
    {
        public string FileId { get; set; } = message[MessagingConstants.FileId]?.ToString();
        public string NewStatus { get; set; } = message[MessagingConstants.Status]?.ToString();
        public string ProjectId { get; set; } = message[MessagingConstants.ProjectId]?.ToString();
        public string SegmentId { get; set; } = message[MessagingConstants.SegmentId]?.ToString();
    }
}