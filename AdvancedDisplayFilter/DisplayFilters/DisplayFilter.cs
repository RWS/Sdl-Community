using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Extensions;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters
{
	public class DisplayFilter : IDisplayFilter
	{	
		private Document ActiveDocument { get; }

		private bool ReverseSearch { get; }
	
		public DisplayFilter(DisplayFilterSettings settings, CustomFilterSettings customSettings, bool reverseSearch, Document document)
		{
			ActiveDocument = document;

			Settings = settings;
			CustomSettings = customSettings;
			ReverseSearch = reverseSearch;

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
				if (ReverseSearch)
				{
					return CustomFilterHelper.Reverse(Settings, success, rowInfo, CustomSettings, ActiveDocument);
				}

				if (success && Settings.SegmentReviewTypes != null && Settings.SegmentReviewTypes.Any())
				{
					success = rowInfo.IsSegmentReviewTypes(Settings);
				}

				if (success && Settings.ConfirmationLevels != null && Settings.ConfirmationLevels.Any())
				{
					success = rowInfo.IsConfirmationLevelFound(Settings);
				}

				if (success && Settings.OriginTypes != null && Settings.OriginTypes.Any())
				{
					if (!Settings.OriginTypes.Contains("EditedF") && !Settings.OriginTypes.Contains("UneditedF"))
					{
						success = rowInfo.IsOriginTypeFound(Settings);
					}
				}

				if (success && Settings.PreviousOriginTypes != null && Settings.PreviousOriginTypes.Any())
				{
					success = rowInfo.IsPreviousOriginTypeFound(Settings);
				}

				if (success && Settings.RepetitionTypes != null && Settings.RepetitionTypes.Any())
				{
					if (!Settings.RepetitionTypes.Contains("Unique"))
					{
						success = rowInfo.IsRepetitionTypes(Settings);
					}
				}

				if (success && Settings.SegmentLockingTypes != null && Settings.SegmentLockingTypes.Any())
				{
					success = rowInfo.IsSegmentLockingTypes(Settings);
				}

				if (success && Settings.SegmentContentTypes != null && Settings.SegmentContentTypes.Any())
				{
					success = rowInfo.IsSegmentContentTypes(Settings);
				}

				if (success && (!string.IsNullOrEmpty(Settings.SourceText) || !string.IsNullOrEmpty(Settings.TargetText)))
				{
					if (!string.IsNullOrEmpty(Settings.SourceText) && !string.IsNullOrEmpty(Settings.TargetText))
					{
						if (CustomSettings.SourceAndTargetLogicalOperator == CustomFilterSettings.LogicalOperators.Or)
						{
							var successSearchOnSource = IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source, out var _);

							var successSearchOnTarget = false;
							if (!successSearchOnSource)
							{
								successSearchOnTarget = IsExpressionFound(Settings.TargetText, rowInfo.SegmentPair.Target, out var _);
							}

							success = successSearchOnSource || successSearchOnTarget;
						}
						else
						{
							var appliedFilter = false;
							if (CustomSettings.UseBackreferences && Settings.IsRegularExpression)
							{
								success = FilterOnSourceAndTargetWithBackreferences(rowInfo, out appliedFilter);
							}

							if (!appliedFilter)
							{
								success = FilterOnSourceAndTarget(rowInfo, true);
							}
						}
					}
					else
					{
						success = FilterOnSourceAndTarget(rowInfo, true);
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
					success = CustomFilterHelper.Filter(CustomSettings, Settings, rowInfo, true, ActiveDocument);
				}
			}
			return success;
		}

		private bool FilterOnSourceAndTargetWithBackreferences(DisplayFilterRowInfo rowInfo, out bool appliedFilter)
		{
			var success = true;
			appliedFilter = false;

			var backReferencesInTarget = GetBackReferences(Settings.SourceText, Settings.TargetText);
			var backReferencesInSource = GetBackReferences(Settings.TargetText, Settings.SourceText);

			if (backReferencesInTarget.Count > 0 || backReferencesInSource.Count > 0)
			{
				appliedFilter = true;

				// priority to using source back references in the target content
				if (backReferencesInTarget.Count > 0)
				{
					//match source regex then match backreferences in the taret
					success = IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source, out var capturedGroups);
					if (success)
					{
						success = IsExpressionFoundWithBackreferences(Settings.TargetText, rowInfo.SegmentPair.Target, capturedGroups,
							backReferencesInTarget);
					}
				}
				else
				{
					//match target regex then match backreferences in the source
					success = IsExpressionFound(Settings.TargetText, rowInfo.SegmentPair.Target, out var capturedGroups);
					if (success)
					{
						success = IsExpressionFoundWithBackreferences(Settings.SourceText, rowInfo.SegmentPair.Source, capturedGroups,
							backReferencesInSource);
					}
				}
			}

			return success;
		}

		private bool IsExpressionFoundWithBackreferences(string searchText, ISegment segment, IEnumerable<CapturedGroup> capturedGroups,
			IReadOnlyCollection<BackReference> backReferences)
		{
			foreach (var capturedGroup in capturedGroups)
			{
				var backReference = backReferences.FirstOrDefault(a => a.Number == capturedGroup.BackReference.Number);
				if (backReference != null)
				{
					searchText = backReference.IsNamed
						? searchText.Replace("${" + backReference.Name + "}", capturedGroup.Value)
						: searchText.Replace("$" + backReference.Number, capturedGroup.Value);
				}
			}

			var success = IsExpressionFound(searchText, segment, out var _);
			return success;
		}

		private bool FilterOnSourceAndTarget(DisplayFilterRowInfo rowInfo, bool success)
		{
			if (success && !string.IsNullOrEmpty(Settings.SourceText))
			{
				success = IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source, out var _);
			}

			if (success && !string.IsNullOrEmpty(Settings.TargetText))
			{
				success = IsExpressionFound(Settings.TargetText, rowInfo.SegmentPair.Target, out var _);
			}

			return success;
		}

		private static List<BackReference> GetBackReferences(string fromExpression, string inExpression)
		{
			var backReferences = new List<BackReference>();
			var regex = new Regex(fromExpression);
			var groupNumbers = regex.GetGroupNumbers();

			if (groupNumbers.Length > 1)
			{
				for (var i = 1; i < groupNumbers.Length; i++)
				{
					var number = groupNumbers[i];
					var name = regex.GroupNameFromNumber(number);

					if (inExpression.Contains("$" + number))
					{
						backReferences.Add(new BackReference(number, name));
					}
					else if (inExpression.Contains("${" + name + "}"))
					{
						backReferences.Add(new BackReference(number, name, true));
					}
				}
			}

			return backReferences;
		}

		private bool IsExpressionFound(string searchString, ISegment segment, out List<CapturedGroup> capturedGroups)
		{
			capturedGroups = new List<CapturedGroup>();
			var text = GetSegmentText(segment);

			var regexOptions = Settings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
			var textSearchOptions = Settings.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			bool success;
			if (Settings.IsRegularExpression)
			{
				var match = ContentHelper.SearchContentRegularExpression(text, searchString, regexOptions, out var regex);
				if (match != null)
				{
					foreach (Group group in match.Groups)
					{
						var capturedGroup = new CapturedGroup
						{
							Value = group.Value,
							Index = group.Index,
							BackReference = new BackReference(regex.GroupNumberFromName(group.Name), group.Name)
						};

						capturedGroups.Add(capturedGroup);
					}
				}

				success = match?.Success ?? false;
			}
			else
			{
				success = text.IndexOf(searchString, textSearchOptions) > -1;
			}

			return success;
		}

		private string GetSegmentText(ISegment segment)
		{
			var textVisitor = new SegmentTextVisitor();
			string text;
			if (CustomSettings.UseTagContent)
			{
				text = CustomSettings.AndOrTagContent
					? textVisitor.GetRawText(segment)
					: textVisitor.GetJustTagContent(segment);
			}
			else
			{
				text = textVisitor.GetText(segment);
			}

			return text;
		}
	}
}
