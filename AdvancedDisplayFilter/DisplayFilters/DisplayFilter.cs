using System.Linq;
using Sdl.Community.AdvancedDisplayFilter.Extensions;
using Sdl.Community.AdvancedDisplayFilter.Services;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.DisplayFilters
{
	public class DisplayFilter : IDisplayFilter
	{
		private readonly QualitySamplingService _qualitySamplingService;
		private readonly bool _reverseSearch;
		private readonly ContentMatchingService _contentMatchingService;
		private readonly CustomFilterService _customFilterService;

		public DisplayFilter(DisplayFilterSettings settings, CustomFilterSettings customSettings, bool reverseSearch,
			QualitySamplingService qualitySamplingService, ContentMatchingService contentMatchingService,
			CustomFilterService customFilterService)
		{
			Settings = settings;
			CustomSettings = customSettings;

			_reverseSearch = reverseSearch;
			_qualitySamplingService = qualitySamplingService;
			_contentMatchingService = contentMatchingService;
			_customFilterService = customFilterService;
		}

		/// <summary>
		/// Display filter settings
		/// </summary>
		public DisplayFilterSettings Settings { get; }

		public CustomFilterSettings CustomSettings { get; }

		public bool EvaluateRow(DisplayFilterRowInfo rowInfo)
		{
			var success = Settings.ShowAllContent || rowInfo.IsSegment;

			if (rowInfo.IsSegment)
			{
				if (_reverseSearch)
				{
					return _customFilterService.Reverse(rowInfo);
				}

				if (CustomSettings.QualitySamplingSegmentsIds != null)
				{
					var segmentPairId = _qualitySamplingService.GetSegmentPairId(rowInfo.SegmentPair);
					if (!CustomSettings.QualitySamplingSegmentsIds.Contains(segmentPairId))
					{
						return false;
					}
				}


				if (success)
				{
					success = _customFilterService.FilterAttributeSuccess(rowInfo);
				}

				if (success && (!string.IsNullOrEmpty(Settings.SourceText) || !string.IsNullOrEmpty(Settings.TargetText)))
				{
					if (!string.IsNullOrEmpty(Settings.SourceText) && !string.IsNullOrEmpty(Settings.TargetText))
					{
						if (CustomSettings.SourceTargetLogicalOperator == DisplayFilterSettings.LogicalOperators.OR)
						{
							var successSearchOnSource = _contentMatchingService.IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source, out var _);

							var successSearchOnTarget = false;
							if (!successSearchOnSource)
							{
								successSearchOnTarget = _contentMatchingService.IsExpressionFound(Settings.TargetText, rowInfo.SegmentPair.Target, out var _);
							}

							success = successSearchOnSource || successSearchOnTarget;
						}
						else
						{
							var appliedFilter = false;
							if (CustomSettings.UseBackreferences && Settings.IsRegularExpression)
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

				if (success && !CustomSettings.UseRegexCommentSearch && !string.IsNullOrEmpty(Settings.CommentText))
				{
					success = rowInfo.IsTextFoundInComment(Settings);
				}

				if (success && !string.IsNullOrEmpty(Settings.CommentAuthor))
				{
					success = rowInfo.IsAuthorFoundInComment(Settings);
				}

				if (success && Settings.CommentSeverity > 0)
				{
					success = rowInfo.IsSeverityFoundInComment(Settings);
				}

				if (success && Settings.ContextInfoTypes.Any())
				{
					success = rowInfo.IsContextInfoTypes(Settings);
				}

				// check custom settings
				if (success)
				{
					success = _customFilterService.Filter(rowInfo, true);
				}
			}
			return success;
		}	
	}
}
