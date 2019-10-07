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
			if (success && _customSettings.Unique)
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
			if (success && _customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(_customSettings.CreatedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, _customSettings.CreatedBy);
			}

			//modify by
			if (success && _customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(_customSettings.ModifiedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, _customSettings.ModifiedBy);
			}

			if (success && _customSettings.EditedFuzzy)
			{
				success = FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target) &&
						  FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			}

			if (success && _customSettings.UnEditedFuzzy)
			{
				success = FuzzyHelper.ContainsFuzzy(rowInfo.SegmentPair.Target) &&
						  !FuzzyHelper.IsEditedFuzzy(rowInfo.SegmentPair.Target);
			}

			// Document Structure Info Location search
			if (success && !string.IsNullOrEmpty(_customSettings.DocumentStructureInfoLocation))
			{
				if (_settings.IsRegularExpression)
				{
					success = DocumentStructureInfoLocationRegexSearch(rowInfo, _customSettings.DocumentStructureInfoLocation, 
						_settings.IsCaseSensitive 
							? RegexOptions.None 
							: RegexOptions.IgnoreCase);
				}
				else
				{
					foreach (var contextInfo in rowInfo.ContextInfo)
					{
						if (contextInfo?.DisplayName == _customSettings.DocumentStructureInfoLocation)
						{
							return true;
						}
					}

					return false;
				}
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

			if (!success && _settings.SegmentReviewTypes != null && _settings.SegmentReviewTypes.Any())
			{
				success = rowInfo.IsSegmentReviewTypes(_settings);
			}


			if (!success && _settings.ConfirmationLevels != null && _settings.ConfirmationLevels.Any())
			{
				success = rowInfo.IsConfirmationLevelFound(_settings);
			}

			if (!success && _settings.OriginTypes != null && _settings.OriginTypes.Any())
				success = rowInfo.IsOriginTypeFound(_settings);


			if (!success && _settings.PreviousOriginTypes != null && _settings.PreviousOriginTypes.Any())
			{
				success = rowInfo.IsPreviousOriginTypeFound(_settings);
			}

			if (!success && _settings.RepetitionTypes != null && _settings.RepetitionTypes.Any())
			{
				success = rowInfo.IsRepetitionTypes(_settings);
			}

			if (!success && _settings.SegmentLockingTypes != null && _settings.SegmentLockingTypes.Any())
			{
				success = rowInfo.IsSegmentLockingTypes(_settings);
			}

			if (!success && _settings.SegmentContentTypes != null && _settings.SegmentContentTypes.Any())
			{
				success = rowInfo.IsSegmentContentTypes(_settings);
			}

			if (!success && (!string.IsNullOrEmpty(_settings.SourceText) || !string.IsNullOrEmpty(_settings.TargetText)))
			{
				if (!string.IsNullOrEmpty(_settings.SourceText) && !string.IsNullOrEmpty(_settings.TargetText))
				{
					if (_customSettings.SourceAndTargetLogicalOperator == CustomFilterSettings.LogicalOperators.Or)
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

			//fuzzy
			if (!success && !string.IsNullOrWhiteSpace(_customSettings.FuzzyMin) &&
				!string.IsNullOrWhiteSpace(_customSettings.FuzzyMax))
			{
				success = FuzzyHelper.IsInFuzzyRange(rowInfo, _customSettings.FuzzyMin, _customSettings.FuzzyMax);

			}

			//tags
			if (!success && _customSettings.ContainsTags)
			{
				var containsTagVisitor = new TagVisitor();
				success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
			}

			if (!success && _customSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(_customSettings.CreatedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, _customSettings.CreatedBy);
			}

			//modify by
			if (!success && _customSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(_customSettings.ModifiedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, _customSettings.ModifiedBy);
			}

			if (!success && _customSettings.EditedFuzzy)
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

			if (!success && _customSettings.UnEditedFuzzy)
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
			if (!success && !string.IsNullOrEmpty(_customSettings.DocumentStructureInfoLocation))
			{
				if (_settings.IsRegularExpression)
				{
					success = DocumentStructureInfoLocationRegexSearch(rowInfo, _customSettings.DocumentStructureInfoLocation, _settings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
				}
				else
				{
					foreach (var contextInfo in rowInfo.ContextInfo)
					{
						if (contextInfo?.DisplayName == _customSettings.DocumentStructureInfoLocation)
						{
							success = true;
						}
					}
				}
			}

			return !success;
		}

		private static bool DocumentStructureInfoLocationRegexSearch(DisplayFilterRowInfo rowInfo, string regexExpression, RegexOptions options)
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
	}
}
