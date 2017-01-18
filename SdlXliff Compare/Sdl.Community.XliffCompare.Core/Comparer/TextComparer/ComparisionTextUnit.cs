using System.Collections.Generic;

namespace Sdl.Community.XliffCompare.Core.Comparer.TextComparer
{
    internal partial class TextComparer
    {
        internal enum ComparisonTextUnitType
        {
            Identical = 0,
            New,
            Removed
        }
        internal class ComparisonTextUnit
        {
            internal string Text { get; set; }
            internal List<SDLXLIFF.SegmentSection> TextSections { get; set; }
            internal ComparisonTextUnitType ComparisonTextUnitType { get; set; }

            internal ComparisonTextUnit()
            {
                Text = "";
                ComparisonTextUnitType = ComparisonTextUnitType.Identical;
                TextSections = null;
            }

            internal ComparisonTextUnit(string text, ComparisonTextUnitType comparisonTextUnitType)
            {
                Text = text;
                ComparisonTextUnitType = comparisonTextUnitType;
                TextSections = null;
            }

        }
    }
}
