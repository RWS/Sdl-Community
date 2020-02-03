using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.Community.Toolkit.FileType;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.Extensions
{
	public static class DisplayFilterRowInfoExtension
	{
		public static bool IsSegmentReviewTypes(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = rowInfo.IsSegmentWithTQAs(settings)
			              || rowInfo.IsSegmentWithTrackedChanges(settings)
			              || rowInfo.IsSegmentWithSourceTrackedChanges(settings)
			              || rowInfo.IsSegmentWithTargetTrackedChanges(settings)
						  ||rowInfo.IsSegmentWithSourceAndTargetTrackedChanges(settings)
			              || rowInfo.IsSegmentWithComments(settings)
			              || rowInfo.IsSegmentWithMessages(settings);

			return success;
		}

		public static bool IsSegmentWithTQAs(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentReviewTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentReviewType.WithTQA.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success && !rowInfo.ContainsTQAs)
			{
				success = false;
			}
			
			return success;
		}

		public static bool IsSegmentWithTrackedChanges(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var sourceContainsTrackChanges = SegmentContainsTrackedChanges(rowInfo.SegmentPair.Source);
			var targetContainsTrackChanges = SegmentContainsTrackedChanges(rowInfo.SegmentPair.Target);

			var success = HasReviewTypeSelected(DisplayFilterSettings.SegmentReviewType.WithTrackedChanges.ToString(), settings.SegmentReviewTypes);

			var containsTrackChanges = sourceContainsTrackChanges || targetContainsTrackChanges;

			if (success && !containsTrackChanges)
			{
				success = false;
			}

			return success;
		}

		public static bool IsSegmentWithSourceTrackedChanges(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var sourceContainsTrackChanges = SegmentContainsTrackedChanges(rowInfo.SegmentPair.Source);

			var success = HasReviewTypeSelected(DisplayFilterSettings.SegmentReviewType.WithSourceTrackedChanges.ToString(), settings.SegmentReviewTypes);

			if (success && !sourceContainsTrackChanges)
			{
				success = false;
			}

			return success;
		}
		public static bool IsSegmentWithTargetTrackedChanges(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}
			var targetContainsTrackChanges = SegmentContainsTrackedChanges(rowInfo.SegmentPair.Target);

			var success = HasReviewTypeSelected(DisplayFilterSettings.SegmentReviewType.WithTargetTrackedChanges.ToString(),settings.SegmentReviewTypes);

			if (success && !targetContainsTrackChanges)
			{
				success = false;
			}

			return success;
		}

		public static bool IsSegmentWithSourceAndTargetTrackedChanges(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var sourceContainsTrackChanges = SegmentContainsTrackedChanges(rowInfo.SegmentPair.Source);
			var targetContainsTrackChanges = SegmentContainsTrackedChanges(rowInfo.SegmentPair.Target);

			var success = HasReviewTypeSelected(DisplayFilterSettings.SegmentReviewType.WithSourceAndTargetTrackedChanges.ToString(), settings.SegmentReviewTypes);

			var containsTrackChanges = sourceContainsTrackChanges && targetContainsTrackChanges;

			if (success && !containsTrackChanges)
			{
				success = false;
			}

			return success;
		}


		public static bool IsSegmentWithComments(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentReviewTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentReviewType.WithComments.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			var visitor = new CommentDataVisitor();

			// check if comments exist in the target segment
			if (success && !visitor.GetComments(rowInfo.SegmentPair.Target).Any())
			{
				// check if comments exit in the source segment
				if (!visitor.GetComments(rowInfo.SegmentPair.Source).Any())
				{
					success = false;
				}
			}

			return success;
		}

		public static bool IsSegmentWithMessages(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentReviewTypes.ToList()
			   .Any(status => string.Compare(status, DisplayFilterSettings.SegmentReviewType.WithFeedbackMessages.ToString()
				   , StringComparison.OrdinalIgnoreCase) == 0);

			if (success && !rowInfo.ContainsFeedbackMessages)
			{
				success = false;
			}

			return success;
		}

		public static bool IsSeverityFoundInComment(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var visitor = new CommentDataVisitor();

			var success = visitor.GetComments(rowInfo.SegmentPair.Target)
				.Any(comment => settings.CommentSeverity == (int)comment.Severity);
			if (!success)
			{
				success = visitor.GetComments(rowInfo.SegmentPair.Source)
				.Any(comment => settings.CommentSeverity == (int)comment.Severity);
			}

			return success;
		}

		public static bool IsAuthorFoundInComment(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var visitor = new CommentDataVisitor();

			var success = visitor.GetComments(rowInfo.SegmentPair.Target)
				.Any(comment => StringMatch(settings.CommentAuthor, comment.Author, false));
			if (!success)
			{
				success = visitor.GetComments(rowInfo.SegmentPair.Source)
				.Any(comment => StringMatch(settings.CommentAuthor, comment.Author, false));
			}

			return success;
		}

		public static bool IsTextFoundInComment(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var visitor = new CommentDataVisitor();

			var success = visitor.GetComments(rowInfo.SegmentPair.Target)
				.Any(comment => StringMatch(settings.CommentText, comment.Text, false));


			if (!success)
			{
				success = visitor.GetComments(rowInfo.SegmentPair.Source)
				.Any(comment => StringMatch(settings.CommentText, comment.Text, false));
			}

			return success;
		}

		public static bool IsConfirmationLevelFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.ConfirmationLevels.ToList().Any(status => string.Compare(status
				, rowInfo.SegmentPair.Properties.ConfirmationLevel.ToString()
				, StringComparison.OrdinalIgnoreCase) == 0);

			return success;
		}

		public static bool IsOriginTypeFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = false;

			var translationType = rowInfo.SegmentPair.GetOriginType();

			if (!IsCompoundOriginType(translationType) && settings.OriginTypes.ToList().Any(status
					=> string.Compare(status, translationType.ToString(), StringComparison.OrdinalIgnoreCase) == 0))
			{
				success = true;
			}

			if (!success)
			{
				success = rowInfo.IsFuzzyMatchRepairOriginTypeFound(settings);
			}

			if (!success)
			{
				success = rowInfo.IsEditedFuzzyMatchFound(settings);
			}

			if (!success)
			{
				success = rowInfo.IsUnEditedFuzzyMatchFound(settings);
			}

			var isNewContentOptionSelected = settings.OriginTypes.ToList().Any(origin =>
				string.Compare(origin, DisplayFilterSettings.OriginType.NewTranslated.ToString(), StringComparison.OrdinalIgnoreCase) == 0);

			if (isNewContentOptionSelected)
			{
				success = SegmentTypesHelper.IsNewContent(rowInfo);
			}

			return success;
		}

		public static bool IsFuzzyMatchRepairOriginTypeFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			if (settings.OriginTypes.Any(a => a == OriginType.FuzzyMatchRepair.ToString()) &&
				rowInfo.SegmentPair.Properties.TranslationOrigin.OriginType == "tm" &&
				rowInfo.SegmentPair.Properties.TranslationOrigin.MetaDataContainsKey(OriginType.FuzzyMatchRepair.ToString()))
			{
				var value = rowInfo.SegmentPair.Properties.TranslationOrigin.GetMetaData(OriginType.FuzzyMatchRepair.ToString());
				return Convert.ToBoolean(value);
			}

			return false;
		}

		public static bool IsPreviousOriginTypeFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = false;

			if (rowInfo.SegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation != null)
			{
				var previousTranslationType = rowInfo.SegmentPair.GetPreviousTranslationOriginType();

				if (!IsCompoundOriginType(previousTranslationType) && settings.PreviousOriginTypes.ToList().Any(status
						=> string.Compare(status, previousTranslationType.ToString(), StringComparison.OrdinalIgnoreCase) == 0))
				{
					success = true;
				}

				if (!success)
				{
					success = rowInfo.IsFuzzyMatchRepairPreviousOriginTypeFound(settings);
				}

				if (!success)
				{
					success = rowInfo.IsPreviousEditedFuzzyMatchFound(settings);
				}

				if (!success)
				{
					success = rowInfo.IsPreviousUnEditedFuzzyMatchFound(settings);
				}
			}

			return success;
		}

		private static bool IsCompoundOriginType(OriginType originType)
		{
			return originType == OriginType.FuzzyMatchRepair || 
			       originType == OriginType.EditedFuzzy ||
				   originType == OriginType.UneditedFuzzy;
		}

		public static bool IsFuzzyMatchRepairPreviousOriginTypeFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			if (rowInfo.SegmentPair.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginType == null)
			{
				return false;
			}

			if (settings.PreviousOriginTypes.Any(a => a == OriginType.FuzzyMatchRepair.ToString()) &&
				rowInfo.SegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginType == "tm" &&
				rowInfo.SegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.MetaDataContainsKey(OriginType.FuzzyMatchRepair.ToString()))
			{
				var value = rowInfo.SegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.GetMetaData(OriginType.FuzzyMatchRepair.ToString());
				return Convert.ToBoolean(value);
			}

			return false;
		}

		public static bool IsRepetitionTypes(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = rowInfo.IsRepetitionsAll(settings)
				|| rowInfo.IsRepetitionsFirstOccurrences(settings)
				|| rowInfo.IsRepetitionsExcludingFirstOccurrences(settings);

			if (!success)
			{
				success = rowInfo.IsUniqueRepetition(settings);
			}

			return success;
		}

		public static bool IsUniqueRepetition(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			if (settings.RepetitionTypes.Any(a => a == DisplayFilterSettings.RepetitionType.UniqueOccurrences.ToString()))
			{
				var isFirst = rowInfo.IsRepetitionsFirstOccurrences(new DisplayFilterSettings
				{
					RepetitionTypes = new List<string> { DisplayFilterSettings.RepetitionType.FirstOccurrences.ToString() }
				});

				if (isFirst)
				{
					return true;
				}

				if (rowInfo.SegmentPair.Properties.TranslationOrigin != null)
				{
					var isRepeated = rowInfo.SegmentPair.Properties.TranslationOrigin.IsRepeated;
					return !isRepeated;
				}
			}

			return false;
		}

		public static bool IsRepetitionsAll(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.RepetitionTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.RepetitionType.All.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.SegmentPair.Properties.TranslationOrigin.IsRepeated;
			}

			return success;
		}

		public static bool IsRepetitionsFirstOccurrences(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.RepetitionTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.RepetitionType.FirstOccurrences.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.RepetitionFirstOccurrence;
			}

			return success;
		}

		public static bool IsRepetitionsExcludingFirstOccurrences(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.RepetitionTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.RepetitionType.ExcludeFirstOccurrences.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.RepetitionExcludeFirstOccurrence;
			}

			return success;
		}

		public static bool IsSegmentLockingTypes(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = rowInfo.IsSegmentLockingTypeLocked(settings)
				|| rowInfo.IsSegmentLockingTypeUnLocked(settings);

			return success;
		}

		public static bool IsSegmentLockingTypeLocked(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentLockingTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentLockingType.Locked.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.SegmentPair.Properties.IsLocked;
			}

			return success;
		}

		public static bool IsSegmentLockingTypeUnLocked(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentLockingTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentLockingType.Unlocked.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = !rowInfo.SegmentPair.Properties.IsLocked;
			}

			return success;
		}

		public static bool IsSegmentContentTypes(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentContentTypes?[0] == DisplayFilterSettings.SegmentContentType.NumbersOnly.ToString() &&
				rowInfo.IsSegmentContentTypeNumbersOnly(settings);

			if (!success && settings.SegmentContentTypes?[0] == DisplayFilterSettings.SegmentContentType.ExcludeNumberOnly.ToString() &&
				rowInfo.IsSegmentContentTypeExcludingNumberOnly(settings))
			{
				success = true;
			}

			return success;
		}

		public static bool IsSegmentContentTypeNumbersOnly(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentContentTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentContentType.NumbersOnly.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.SegmentPair.Source.IsValidFloatingNumber();
				if (!success)
				{
					success = rowInfo.SegmentPair.Target.IsValidFloatingNumber();
				}
			}

			return success;
		}

		public static bool IsSegmentContentTypeExcludingNumberOnly(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = settings.SegmentContentTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentContentType.ExcludeNumberOnly.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.SegmentPair.Source.IsValidFloatingNumber();
				if (success)
				{
					return false;
				}

				success = rowInfo.SegmentPair.Target.IsValidFloatingNumber();
			}

			return !success;
		}

		public static bool IsTextFoundInSource(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var text = rowInfo.SegmentPair.Source.GetString(true);

			var success = settings.IsRegularExpression
				? RegularExpressionMatch(settings.SourceText, text, settings.IsCaseSensitive)
				: StringMatch(settings.SourceText, text, settings.IsCaseSensitive);

			return success;
		}

		public static bool IsTextFoundInTarget(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var text = rowInfo.SegmentPair.Target.GetString(true);

			var success = settings.IsRegularExpression
				? RegularExpressionMatch(settings.TargetText, text, settings.IsCaseSensitive)
				: StringMatch(settings.TargetText, text, settings.IsCaseSensitive);

			return success;
		}

		public static bool IsContextInfoTypes(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var success = false;

			if (rowInfo.ContextInfo.Count <= 0)
			{
				return false;
			}

			if (rowInfo.ContextInfo.Any(contextInfo => settings.ContextInfoTypes.Contains(contextInfo.ContextType)))
			{
				success = true;
			}

			return success;
		}
		public static bool IsEditedFuzzyMatchFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			return settings.OriginTypes.Any(a => a == OriginType.EditedFuzzy.ToString()) &&
				   FuzzyHelper.ContainsFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin) &&
				   FuzzyHelper.IsEditedFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin);
		}

		public static bool IsUnEditedFuzzyMatchFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			return settings.OriginTypes.Any(a => a == OriginType.UneditedFuzzy.ToString()) &&
				   FuzzyHelper.ContainsFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin) &&
				   !FuzzyHelper.IsEditedFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin);
		}

		public static bool IsPreviousEditedFuzzyMatchFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			return settings.PreviousOriginTypes.Any(a => a == OriginType.EditedFuzzy.ToString()) &&
				   FuzzyHelper.ContainsFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin?.OriginBeforeAdaptation) &&
				   FuzzyHelper.IsEditedFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin?.OriginBeforeAdaptation);
		}

		public static bool IsPreviousUnEditedFuzzyMatchFound(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			return settings.PreviousOriginTypes.Any(a => a == OriginType.UneditedFuzzy.ToString()) &&
				   FuzzyHelper.ContainsFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin?.OriginBeforeAdaptation) &&
				   !FuzzyHelper.IsEditedFuzzyMatch(rowInfo.SegmentPair.Target?.Properties?.TranslationOrigin?.OriginBeforeAdaptation);
		}

		private static bool RegularExpressionMatch(string searchFor, string searchIn, bool isCaseSensitive)
		{
			var regex = new Regex(searchFor,
				RegexOptions.Singleline | (!isCaseSensitive ? RegexOptions.IgnoreCase : RegexOptions.None));
			var match = regex.Match(searchIn);

			return match.Success;
		}

		private static bool StringMatch(string searchFor, string searchIn, bool isCaseSensitive)
		{
			if (searchIn == null || searchFor == null)
			{
				return false;
			}

			if (isCaseSensitive)
			{
				return searchIn.IndexOf(searchFor, StringComparison.Ordinal) > -1;
			}

			return searchIn.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) > -1;
		}

		private static bool HasReviewTypeSelected(string selectedOption, List<string> segmentReviewTyes)
		{
			return segmentReviewTyes.Any(status => string.Compare(status, selectedOption, StringComparison.OrdinalIgnoreCase) == 0);
		}

		private static bool SegmentContainsTrackedChanges(ISegment segment)
		{
			var segmentVisitor = new SegmentTextVisitor();

			return segmentVisitor.ContainsTrackChanges(segment);
		}
		
	}
}
