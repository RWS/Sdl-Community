using System.Collections.Generic;

namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{
    public class ParagraphUnit
    {
        public string ParagraphUnitId { get; set; }
        public List<SegmentPair> SegmentPairs { get; set; }
        public string FileName { get; set; }

        public ParagraphUnit(string paragraphUnitId, List<SegmentPair> segmentPairs, string fileName)
        {
            ParagraphUnitId = paragraphUnitId;
            SegmentPairs = segmentPairs;
            FileName = fileName;
        }
        public ParagraphUnit()
        {
            ParagraphUnitId = string.Empty;
            SegmentPairs = new List<SegmentPair>();
            FileName = string.Empty;
        }
    }
}
