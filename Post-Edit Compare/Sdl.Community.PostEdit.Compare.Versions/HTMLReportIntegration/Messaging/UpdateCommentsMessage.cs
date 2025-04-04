using Newtonsoft.Json.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging
{
    public class UpdateCommentsMessage(JObject message) : ISyncMessage
    {
        public string Comment { get; set; } = message[MessagingConstants.Comment]?.ToString();
        public string FileId { get; set; } = message[MessagingConstants.FileId]?.ToString();
        public string ProjectId { get; set; } = message[MessagingConstants.ProjectId]?.ToString();
        public string SegmentId { get; set; } = message[MessagingConstants.SegmentId]?.ToString();
        public string Severity { get; set; } = message[MessagingConstants.Severity]?.ToString();
    }
}