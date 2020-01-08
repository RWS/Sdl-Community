using System;
using System.Net;

namespace SDLXLIFFSliceOrChange
{
    public class ReplaceSettings
    {
	    private string _sourceSearchText;
	    private string _sourceReplaceText;
	    private string _targetSearchText;
	    private string _targetReplaceText;

	    public string SourceSearchText
	    {
		    get => _sourceSearchText;
		    set { _sourceSearchText = WebUtility.HtmlEncode(value); }
	    }

	    public string SourceReplaceText
	    {
		    get => _sourceReplaceText;
		    set { _sourceReplaceText = WebUtility.HtmlEncode(value); }
	    }

	    public string TargetSearchText
	    {
		    get => _targetSearchText;
		    set { _targetSearchText = WebUtility.HtmlEncode(value); }
	    }

	    public string TargetReplaceText
	    {
		    get => _targetReplaceText;
		    set { _targetReplaceText = WebUtility.HtmlEncode(value); }
	    }

	    public bool MatchCase { get; set; }
        public bool MatchWholeWord { get; set; }
        public bool UseRegEx { get; set; }
    }
}