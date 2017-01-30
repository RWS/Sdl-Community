namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    internal class SegmentSection
    {
        internal bool IsText { get; set; }
        internal string Content { get; set; }
        internal SegmentSection(bool isText, string content)
        {
            IsText = isText;
            Content = content;
        }
    }
}
