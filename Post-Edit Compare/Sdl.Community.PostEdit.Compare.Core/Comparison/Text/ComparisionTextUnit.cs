using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Compare.Core.Comparison.Text
{
    public partial class TextComparer
    {
        public enum ComparisonTextUnitType
        {
            Identical = 0,
            New,
            Removed
        }
        public class ComparisonTextUnit
        {
            public string Text { get; set; }
            public List<SDLXLIFF.SegmentSection> TextSections { get; set; }
            public ComparisonTextUnitType ComparisonTextUnitType { get; set; }

            public ComparisonTextUnit()
            {
                Text = "";
                ComparisonTextUnitType = ComparisonTextUnitType.Identical;
                TextSections = null;
            }

            public ComparisonTextUnit(string text, ComparisonTextUnitType comparisonTextUnitType)
            {
                Text = text;
                ComparisonTextUnitType = comparisonTextUnitType;
                TextSections = null;
            }

        }
    }
}
