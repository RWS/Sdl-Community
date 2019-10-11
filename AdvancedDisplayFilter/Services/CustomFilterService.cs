using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.AdvancedDisplayFilter.Extensions;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.Community.Toolkit.FileType;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.Services
{
	public class CustomFilterService
	{
		
		private readonly DisplayFilterSettings _settings;
		private readonly CustomFilterSettings _customSettings;
		private readonly Document _document;
		private readonly QualitySamplingService _qualitySamplingService;
		private readonly ContentMatchingService _contentMatchingService;

		public CustomFilterService(DisplayFilterSettings settings, CustomFilterSettings customSettings, Document document,
			QualitySamplingService qualitySamplingService, ContentMatchingService contentMatchingService)
		{
			_settings = settings;
			_customSettings = customSettings;
			_document = document;
			_qualitySamplingService = qualitySamplingService;
			_contentMatchingService = contentMatchingService;
		}

		public bool Filter(DisplayFilterRowInfo rowInfo, bool success)
		{
			var rowId = rowInfo.SegmentPair.Properties.Id.Id;

			if (success && _customSettings.EvenNo)
			{
				success = SegmentNumbersHelper.IsEven(rowId);
			}

			if (success && _customSettings.OddsNo)
			{
				success = SegmentNumbersHelper.IsOdd(rowId);
			}

			if (success && _customSettings.SplitSegments)
			{
				success = SegmentNumbersHelper.IsSplitSegment(rowId, _document);
			}

			if (success && (_customSettings.MergedSegments || _customSettings.MergedAcross))
			{
				success = SegmentNumbersHelper.IsMergedSegment(rowId, _document, _customSettings.MergedAcross);
			}

			if (success && _customSettings.SourceEqualsTarget)
			{
				success = SegmentNumbersHelper.IsSourceEqualsToTarget(rowInfo.SegmentPair, _customSettings.IsEqualsCaseSensitive);
			}

			if (success && _customSettings.Grouped && !string.IsNullOrWhiteSpace(_customSettings.GroupedList))
			{
				success = SegmentNumbersHelper.IdInRange(rowId, _customSettings.GroupedList);
			}

			if (success && _customSettings.UseRegexCommentSearch && !string.IsNullOrWhiteSpace(_customSettings.CommentRegex))
			{
				//create a list with source and target comments
				var commentsList = rowInfo.SegmentPair.Source.GetComments();
				commentsList.AddRange(rowInfo.SegmentPair.Target.GetComments());

				success = CommentsHelper.IsCommentTextFoundWithRegex(commentsList, _customSettings.CommentRegex);
			}

			if (success && _customSettings.Colors.Count > 0)
			{
				try
				{
					success = ColorPickerHelper.ContainsColor(rowInfo, _customSettings.Colors);
				}
				catch
				{
					// catch all; ignore
				}
			}

			//fuzzy
			if (success && !string.IsNullOrWhiteSpace(_customSettings.FuzzyMin) && !string.IsNullOrWhiteSpace(_customSettings.FuzzyMax))
			{
				success = FuzzyHelper.IsInFuzzyRange(rowInfo, _customSettings.FuzzyMin, _customSettings.FuzzyMax);
			}

			if (success && _customSettings.ContainsTags)
			{
				var containsTagVisitor = new TagVisitor();
				success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
			}

			//unique 
			// TODO Unique
			//if (success && _customSettings.Unique)
			//{
			//	var settings = new DisplayFilterSettings
			//	{
			//		RepetitionTypes = new List<string>
			//		{
			//			"FirstOccurrences"
			//		}
			//	};

			//	var isFirst = rowInfo.IsRepetitionsFirstOccurrences(settings);
			//	if (isFirst)
			//	{
			//		return true;
			//	}

			//	if (rowInfo.SegmentPair.Properties.TranslationOrigin != null)
			//	{
			//		var isRepeated = rowInfo.SegmentPair.Properties.TranslationOrigin.IsRepeated;
			//		return !isRepeated;
			//	}

			//	return false;
			//}

			//created by
			if (success && _customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(_customSettings.CreatedBy))
			{
				var userVisitor = new TranslationOriginMetaDataVisitor();
				success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, _customSettings.CreatedBy);
			}

			//modify by
			if (success && _customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(_customSettings.ModifiedBy))
			{
				var userVisitor = new TranslationOriginMetaDataVisitor();
				success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, _customSettings.ModifiedBy);
			}

			//TODO IsEditedFuzzy
			//if (success && _customSettings.IsEditedFuzzy)
			//{
			//	success = FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target) &&
			//			  FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			//}

			//if (success && _customSettings.IsUnEditedFuzzy)
			//{
			//	success = FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target) &&
			//			  !FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			//}

			if (success && !string.IsNullOrEmpty(_customSettings.DocumentStructureInformation))
			{
				success = _settings.IsRegularExpression
					? DocumentStructureInfoRegexSearch(rowInfo, _customSettings.DocumentStructureInformation,
						_settings.IsCaseSensitive
							? RegexOptions.None
							: RegexOptions.IgnoreCase)
					: DocumentStructureInfoSearch(rowInfo, _customSettings);
			}

			return success;
		}

		public bool Reverse(DisplayFilterRowInfo rowInfo)
		{
			var success = false;

			if (_customSettings.QualitySamplingSegmentsIds != null)
			{
				var segmentPairId = _qualitySamplingService.GetSegmentPairId(rowInfo.SegmentPair);
				if (_customSettings.QualitySamplingSegmentsIds.Contains(segmentPairId))
				{
					return false;
				}
			}


			if (!success)
			{
				success = FilterAttributeSuccess(rowInfo);
			}

			if (!success && (!string.IsNullOrEmpty(_settings.SourceText) || !string.IsNullOrEmpty(_settings.TargetText)))
			{
				if (!string.IsNullOrEmpty(_settings.SourceText) && !string.IsNullOrEmpty(_settings.TargetText))
				{
					if (_customSettings.SourceTargetLogicalOperator == DisplayFilterSettings.LogicalOperators.OR)
					{
						var successSearchOnSource = _contentMatchingService.IsExpressionFound(_settings.SourceText, rowInfo.SegmentPair.Source, out var _);

						var successSearchOnTarget = false;
						if (!successSearchOnSource)
						{
							successSearchOnTarget = _contentMatchingService.IsExpressionFound(_settings.TargetText, rowInfo.SegmentPair.Target, out var _);
						}

						success = successSearchOnSource || successSearchOnTarget;
					}
					else
					{
						var appliedFilter = false;
						if (_customSettings.UseBackreferences && _settings.IsRegularExpression)
						{
							success = _contentMatchingService.FilterOnSourceAndTargetWithBackreferences(rowInfo, out appliedFilter);
						}

						if (!appliedFilter)
						{
							success = _contentMatchingService.FilterOnSourceAndTarget(rowInfo, true);
						}
					}
				}
				else
				{
					success = _contentMatchingService.FilterOnSourceAndTarget(rowInfo, true);
				}
			}

			if (!success && !_customSettings.UseRegexCommentSearch && !string.IsNullOrEmpty(_settings.CommentText))
			{
				success = rowInfo.IsTextFoundInComment(_settings);
			}

			if (!success && !string.IsNullOrEmpty(_settings.CommentAuthor))
			{
				success = rowInfo.IsAuthorFoundInComment(_settings);
			}

			if (!success && _settings.CommentSeverity > 0)
			{
				success = rowInfo.IsSeverityFoundInComment(_settings);
			}

			if (!success && _settings.ContextInfoTypes.Any())
			{
				success = rowInfo.IsContextInfoTypes(_settings);
			}

			// check custom settings
			var rowId = rowInfo.SegmentPair.Properties.Id.Id;
			if (!success && _customSettings.EvenNo)
			{
				success = SegmentNumbersHelper.IsEven(rowId);
			}

			if (!success && _customSettings.OddsNo)
			{
				success = SegmentNumbersHelper.IsOdd(rowId);
			}

			if (!success && _customSettings.SplitSegments)
			{
				success = SegmentNumbersHelper.IsSplitSegment(rowId, _document);
			}

			if (!success && (_customSettings.MergedSegments || _customSettings.MergedAcross))
			{
				success = SegmentNumbersHelper.IsMergedSegment(rowId, _document, _customSettings.MergedAcross);
			}

			if (!success && _customSettings.SourceEqualsTarget)
			{
				success = SegmentNumbersHelper.IsSourceEqualsToTarget(rowInfo.SegmentPair, _customSettings.IsEqualsCaseSensitive);
			}

			if (!success && _customSettings.Grouped && !string.IsNullOrWhiteSpace(_customSettings.GroupedList))
			{
				success = SegmentNumbersHelper.IdInRange(rowId, _customSettings.GroupedList);
			}

			if (!success && _customSettings.UseRegexCommentSearch && !string.IsNullOrWhiteSpace(_customSettings.CommentRegex))
			{
				//create a list with source and target comments
				var commentsList = rowInfo.SegmentPair.Source.GetComments();
				commentsList.AddRange(rowInfo.SegmentPair.Target.GetComments());

				success = CommentsHelper.IsCommentTextFoundWithRegex(commentsList, _customSettings.CommentRegex);
			}

			if (!success && _customSettings.Colors.Count > 0)
			{
				try
				{
					success = ColorPickerHelper.ContainsColor(rowInfo, _customSettings.Colors);
				}
				catch
				{
					// ignored
				}
			}

			// fuzzy
			if (!success && !string.IsNullOrWhiteSpace(_customSettings.FuzzyMin) &&
				!string.IsNullOrWhiteSpace(_customSettings.FuzzyMax))
			{
				success = FuzzyHelper.IsInFuzzyRange(rowInfo, _customSettings.FuzzyMin, _customSettings.FuzzyMax);

			}

			// tags
			if (!success && _customSettings.ContainsTags)
			{
				var containsTagVisitor = new TagVisitor();
				success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
			}

			// created by
			if (!success && _customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(_customSettings.CreatedBy))
			{
				var visitor = new TranslationOriginMetaDataVisitor();
				success = visitor.CreatedBy(rowInfo.SegmentPair.Source, _customSettings.CreatedBy);
			}

			// modified by
			if (!success && _customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(_customSettings.ModifiedBy))
			{
				var visitor = new TranslationOriginMetaDataVisitor();
				success = visitor.ModifiedBy(rowInfo.SegmentPair.Source, _customSettings.ModifiedBy);
			}

			//TODO IsEditedFuzzy
			//if (!success && _customSettings.IsEditedFuzzy)
			//{
			//	if (FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target))
			//	{
			//		success = FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			//	}
			//	else
			//	{
			//		return false;
			//	}
			//}

			//if (!success && _customSettings.IsUnEditedFuzzy)
			//{
			//	if (FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target))
			//	{
			//		success = !FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			//	}
			//	else
			//	{
			//		return false;
			//	}
			//}

			if (!success && !string.IsNullOrEmpty(_customSettings.DocumentStructureInformation))
			{
				success = _settings.IsRegularExpression
					? DocumentStructureInfoRegexSearch(rowInfo, _customSettings.DocumentStructureInformation, _settings.IsCaseSensitive
						? RegexOptions.None
						: RegexOptions.IgnoreCase)
					: DocumentStructureInfoSearch(rowInfo, _customSettings);
			}

			return !success;
		}

		public bool FilterAttributeSuccess(DisplayFilterRowInfo rowInfo)
		{
			var isAnd = _customSettings.FilterAttributesLogicalOperator == DisplayFilterSettings.LogicalOperators.AND;
			var success = isAnd;

			if (_settings.SegmentReviewTypes != null && _settings.SegmentReviewTypes.Any())
			{
				success = rowInfo.IsSegmentReviewTypes(_settings);
			}

			if (LogicalSuccess(success) && _settings.ConfirmationLevels != null && _settings.ConfirmationLevels.Any())
			{
				success = rowInfo.IsConfirmationLevelFound(_settings);
			}

			if (LogicalSuccess(success) && _settings.OriginTypes != null && _settings.OriginTypes.Any())
			{
				success = rowInfo.IsOriginTypeFound(_settings);
			}

			if (LogicalSuccess(success) && _settings.PreviousOriginTypes != null && _settings.PreviousOriginTypes.Any())
			{
				success = rowInfo.IsPreviousOriginTypeFound(_settings);
			}

			if (LogicalSuccess(success) && _settings.RepetitionTypes != null && _settings.RepetitionTypes.Any())
			{
				success = rowInfo.IsRepetitionTypes(_settings);
			}

			if (LogicalSuccess(success) && _settings.SegmentLockingTypes != null && _settings.SegmentLockingTypes.Any())
			{
				success = rowInfo.IsSegmentLockingTypes(_settings);
			}

			if (LogicalSuccess(success) && _settings.SegmentContentTypes != null && _settings.SegmentContentTypes.Any())
			{
				success = rowInfo.IsSegmentContentTypes(_settings);
			}
			
			return success;
		}

		private bool LogicalSuccess(bool success)
		{
			if (_customSettings.FilterAttributesLogicalOperator == DisplayFilterSettings.LogicalOperators.AND)
			{
				return success;
			}

			return !success;
		}
		private static bool DocumentStructureInfoSearch(DisplayFilterRowInfo rowInfo, CustomFilterSettings customSettings)
		{
			foreach (var contextInfo in rowInfo.ContextInfo)
			{
				if (contextInfo != null)
				{
					if (contextInfo.DisplayName?.IndexOf(customSettings.DocumentStructureInformation,
							StringComparison.InvariantCultureIgnoreCase) > -1)
					{
						return true;
					}

					if (contextInfo.DisplayCode?.IndexOf(customSettings.DocumentStructureInformation,
							StringComparison.InvariantCultureIgnoreCase) > -1)
					{
						return true;
					}

					if (contextInfo.Description?.IndexOf(customSettings.DocumentStructureInformation,
							StringComparison.InvariantCultureIgnoreCase) > -1)
					{
						return true;
					}
				}
			}

			return false;
		}

		private static bool DocumentStructureInfoRegexSearch(DisplayFilterRowInfo rowInfo, string regexExpression, RegexOptions options)
		{
			var regex = new Regex(regexExpression, options);
			foreach (var contextInfo in rowInfo.ContextInfo)
			{
				if (contextInfo != null)
				{
					if (regex.Match(contextInfo.DisplayName ?? string.Empty).Success)
					{
						return true;
					}

					if (regex.Match(contextInfo.DisplayCode ?? string.Empty).Success)
					{
						return true;
					}

					if (regex.Match(contextInfo.Description ?? string.Empty).Success)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
