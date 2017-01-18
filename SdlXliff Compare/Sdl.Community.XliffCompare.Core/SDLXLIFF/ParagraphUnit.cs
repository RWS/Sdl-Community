using System.Collections.Generic;

namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    internal class ParagraphUnit
    {
        internal string ParagraphUnitId { get; set; }
        internal List<SegmentPair> SegmentPairs { get; set; }

        internal ParagraphUnit(string paragraphUnitId, List<SegmentPair> segmentPairs)
        {
            ParagraphUnitId = paragraphUnitId;
            SegmentPairs = segmentPairs;
        }
        internal ParagraphUnit()
        {
            ParagraphUnitId = "";
            SegmentPairs = new List<SegmentPair>();
        }
    }
}
