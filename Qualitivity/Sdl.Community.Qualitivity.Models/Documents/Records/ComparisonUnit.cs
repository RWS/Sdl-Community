using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.Documents.Records
{
   
    [Serializable]
    public class ComparisonUnit: ICloneable
    {
        public enum ComparisonType
        {
            Identical = 0,
            New,
            Removed,
            None
        }
      
        public string Text { get; set; }
        public ComparisonType Type { get; set; }
        public List<ContentSection> Section { get; set; }

        public ComparisonUnit()
        {
            Text = string.Empty;
            Section = new List<ContentSection>();
            Type = ComparisonType.Identical;
        }
        public ComparisonUnit(string text, ComparisonType tp)
        {
            Text = text;
            Type = tp;
            Section = new List<ContentSection>();
        }
        public object Clone()
        {
            var comparisonUnit = new ComparisonUnit
            {
                Text = Text,
                Section = new List<ContentSection>()
            };

            foreach (var section in Section)
                comparisonUnit.Section.Add((ContentSection)section.Clone());

            comparisonUnit.Type = Type;

            return comparisonUnit;
        }

    }
}
