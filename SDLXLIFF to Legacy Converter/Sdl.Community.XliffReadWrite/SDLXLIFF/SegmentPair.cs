using System.Collections.Generic;

namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{
    public class SegmentPair
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public List<SegmentSection> SourceSections { get; set; }
        public List<SegmentSection> TargetSections { get; set; }

        public string ParagraphId { get; set; }
        public string Id { get; set; }
        public string SegmentStatus { get; set; }
        public bool IsLocked { get; set; }
        
        public TranslationOrigin TranslationOrigin { get; set; }
        public List<Comment> Comments { get; set; }
        
        public object SpellCheckUnit { get; set; }
        public object QaCheckUnit { get; set; }

        public string MatchTypeId { get; set; }


        public SegmentPair()
        {
            
            SourceSections = new List<SegmentSection>();
            TargetSections = new List<SegmentSection>();

            Source = string.Empty;
            Target = string.Empty;

            ParagraphId = string.Empty;

            Id = string.Empty;


            SegmentStatus = string.Empty;
            IsLocked = false;
            TranslationOrigin = new TranslationOrigin();
            Comments = new List<Comment>();

            SpellCheckUnit = null;
            QaCheckUnit = null;

            MatchTypeId = string.Empty;
        }
    }
}
