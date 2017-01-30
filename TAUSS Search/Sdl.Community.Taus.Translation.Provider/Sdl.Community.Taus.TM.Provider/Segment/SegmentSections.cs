namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment
{
    
    public class SegmentSection
    {

        public bool IsText { get; set; }
        public string Content { get; set; }
        public SegmentSection(bool isText, string content)
        {
            IsText = isText;
            Content = content;
        }


    }
}
