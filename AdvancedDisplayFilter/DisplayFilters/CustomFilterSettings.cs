using System.Collections.Generic;

namespace Sdl.Community.AdvancedDisplayFilter.DisplayFilters
{
	public class CustomFilterSettings
	{
		public enum LogicalOperators
		{			
			And = 0,
			Or = 1
		}

		public enum OriginTypeExtended
		{
			FuzzyMatchRepair,
			EditedFuzzy,
			UneditedFuzzy
		}

		public enum RepetitionTypeExtended
		{
			UniqueOccurrences
		}

		public LogicalOperators SourceAndTargetLogicalOperator { get; set; }
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
		public string DocumentStructureInfoLocation { get; set; }
		public bool UseTagContent { get; set; }

		/// <summary>
		/// Set this to true to ALSO search inside tags and
		/// Set this to false to search ONLY inside tags
		/// </summary>
		public bool AndOrTagContent { get; set; }
	}
}