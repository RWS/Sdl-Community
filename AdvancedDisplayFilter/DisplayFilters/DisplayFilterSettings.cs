using System;
using System.Collections.Generic;
using Sdl.Community.Toolkit.FileType;

namespace Sdl.Community.AdvancedDisplayFilter.DisplayFilters
{
	public class DisplayFilterSettings
	{
		public enum LogicalOperators
		{
			AND = 0,
			OR = 1
		}

		public enum ContentLocation
		{
			None = 0,
			SourceOrTarget = 1,
			SourceAndTarget = 2,
			Source = 3,
			Target = 4
		}

		public enum ConfirmationLevel
		{
			Unspecified,
			Draft,
			Translated,
			RejectedTranslation,
			ApprovedTranslation,
			RejectedSignOff,
			ApprovedSignOff
		}

		public enum CommentSeverityType
		{
			None = 0,
			Information = 1,
			Warning = 2,
			Error = 3
		}

		public enum RepetitionType
		{
			All,
			FirstOccurrences,
			ExcludeFirstOccurrences,
			UniqueOccurrences
		}

		public enum SegmentReviewType
		{
			WithFeedbackMessages,
			WithComments,
			WithTrackedChanges,
			WithTQA
		}

		public enum SegmentLockingType
		{
			Locked,
			Unlocked
		}

		public enum SegmentContentType
		{
			NumbersOnly,
			ExcludeNumberOnly
		}

		public string Version { get; set; }

		public DateTime Created { get; set; }

		public OriginType Origin { get; set; }

		public bool IsRegularExpression { get; set; }

		public bool IsCaseSensitive { get; set; }

		public string SourceText { get; set; }

		public string TargetText { get; set; }

		public string CommentText { get; set; }

		public string CommentAuthor { get; set; }

		public int CommentSeverity { get; set; }

		public List<string> ContextInfoTypes { get; set; }

		public bool ShowAllContent { get; set; }

		public List<string> RepetitionTypes { get; set; }

		public List<string> SegmentReviewTypes { get; set; }

		public List<string> SegmentLockingTypes { get; set; }

		public List<string> SegmentContentTypes { get; set; }

		public List<string> ConfirmationLevels { get; set; }

		public List<string> OriginTypes { get; set; }

		public List<string> PreviousOriginTypes { get; set; }

		public DisplayFilterSettings()
		{
			Version = "1.0";
			Created = DateTime.Now;

			IsRegularExpression = false;
			IsCaseSensitive = false;
			SourceText = string.Empty;
			TargetText = string.Empty;
			CommentText = string.Empty;
			CommentAuthor = string.Empty;
			CommentSeverity = 0;

			ContextInfoTypes = new List<string>();

			ShowAllContent = false;
			RepetitionTypes = new List<string>();
			SegmentReviewTypes = new List<string>();
			SegmentLockingTypes = new List<string>();
			SegmentContentTypes = new List<string>();

			ConfirmationLevels = new List<string>();
			OriginTypes = new List<string>();
			PreviousOriginTypes = new List<string>();
		}
	}
}
