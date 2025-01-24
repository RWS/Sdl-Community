using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Versions.ReportViewer.Utilities
{
    public class CommentComparer : IEqualityComparer<IComment>
    {
        public bool Equals(IComment x, IComment y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.Text == y.Text && x.Author == y.Author && x.Date == y.Date;
        }

        public int GetHashCode(IComment obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            unchecked
            {
                var hash = 17;
                hash = hash * 23 + (obj.Text?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.Author?.GetHashCode() ?? 0);
                hash = hash * 23 + obj.Date.GetHashCode();
                return hash;
            }
        }
    }
}