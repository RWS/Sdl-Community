using System.Collections.Generic;

namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    internal class SegmentPair
    {
        internal string Source { get; set; }
        internal string Target { get; set; }

        internal List<SegmentSection> SourceSections { get; set; }
        internal List<SegmentSection> TargetSections { get; set; }

        internal string Id { get; set; }
        internal string SegmentStatus { get; set; }
        internal bool IsLocked { get; set; }
        internal TranslationOrigin TranslationOrigin { get; set; }
        internal List<Comment> Comments { get; set; }

        internal SegmentPair()
        {
            SourceSections = new List<SegmentSection>();
            TargetSections = new List<SegmentSection>();

            Source = "";
            Target = "";
            Id = "";
            SegmentStatus = "";
            IsLocked = false;
            TranslationOrigin = new TranslationOrigin();
            Comments = new List<Comment>();
        }
    }
}
