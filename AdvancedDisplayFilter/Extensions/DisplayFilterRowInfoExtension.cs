﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.Toolkit.FileType;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.Extensions
{
	public static class DisplayFilterRowInfoExtension
	{
		public static bool IsSegmentReviewTypes(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
			var success = rowInfo.IsSegmentWithTQAs(settings)
				|| rowInfo.IsSegmentWithTrackedChanges(settings)
				|| rowInfo.IsSegmentWithComments(settings)
				|| rowInfo.IsSegmentWithMessages(settings);

			return success;
		}

		public static bool IsSegmentWithTQAs(this DisplayFilterRowInfo rowInfo, DisplayFilterSettings settings)
		{
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
			var success = settings.SegmentReviewTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentReviewType.WithTrackedChanges.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success && !rowInfo.ContainsTrackChanges)
			{
				success = false;
			}

			return success;
		}

		public static bool IsSegmentWithComments(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.SegmentReviewTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentReviewType.WithComments.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			// check if comments exist in the target segment
			if (success && !rowInfo.SegmentPair.Target.GetComments().Any())
			{
				// check if comments exit in the source segment
				if (!rowInfo.SegmentPair.Source.GetComments().Any())
				{
					success = false;
				}
			}

			return success;
		}

		public static bool IsSegmentWithMessages(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.SegmentReviewTypes.ToList()
			   .Any(status => string.Compare(status, DisplayFilterSettings.SegmentReviewType.WithFeedbackMessages.ToString()
				   , StringComparison.OrdinalIgnoreCase) == 0);

			if (success && !rowInfo.ContainsFeedbackMessages)
			{
				success = false;
			}

			return success;
		}

		public static bool IsSeverityFoundInComment(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = rowInfo.SegmentPair.Target.GetComments()
				.Any(comment => settings.CommentSeverity == (int)comment.Severity);
			if (!success)
			{
				success = rowInfo.SegmentPair.Source.GetComments()
				.Any(comment => settings.CommentSeverity == (int)comment.Severity);
			}

			return success;
		}

		public static bool IsAuthorFoundInComment(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = rowInfo.SegmentPair.Target.GetComments()
				.Any(comment => StringMatch(settings.CommentAuthor, comment.Author, false));
			if (!success)
			{
				success = rowInfo.SegmentPair.Source.GetComments()
				.Any(comment => StringMatch(settings.CommentAuthor, comment.Author, false));
			}

			return success;
		}

		public static bool IsTextFoundInComment(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = rowInfo.SegmentPair.Target.GetComments()
				.Any(comment => StringMatch(settings.CommentText, comment.Text, false));
			if (!success)
			{
				success = rowInfo.SegmentPair.Source.GetComments()
				.Any(comment => StringMatch(settings.CommentText, comment.Text, false));
			}

			return success;
		}

		public static bool IsConfirmationLevelFound(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.ConfirmationLevels.ToList().Any(status => string.Compare(status
				, rowInfo.SegmentPair.Properties.ConfirmationLevel.ToString()
				, StringComparison.OrdinalIgnoreCase) == 0);

			return success;
		}

		public static bool IsOriginTypeFound(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var translationType =
				rowInfo.SegmentPair.GetOriginType();

			var success = settings.OriginTypes.ToList()
				.Any(status => string.Compare(status, translationType.ToString()
					               , StringComparison.OrdinalIgnoreCase) == 0);

			return success;
		}
		public static bool IsPreviousOriginTypeFound(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = false;

			if (rowInfo.SegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation != null)
			{
				var previousTranslationType =
					rowInfo.SegmentPair.GetPreviousTranslationOriginType();
				if (settings.PreviousOriginTypes.ToList()
					.Any(status => string.Compare(status,
						               previousTranslationType.ToString()
						               , StringComparison.OrdinalIgnoreCase) == 0))
				{
					success = true;
				}
			}

			return success;
		}


		public static bool IsRepetitionTypes(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = rowInfo.IsRepetitionsAll(settings)
				|| rowInfo.IsRepetitionsFirstOccurrences(settings)
				|| rowInfo.IsRepetitionsExcludingFirstOccurrences(settings);

			return success;
		}
		public static bool IsRepetitionsAll(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.RepetitionTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.RepetitionType.All.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.SegmentPair.Properties.TranslationOrigin.IsRepeated;
			}

			return success;
		}
		public static bool IsRepetitionsFirstOccurrences(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.RepetitionTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.RepetitionType.FirstOccurrences.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.RepetitionFirstOccurrence;
			}

			return success;
		}
		public static bool IsRepetitionsExcludingFirstOccurrences(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.RepetitionTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.RepetitionType.ExcludeFirstOccurrences.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.RepetitionExcludeFirstOccurrence;
			}

			return success;
		}


		public static bool IsSegmentLockingTypes(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = rowInfo.IsSegmentLockingTypeLocked(settings)
				|| rowInfo.IsSegmentLockingTypeUnLocked(settings);

			return success;
		}
		public static bool IsSegmentLockingTypeLocked(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.SegmentLockingTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentLockingType.Locked.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = rowInfo.SegmentPair.Properties.IsLocked;
			}

			return success;
		}
		public static bool IsSegmentLockingTypeUnLocked(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var success = settings.SegmentLockingTypes.ToList()
				.Any(status => string.Compare(status, DisplayFilterSettings.SegmentLockingType.Unlocked.ToString()
					, StringComparison.OrdinalIgnoreCase) == 0);

			if (success)
			{
				success = !rowInfo.SegmentPair.Properties.IsLocked;
			}

			return success;
		}


		public static bool IsSegmentContentTypes(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			if (settings.SegmentContentTypes?[0] == "NumbersOnly")
			{
				return rowInfo.IsSegmentContentTypeNumbersOnly(settings);
			}

			if (settings.SegmentContentTypes?[0] == "ExcludeNumberOnly")
			{
				return rowInfo.IsSegmentContentTypeExcludingNumberOnly(settings);
			}

			return true;
		}
		public static bool IsSegmentContentTypeNumbersOnly(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
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

		public static bool IsSegmentContentTypeExcludingNumberOnly(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
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

		public static bool IsTextFoundInSource(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var text = rowInfo.SegmentPair.Source.GetString(true);

			var success = settings.IsRegularExpression
				? RegularExpressionMatch(settings.SourceText, text, settings.IsCaseSensitive)
				: StringMatch(settings.SourceText, text, settings.IsCaseSensitive);

			return success;
		}

		public static bool IsTextFoundInTarget(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
			var text = rowInfo.SegmentPair.Target.GetString(true);

			var success = settings.IsRegularExpression
				? RegularExpressionMatch(settings.TargetText, text, settings.IsCaseSensitive)
				: StringMatch(settings.TargetText, text, settings.IsCaseSensitive);

			return success;
		}

		public static bool IsContextInfoTypes(this DisplayFilterRowInfo rowInfo,
			DisplayFilterSettings settings)
		{
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

		private static bool RegularExpressionMatch(string searchFor, string searchIn, bool isCaseSensitive)
		{
			var regex = new Regex(searchFor,
				RegexOptions.Singleline | (!isCaseSensitive ? RegexOptions.IgnoreCase : RegexOptions.None));
			var match = regex.Match(searchIn);

			return match.Success;
		}
		private static bool StringMatch(string searchFor, string searchIn, bool isCaseSensitive)
		{
			if (isCaseSensitive)
			{
				return searchIn.IndexOf(searchFor, StringComparison.Ordinal) > -1 ? true : false;
			}

			return searchIn.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) > -1 ? true : false;
		}
	}
}
