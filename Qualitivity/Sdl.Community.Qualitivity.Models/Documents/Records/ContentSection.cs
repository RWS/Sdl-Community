using System;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{

    [Serializable]
    public class ContentSection : ICloneable
    {
        public enum LanguageType
        {
            Source,
            Target,
            TargetUpdated
        }
        public enum ContentType
        {
            Text,
            End,
            LockedContent,
            Standalone,
            Start,
            TextPlaceholder,
            Undefined,
            UnmatchedEnd,
            UnmatchedStart
        }

        public string Id { get; set; }
        [XmlIgnore]
        public int RecordId { get; set; }
        [XmlIgnore]
        public int DocumentActivityId { get; set; }
        public ContentType CntType { get; set; }
        public LanguageType LangType { get; set; }
        public string IdRef { get; set; }       
        public string Content { get; set; }
        public bool HasRevision { get; set; }
        public RevisionMarker RevisionMarker { get; set; }

        public ContentSection()
        {
            Id = Guid.NewGuid().ToString();
            RecordId = -1;
            DocumentActivityId = -1;
            CntType = ContentType.Text;
            LangType = LanguageType.Source;
            IdRef = string.Empty;
            Content = string.Empty;
            HasRevision = false;
            RevisionMarker = null;
        }
        public ContentSection(LanguageType langType, ContentType cntType, string idRef, string content)
        {
            Id = Guid.NewGuid().ToString();
            RecordId = -1;
            DocumentActivityId = -1;
            LangType = langType;
            CntType = cntType;
            IdRef = idRef;
            Content = content;
            HasRevision = false;
            RevisionMarker = null;
        }
        public ContentSection(LanguageType langType, ContentType cntType, string idRef, string content, RevisionMarker revisionMarker)
        {
            Id = Guid.NewGuid().ToString();
            RecordId = -1;
            DocumentActivityId = -1;
            LangType = langType;
            CntType = cntType;
            IdRef = idRef;
            Content = content;
            HasRevision = revisionMarker != null;
            RevisionMarker = revisionMarker;            
        }


        public object Clone()
        {
            var contentSection = new ContentSection
            {
                Id = Id,
                RecordId = RecordId,
                DocumentActivityId = DocumentActivityId,
                LangType = LangType,
                CntType = CntType,
                IdRef = IdRef,
                Content = Content,
                HasRevision = HasRevision,
                RevisionMarker = RevisionMarker != null ? (RevisionMarker) RevisionMarker.Clone() : RevisionMarker
            };




            return contentSection;
        }
    }
}
