namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    public class SegmentSection
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
    }
}
