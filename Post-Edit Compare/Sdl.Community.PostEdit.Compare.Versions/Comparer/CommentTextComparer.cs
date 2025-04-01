using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Versions.Comparer
{
    public class CommentTextComparer : IEqualityComparer<CommentInfo>
    {
        public bool Equals(CommentInfo x, CommentInfo y)
        {
            if (x is null || y is null) return false;
            return x.Text == y.Text;
        }

        public int GetHashCode(CommentInfo obj)
        {
            return obj.Text.GetHashCode();
        }
    }
}