using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Utilities
{
    public class SegmentMatcher
    {
        public static List<ReportSegment> GetAllMatchingSegments(List<ReportSegment> segments, SegmentFilter filter)
        {
            if (segments == null)
                throw new ArgumentNullException(nameof(segments));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return segments.Where(filter.IsMatch).ToList();
        }
    }
}