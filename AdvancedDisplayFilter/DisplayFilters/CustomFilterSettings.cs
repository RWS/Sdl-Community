using System.Collections.Generic;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters
{
	public class CustomFilterSettings
	{
		public bool OddsNo { get; set; }
		public bool EvenNo { get; set; }
		public bool Grouped { get; set; }
		public bool None { get; set; }
		public string CommaSeparatedVelues { get; set; }
		public string GroupedList { get; set; }
		public bool UseRegexCommentSearch { get; set; }
		public string CommentRegex { get; set; }
		public bool Unique { get; set; }
		public List<string> Colors { get; set; }
		public string FuzzyMin { get; set; }
		public string FuzzyMax { get; set; }
		public bool SplitSegments { get; set; }
		public bool MergedSegments { get; set; }
		public bool MergedAcross { get; set; }
		public bool SourceEqualsTarget { get; set; }
		public bool IsEqualsCaseSensitive { get; set; }
		public bool ContainsTags { get; set; }
		public bool ModifiedByChecked { get; set; }
		public string ModifiedBy { get; set; }
		public bool CreatedByChecked { get; set; }
		public string CreatedBy { get; set; }
		public bool EditedFuzzy { get; set; }
		public bool UnEditedFuzzy { get; set; }
	}
}