namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{
    public class SegmentSection
    {
        public ContentType Type { get; set; }
        public string Content { get; set; }
        public string SectionId { get; set; }

        public SegmentSection(ContentType type, string sectionId, string content)
        {
            Type = type;
            SectionId = sectionId;
            Content = content;
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
