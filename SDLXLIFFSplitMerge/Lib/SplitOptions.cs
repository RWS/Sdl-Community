using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    public class SplitOptions
    {
        public enum SplitType { WordsCount, EqualParts, SegmentNumbers };

        private List<string> segmentIDs;

        public List<SegStatus> SplitNonCountStatus
        { get; set; }

        public SplitType Criterion
        { get; set; }
        public int WordsCount
        { get; set; }
        public int PartsCount
        { get; set; }
        public List<string> SegmentIDs
        { get { return segmentIDs; } }

        public bool IsPercent
        { get; set; }
        public int PercMax
        { get; set; }

        public SplitOptions()
        {
            Criterion = SplitType.WordsCount;
            WordsCount = 1000;
            PartsCount = 0;
            segmentIDs = new List<string>();

            PercMax = 0;
        }

        public SplitOptions(SplitType criterion)
        {
            Criterion = criterion;
            segmentIDs = new List<string>();
            PercMax = 90;
            switch (Criterion)
            {
                case SplitType.WordsCount:
                    WordsCount = 1000;
                    PartsCount = 0;
                    break;
                case SplitType.EqualParts:
                    WordsCount = 0;
                    PartsCount = 2;
                    break;
                case SplitType.SegmentNumbers:
                    WordsCount = 0;
                    PartsCount = 0;
                    break;
            }
        }

        public void setSegmentIDs(string ids)
        {
            segmentIDs.Clear();

            string idNum = "";
            string[] sIds = ids.Split(',');
            foreach (string id in sIds)
            {
                idNum = id.Trim();
                if (idNum.Length > 0)
                    if (!segmentIDs.Contains(idNum))
                        segmentIDs.Add(idNum);
            }
        }
    }
}
