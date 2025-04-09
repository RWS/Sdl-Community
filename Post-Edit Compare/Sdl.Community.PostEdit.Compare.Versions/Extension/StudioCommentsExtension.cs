using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.PostEdit.Versions.Extension
{
    public static class StudioCommentsExtension
    {
        public static List<CommentInfo> ToCommentInfoList(this IEnumerable<IComment> comments)
        {
            return comments?.Select(c => new CommentInfo
            {
                Author = c.Author,
                Date = c.Date.ToString(MessagingConstants.DateFormat),
                Severity = c.Severity.ToString(),
                Text = c.Text
            }).ToList();
        }
    }
}