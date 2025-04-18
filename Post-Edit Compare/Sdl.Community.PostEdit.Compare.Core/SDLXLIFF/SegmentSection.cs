using System;

namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    public class SegmentSection : IEquatable<SegmentSection>
    {
        public ContentType Type { get; set; }
        public string Content { get; set; }
        public string SectionId { get; set; }
        public RevisionMarker RevisionMarker { get; set; }

        public SegmentSection(ContentType type, string sectionId, string content)
        {
            Type = type;
            SectionId = sectionId;
            Content = content;
            RevisionMarker = null;
        }

        public SegmentSection(ContentType type, string sectionId, string content, RevisionMarker revisionMarker)
        {
            Type = type;
            SectionId = sectionId;
            Content = content;
            RevisionMarker = revisionMarker;
        }

        public enum ContentType
        {
            Text = 0,
            Tag,
            TagClosing,
            Placeholder,
            LockedContent
        }

        public bool Equals(SegmentSection other)
        {
            if (other == null)
                return false;

            return Type == other.Type &&
                   Content == other.Content &&
                   SectionId == other.SectionId &&
                   ((RevisionMarker == null && other.RevisionMarker == null) ||
                    (RevisionMarker != null && other.RevisionMarker != null &&
                     RevisionMarker.Author == other.RevisionMarker.Author &&
                     RevisionMarker.Date == other.RevisionMarker.Date &&
                     RevisionMarker.Type == other.RevisionMarker.Type));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SegmentSection);
        }

        public override int GetHashCode()
        {
            int hash = Type.GetHashCode();
            hash = (hash * 397) ^ (Content?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (SectionId?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (RevisionMarker?.Author?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (RevisionMarker?.Date.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (RevisionMarker?.Type.GetHashCode() ?? 0);
            return hash;
        }
    }
}
