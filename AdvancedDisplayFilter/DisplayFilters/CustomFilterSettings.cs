using System.Collections.Generic;

namespace Sdl.Community.AdvancedDisplayFilter.DisplayFilters
{
	public class CustomFilterSettings
	{						
		public DisplayFilterSettings.LogicalOperators FilterAttributesLogicalOperator { get; set; }

		public DisplayFilterSettings.LogicalOperators SourceTargetLogicalOperator { get; set; }

		public bool UseBackreferences { get; set; }

		public bool QualitySamplingSegmentSelection { get; set; }

		public bool QualitySamplingMinMaxCharacters { get; set; }

		public bool QualitySamplingRandomlySelect { get; set; }

		public bool QualitySamplingSelectOneInEvery { get; set; }

		public int QualitySamplingRandomlySelectValue { get; set; }

		public int QualitySamplingSelectOneInEveryValue { get; set; }

		public int QualitySamplingMinCharsValue { get; set; }

		public int QualitySamplingMaxCharsValue { get; set; }
		
		public List<string> QualitySamplingSegmentsIds { get; set; }

		public bool OddsNo { get; set; }

		public bool EvenNo { get; set; }

		public bool Grouped { get; set; }

		public bool None { get; set; }

		public string CommaSeparatedVelues { get; set; }

		public string GroupedList { get; set; }

		public bool UseRegexCommentSearch { get; set; }

		public string CommentRegex { get; set; }

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

		public string DocumentStructureInformation { get; set; }

		public bool SearchInTagContent { get; set; }
	
		public bool SearchInTagContentAndText { get; set; }
	}
}