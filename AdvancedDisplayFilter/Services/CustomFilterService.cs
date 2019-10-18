using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.AdvancedDisplayFilter.Extensions;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.Services
{
	public class CustomFilterService
	{
		private readonly DisplayFilterSettings _settings;
		private readonly CustomFilterSettings _customSettings;
		private readonly Document _document;

		public CustomFilterService(DisplayFilterSettings settings, CustomFilterSettings customSettings, Document document)
		{
			_settings = settings;
			_customSettings = customSettings;
			_document = document;
		}

		public bool Filter(DisplayFilterRowInfo rowInfo, bool success)
		{
			if (!rowInfo.IsSegment)
			{
				return !HasCustomSettings();
			}

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
				var visitor = new CommentDataVisitor();

				var commentsList = visitor.GetComments(rowInfo.SegmentPair.Source);
				commentsList.AddRange(visitor.GetComments(rowInfo.SegmentPair.Target));

				success = CommentsHelper.IsCommentTextFoundWithRegex(commentsList, _customSettings.CommentRegex);
			}

			if (success && _customSettings.Colors?.Count > 0)
			{
				success = ColorPickerHelper.ContainsColor(rowInfo, _customSettings.Colors, _customSettings.ColorsFoundIn);
			}
		
			if (success && !string.IsNullOrWhiteSpace(_customSettings.FuzzyMin) && !string.IsNullOrWhiteSpace(_customSettings.FuzzyMax))
			{
				success = FuzzyHelper.IsInFuzzyRange(rowInfo, _customSettings.FuzzyMin, _customSettings.FuzzyMax);
			}

			if (success && _customSettings.ContainsTags)
			{
				var containsTagVisitor = new TagVisitor();
				success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
			}
			
			if (success && _customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(_customSettings.CreatedBy))
			{
				var userVisitor = new TranslationOriginMetaDataVisitor();
				success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, _customSettings.CreatedBy);
			}
			
			if (success && _customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(_customSettings.ModifiedBy))
			{
				var userVisitor = new TranslationOriginMetaDataVisitor();
				success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, _customSettings.ModifiedBy);
			}

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

		public bool FilterAttributeSuccess(DisplayFilterRowInfo rowInfo, bool success)
		{
			if (GetAttributeFilterGroupsCount() == 0)
			{
				return success;
			}

			if (!rowInfo.IsSegment)
			{
				return false;
			}

			var isAndOperator = _customSettings.FilterAttributesLogicalOperator == DisplayFilterSettings.LogicalOperators.AND;
			success = isAndOperator;

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

		public int GetAttributeFilterGroupsCount()
		{
			var count = 0;
			if (_settings.SegmentReviewTypes != null && _settings.SegmentReviewTypes.Any())
			{
				count++;
			}

			if (_settings.ConfirmationLevels != null && _settings.ConfirmationLevels.Any())
			{
				count++;
			}

			if (_settings.OriginTypes != null && _settings.OriginTypes.Any())
			{
				count++;
			}

			if (_settings.PreviousOriginTypes != null && _settings.PreviousOriginTypes.Any())
			{
				count++;
			}

			if (_settings.RepetitionTypes != null && _settings.RepetitionTypes.Any())
			{
				count++;
			}

			if (_settings.SegmentLockingTypes != null && _settings.SegmentLockingTypes.Any())
			{
				count++;
			}

			if (_settings.SegmentContentTypes != null && _settings.SegmentContentTypes.Any())
			{
				count++;
			}

			return count;
		}

		public bool HasCustomSettings()
		{
			return _customSettings.EvenNo ||
				   _customSettings.OddsNo ||
				   _customSettings.SplitSegments ||
				   _customSettings.MergedSegments ||
				   _customSettings.MergedAcross ||
				   _customSettings.SourceEqualsTarget ||
				   (_customSettings.Grouped && !string.IsNullOrWhiteSpace(_customSettings.GroupedList)) ||
				   (_customSettings.UseRegexCommentSearch && !string.IsNullOrWhiteSpace(_customSettings.CommentRegex)) ||
				   _customSettings.Colors.Count > 0 ||
				   !string.IsNullOrWhiteSpace(_customSettings.FuzzyMin) &&
				   !string.IsNullOrWhiteSpace(_customSettings.FuzzyMax) ||
				   _customSettings.ContainsTags ||
				   (_customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(_customSettings.CreatedBy)) ||
				   (_customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(_customSettings.ModifiedBy)) ||
				   !string.IsNullOrEmpty(_customSettings.DocumentStructureInformation);
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
			if (!rowInfo.IsSegment)
			{
				return false;
			}

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
			if (!rowInfo.IsSegment)
			{
				return false;
			}

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
