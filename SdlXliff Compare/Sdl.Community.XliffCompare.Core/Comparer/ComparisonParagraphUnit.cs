using System.Collections.Generic;

namespace Sdl.Community.XliffCompare.Core.Comparer
{
    internal partial class Comparer
    {

        internal class ComparisonParagraphUnit
        {
            internal string ParagraphId { get; set; }
            internal List<ComparisonSegmentUnit> ComparisonSegmentUnits { get; set; }
            internal bool ParagraphIsUpdated { get; set; }
            internal bool ParagraphStatusChanged { get; set; }
            internal bool ParagraphHasComments { get; set; }


            internal ComparisonParagraphUnit()
            {
                ParagraphId = "";
                ComparisonSegmentUnits = new List<ComparisonSegmentUnit>();
                ParagraphIsUpdated = false;
                ParagraphStatusChanged = false;
                ParagraphHasComments = false;
            }
        }
    }
}
