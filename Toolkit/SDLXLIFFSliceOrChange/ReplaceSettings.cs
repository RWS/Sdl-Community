using System;

namespace SDLXLIFFSliceOrChange
{
    public class ReplaceSettings
    {
        public String SourceSearchText { get; set; }
        public String SourceReplaceText { get; set; }
        public String TargetSearchText { get; set; }
        public String TargetReplaceText { get; set; }

        public bool MatchCase { get; set; }
        public bool MatchWholeWord { get; set; }
        public bool UseRegEx { get; set; }
    }
}