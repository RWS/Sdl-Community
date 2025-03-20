using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model
{
    public class SegmentComments
    {
        public List<CommentInfo> Comments { get; set; }
        public string FileId { get; set; }
        public string ProjectId { get; set; }
        public string SegmentId { get; set; }
    }
}