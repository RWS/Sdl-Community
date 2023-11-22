using System;
using System.Net;

namespace SDLXLIFFSliceOrChange
{
    public class ReplaceSettings
    {
	    public string SourceSearchText { get; set; }

		public string SourceReplaceText { get; set; }

		public string TargetSearchText { get; set; }

		public string TargetReplaceText { get; set; }

	    public bool MatchCase { get; set; }

        public bool MatchWholeWord { get; set; }

        public bool UseRegEx { get; set; }
    }
}