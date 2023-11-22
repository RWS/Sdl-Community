using System.Text.RegularExpressions;
using Sdl.Community.Toolkit.FileType.Processors.API;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Models
{
	public class MatchRule: IMatchRule
	{
	    private SegmentationHint _defaultSegmentationHint = SegmentationHint.MayExclude;
	    private TagTypeOption _defaultTagType = TagTypeOption.Placeholder;
	    private string _defaultStartTagRegex = string.Empty;
	    private string _defaultEndTagRegex = string.Empty;
	    private bool _defaultIgnoreCase = false;
	    private bool _defaultContentTranslatable = false;
	    private bool _defaultWordStop = false;
	    private bool _defaultSoftBreak = false;
	    private bool _defaultCanHide = false;
	    private string _defaultTextEquivalent = string.Empty;

	    
		public SegmentationHint SegmentationHint { get; set; }

		public TagTypeOption TagType { get; set; }

		public string StartTagRegexValue { get; set; }

		public string EndTagRegexValue { get; set; }

		public bool IgnoreCase { get; set; }

		public bool IsContentTranslatable { get; set; }

		public bool IsWordStop { get; set; }

		public bool IsSoftBreak { get; set; }

		public bool CanHide { get; set; }

		public string TextEquivalent { get; set; }

		public FormattingGroupSettings Formatting { get; set; }

		public Regex BuildStartTagRegex()
		{
			return new Regex(StartTagRegexValue,
				IgnoreCase
				? RegexOptions.IgnoreCase
				: RegexOptions.None);
		}

		public Regex BuildEndTagRegex()
		{
			return new Regex(EndTagRegexValue,
				IgnoreCase
				? RegexOptions.IgnoreCase
				: RegexOptions.None);
		}

	    public object Clone()
	    {
		    return new MatchRule
		    {
			    SegmentationHint = SegmentationHint,
			    TagType = TagType,
			    StartTagRegexValue = StartTagRegexValue,
			    EndTagRegexValue = EndTagRegexValue,
			    IgnoreCase = IgnoreCase,
			    IsContentTranslatable = IsContentTranslatable,
			    IsWordStop = IsWordStop,
			    IsSoftBreak = IsSoftBreak,
			    CanHide = CanHide,
			    TextEquivalent = TextEquivalent,
			    Formatting = Formatting
		    };
	    }
	}
}
