using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    public class SegmentPair
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public List<SegmentSection> SourceSections { get; set; }
        public List<SegmentSection> TargetSections { get; set; }

        public string Id { get; set; }
        public string SegmentStatus { get; set; }
        public bool IsLocked { get; set; }
        public TranslationOrigin TranslationOrigin { get; set; }
        public List<Comment> Comments { get; set; }


        public int SourceWords { get; set; }
        public int SourceChars { get; set; }
        public int SourceTags { get; set; }
        public int SourcePlaceables { get; set; }
    

        public SegmentPair()
        {
            SourceSections = new List<SegmentSection>();
            TargetSections = new List<SegmentSection>();

            Source = string.Empty;
            Target = string.Empty;
            Id = string.Empty;
            SegmentStatus = string.Empty;
            IsLocked = false;
            TranslationOrigin = new TranslationOrigin();
            Comments = new List<Comment>();


            SourceWords = 0;
            SourceChars = 0;
            SourceTags = 0;
            SourcePlaceables = 0;
        }
    }
}
