using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{

    [Serializable]
    public class ContentSections : ICloneable
    {
        
        public List<ContentSection> SourceSections { get; set; }        
        public List<ContentSection> TargetOriginalSections { get; set; }       
        public List<ContentSection> TargetUpdatedSections { get; set; }

        [XmlIgnore]
        public string SourceIndex { get; set; }
        [XmlIgnore]
        public string TargetOriginalIndex { get; set; }
        [XmlIgnore]
        public string TargetUpdatedIndex { get; set; }
        [XmlIgnore]
        public string TargetCompareIndex { get; set; }

        [XmlIgnore]
        public Span SourceSectionLines { get; set; }
        [XmlIgnore]
        public Span TargetOriginalSectionLines { get; set; }
        [XmlIgnore]
        public Span TargetUpdatedSectionLines { get; set; }
        [XmlIgnore]
        public Span TargetCompareSectionLines { get; set; }
        [XmlIgnore]
        public Span TargetTrackChangesLines { get; set; }

        public ContentSections()
        {
            SourceSections = new List<ContentSection>();
            TargetOriginalSections = new List<ContentSection>();
            TargetUpdatedSections = new List<ContentSection>();

            SourceIndex = string.Empty;
            TargetUpdatedIndex = string.Empty;
            TargetOriginalIndex = string.Empty;
            TargetCompareIndex = string.Empty;

            SourceSectionLines = null;
            TargetOriginalSectionLines = null;
            TargetUpdatedSectionLines = null;
            TargetCompareSectionLines = null;
            TargetTrackChangesLines = null;

        }

        public object Clone()
        {
            var contentSections = new ContentSections {SourceSections = new List<ContentSection>()};

            foreach (var tcrs in SourceSections)
                contentSections.SourceSections.Add((ContentSection)tcrs.Clone());

            contentSections.TargetOriginalSections = new List<ContentSection>();
            foreach (var tcrs in TargetOriginalSections)
                contentSections.TargetOriginalSections.Add((ContentSection)tcrs.Clone());

            contentSections.TargetUpdatedSections = new List<ContentSection>();
            foreach (var tcrs in TargetUpdatedSections)
                contentSections.TargetUpdatedSections.Add((ContentSection)tcrs.Clone());

            //set to empty or null for now
            SourceIndex = string.Empty;
            TargetUpdatedIndex = string.Empty;
            TargetOriginalIndex = string.Empty;
            TargetCompareIndex = string.Empty;

            SourceSectionLines = null;
            TargetOriginalSectionLines = null;
            TargetUpdatedSectionLines = null;
            TargetCompareSectionLines = null;
            TargetTrackChangesLines = null;

            return contentSections;

        }
    }
}
