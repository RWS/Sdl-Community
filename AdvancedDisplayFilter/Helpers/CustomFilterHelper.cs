using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.Toolkit.FileType;
using Sdl.Community.Toolkit.Integration;
using Sdl.Community.Toolkit.Integration.DisplayFilter;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class CustomFilterHelper
	{
		public static bool Filter(CustomFilterSettings customSettings, DisplayFilterSettings filterSettings,
			DisplayFilterRowInfo rowInfo, bool success, Document activeDocument)
		{

			var rowId = rowInfo.SegmentPair.Properties.Id.Id;
			if (success && customSettings.EvenNo)
			{
				success = SegmentNumbersHelper.IsEven(rowId);
			}
			if (success && customSettings.OddsNo)
			{
				success = SegmentNumbersHelper.IsOdd(rowId);
			}
			if (success && customSettings.SplitSegments)
			{
				success = SegmentNumbersHelper.IsSplitSegment(rowId, activeDocument);
			}
			if (success && (customSettings.MergedSegments || customSettings.MergedAcross))
			{
				success = SegmentNumbersHelper.IsMergedSegment(rowId, activeDocument, customSettings.MergedAcross);
			}

			if (success && customSettings.SourceEqualsTarget)
			{
				success = SegmentNumbersHelper.IsSourceEqualsToTarget(rowInfo.SegmentPair, customSettings.IsEqualsCaseSensitive);
			}
			if (success && customSettings.Grouped && !string.IsNullOrWhiteSpace(customSettings.GroupedList))
			{
				success = SegmentNumbersHelper.IdInRange(rowId, customSettings.GroupedList);
			}
			if (success && customSettings.UseRegexCommentSearch &&
			    !string.IsNullOrWhiteSpace(customSettings.CommentRegex))
			{
				//create a list with source and target comments
				var commentsList = rowInfo.SegmentPair.Source.GetComments();
				commentsList.AddRange(rowInfo.SegmentPair.Target.GetComments());

				success = CommentsHelper.IsCommentTextFoundWithRegex(commentsList, customSettings.CommentRegex);
			}
			if (success && customSettings.Colors.Count > 0)
			{
				try
				{
					success = ColorPickerHelper.ContainsColor(rowInfo, customSettings.Colors);
				}
				catch (Exception e)
				{
				}
			}

			//fuzzy
			if (success && !string.IsNullOrWhiteSpace(customSettings.FuzzyMin) &&
			    !string.IsNullOrWhiteSpace(customSettings.FuzzyMax))
			{
				success = FuzzyHelper.IsInFuzzyRange(rowInfo, customSettings.FuzzyMin, customSettings.FuzzyMax);

			}
			if (success && customSettings.ContainsTags)
			{
				var containsTagVisitor = new TagVisitor();
				success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
			}
			//unique 
			if (success && customSettings.Unique)
			{

				var settings = new DisplayFilterSettings
				{
					RepetitionTypes = new List<string>
					{
						"FirstOccurrences"
					}
				};

				var isFirst = rowInfo.IsRepetitionsFirstOccurrences(settings);
				if (isFirst)
				{
					return true;
				}

				if (rowInfo.SegmentPair.Properties.TranslationOrigin != null)
				{
					var isRepeated = rowInfo.SegmentPair.Properties.TranslationOrigin.IsRepeated;
					return !isRepeated;
				}

				return false;
			}
			//created by
			if (success && customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(customSettings.CreatedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, customSettings.CreatedBy);
			}
			//modify by
			if (success && customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(customSettings.ModifiedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, customSettings.ModifiedBy);
			}
			if (success && customSettings.EditedFuzzy)
			{
				success = FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target) &&
				          FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			}
			if (success && customSettings.UnEditedFuzzy)
			{
				success = FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target) &&
				          !FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			}

			//String id seach
			if (success && !string.IsNullOrEmpty(customSettings.ContextInfoStringId))
			{
				if (filterSettings.IsRegularExpression)
				{
					success = StringIdRegexSearch(rowInfo, customSettings.ContextInfoStringId, filterSettings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
				}
				else
				{
					foreach (var contextInfo in rowInfo.ContextInfo)
					{
						if (contextInfo?.DisplayName == customSettings.ContextInfoStringId)
						{
							return true;
						}
					}
					return false;
				}
			}
			return success;
		}

		private static bool StringIdRegexSearch(DisplayFilterRowInfo rowInfo,string regexExpression,RegexOptions options)
		{
			var regex = new Regex(regexExpression, options);
			foreach (var contextInfo in rowInfo.ContextInfo)
			{
				if (contextInfo != null)
				{
					if (regex.Match(contextInfo.DisplayName).Success)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static  bool Reverse(DisplayFilterSettings settings,bool success, DisplayFilterRowInfo rowInfo, CustomFilterSettings customSettings, Document activeDocument)
		{
			success = false;
			if (!success && settings.SegmentReviewTypes != null && settings.SegmentReviewTypes.Any())
				success = rowInfo.IsSegmentReviewTypes(settings);


			if (!success && settings.ConfirmationLevels != null && settings.ConfirmationLevels.Any())
				success = rowInfo.IsConfirmationLevelFound(settings);


			if (!success && settings.OriginTypes != null && settings.OriginTypes.Any())
				success = rowInfo.IsOriginTypeFound(settings);


			if (!success && settings.PreviousOriginTypes != null && settings.PreviousOriginTypes.Any())
				success = rowInfo.IsPreviousOriginTypeFound(settings);


			if (!success && settings.RepetitionTypes != null && settings.RepetitionTypes.Any())
				success = rowInfo.IsRepetitionTypes(settings);


			if (!success && settings.SegmentLockingTypes != null && settings.SegmentLockingTypes.Any())
				success = rowInfo.IsSegmentLockingTypes(settings);


			if (!success && settings.SegmentContentTypes != null && settings.SegmentContentTypes.Any())
				success = rowInfo.IsSegmentContentTypes(settings);


			if (!success && settings.SourceText.Trim() != string.Empty)
				success = rowInfo.IsTextFoundInSource(settings);


			if (!success && settings.TargetText.Trim() != string.Empty)
				success = rowInfo.IsTextFoundInTarget(settings);


			if (!success && !customSettings.UseRegexCommentSearch && settings.CommentText.Trim() != string.Empty)
				success = rowInfo.IsTextFoundInComment(settings);


			if (!success && settings.CommentAuthor.Trim() != string.Empty)
				success = rowInfo.IsAuthorFoundInComment(settings);


			if (!success && settings.CommentSeverity > 0)
				success = rowInfo.IsSeverityFoundInComment(settings);


			if (!success && settings.ContextInfoTypes.Any())
				success = rowInfo.IsContextInfoTypes(settings);

			// check custom settings
			var rowId = rowInfo.SegmentPair.Properties.Id.Id;
			if (!success && customSettings.EvenNo)
			{
				success = SegmentNumbersHelper.IsEven(rowId);
			}
			if (!success && customSettings.OddsNo)
			{
				success = SegmentNumbersHelper.IsOdd(rowId);
			}
			if (!success && customSettings.SplitSegments)
			{
				success = SegmentNumbersHelper.IsSplitSegment(rowId, activeDocument);
			}
			if (!success && (customSettings.MergedSegments || customSettings.MergedAcross))
			{
				success = SegmentNumbersHelper.IsMergedSegment(rowId, activeDocument, customSettings.MergedAcross);
			}
			if (!success && customSettings.SourceEqualsTarget)
			{
				success = SegmentNumbersHelper.IsSourceEqualsToTarget(rowInfo.SegmentPair, customSettings.IsEqualsCaseSensitive);
			}
			if (!success && customSettings.Grouped && !string.IsNullOrWhiteSpace(customSettings.GroupedList))
			{
				success = SegmentNumbersHelper.IdInRange(rowId, customSettings.GroupedList);
			}
			if (!success && customSettings.UseRegexCommentSearch &&
			    !string.IsNullOrWhiteSpace(customSettings.CommentRegex))
			{
				//create a list with source and target comments
				var commentsList = rowInfo.SegmentPair.Source.GetComments();
				commentsList.AddRange(rowInfo.SegmentPair.Target.GetComments());

				success = CommentsHelper.IsCommentTextFoundWithRegex(commentsList, customSettings.CommentRegex);
			}
			if (!success && customSettings.Colors.Count > 0)
			{
				try
				{
					success = ColorPickerHelper.ContainsColor(rowInfo, customSettings.Colors);
				}
				catch (Exception e) { }
			}

			//fuzzy
			if (!success && !string.IsNullOrWhiteSpace(customSettings.FuzzyMin) &&
			    !string.IsNullOrWhiteSpace(customSettings.FuzzyMax))
			{
				success = FuzzyHelper.IsInFuzzyRange(rowInfo, customSettings.FuzzyMin, customSettings.FuzzyMax);

			}
			//tags
			if (!success && customSettings.ContainsTags)
			{
				var containsTagVisitor = new TagVisitor();
				success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
			}
			if (!success && customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(customSettings.CreatedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, customSettings.CreatedBy);
			}
			//modify by
			if (!success && customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(customSettings.ModifiedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, customSettings.ModifiedBy);
			}
			if (!success && customSettings.EditedFuzzy)
			{
				if (FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target))
				{
					success = FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
				}
				else
				{
					return false;
				}
			}
			if (!success && customSettings.UnEditedFuzzy)
			{
				if (FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target))
				{
					success = !FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
				}
				else
				{
					return false;
				}
			}
			//String id seach
			if (!success && !string.IsNullOrEmpty(customSettings.ContextInfoStringId))
			{
				if (settings.IsRegularExpression)
				{
					success = StringIdRegexSearch(rowInfo, customSettings.ContextInfoStringId, settings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
				}
				else
				{
					foreach (var contextInfo in rowInfo.ContextInfo)
					{
						if (contextInfo?.DisplayName == customSettings.ContextInfoStringId)
						{
							success = true;
						}
					}
				}
			}
			return !success;
		}
	}
}
