using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Compare.Core.Comparison
{
    public partial class Comparer
    {

        public class ComparisonParagraphUnit
        {
            public string ParagraphId { get; set; }
            public List<ComparisonSegmentUnit> ComparisonSegmentUnits { get; set; }
            public bool ParagraphIsUpdated { get; set; }
            public bool ParagraphStatusChanged { get; set; }
            public bool ParagraphHasComments { get; set; }


            public ComparisonParagraphUnit()
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
