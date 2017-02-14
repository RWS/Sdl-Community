using System;
using System.Collections.Generic;

namespace Sdl.Community.Parser
{
    [Serializable]
    public class ParagraphUnit
    {
        public string ParagraphUnitId { get; set; }
        public List<SegmentPair> SegmentPairs { get; set; }

        public ParagraphUnit(string paragraphUnitId, List<SegmentPair> segmentPairs)
        {
            ParagraphUnitId = paragraphUnitId;
            SegmentPairs = segmentPairs;
        }
        public ParagraphUnit()
        {
            ParagraphUnitId = "";
            SegmentPairs = new List<SegmentPair>();
        }
    }
}
