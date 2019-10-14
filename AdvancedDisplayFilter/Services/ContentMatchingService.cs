using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.Community.AdvancedDisplayFilter.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.Services
{
	public class ContentMatchingService
	{
		private readonly DisplayFilterSettings _settings;
		private readonly CustomFilterSettings _customSettings;

		public ContentMatchingService(DisplayFilterSettings settings, CustomFilterSettings customSettings)
		{
			_settings = settings;
			_customSettings = customSettings;
		}

		public bool IsExpressionFound(string searchString, ISegment segment, out List<CapturedGroup> capturedGroups)
		{
			capturedGroups = new List<CapturedGroup>();
			var text = GetSegmentText(segment);

			var regexOptions = _settings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
			var textSearchOptions = _settings.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			bool success;
			if (_settings.IsRegularExpression)
			{
				var match = SearchContentRegularExpression(text, searchString, regexOptions, out var regex);
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
	
		public bool FilterOnSourceAndTarget(DisplayFilterRowInfo rowInfo, bool success)
		{
			if (success && !string.IsNullOrEmpty(_settings.SourceText))
			{
				success = IsExpressionFound(_settings.SourceText, rowInfo.SegmentPair.Source, out var _);
			}

			if (success && !string.IsNullOrEmpty(_settings.TargetText))
			{
				success = IsExpressionFound(_settings.TargetText, rowInfo.SegmentPair.Target, out var _);
			}

			return success;
		}

		public bool FilterOnSourceAndTargetWithBackreferences(DisplayFilterRowInfo rowInfo, out bool appliedFilter)
		{
			var success = true;
			appliedFilter = false;

			var backReferencesInTarget = GetBackReferences(_settings.SourceText, _settings.TargetText);
			var backReferencesInSource = GetBackReferences(_settings.TargetText, _settings.SourceText);

			if (backReferencesInTarget.Count > 0 || backReferencesInSource.Count > 0)
			{
				appliedFilter = true;

				// priority to using source back references in the target content
				if (backReferencesInTarget.Count > 0)
				{
					//match source regex then match backreferences in the taret
					success = IsExpressionFound(_settings.SourceText, rowInfo.SegmentPair.Source, out var capturedGroups);
					if (success)
					{
						success = IsExpressionFoundWithBackreferences(_settings.TargetText, rowInfo.SegmentPair.Target, capturedGroups,
							backReferencesInTarget);
					}
				}
				else
				{
					//match target regex then match backreferences in the source
					success = IsExpressionFound(_settings.TargetText, rowInfo.SegmentPair.Target, out var capturedGroups);
					if (success)
					{
						success = IsExpressionFoundWithBackreferences(_settings.SourceText, rowInfo.SegmentPair.Source, capturedGroups,
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
		
		private static Match SearchContentRegularExpression(string text, string searchString, RegexOptions regexOptions, out Regex regex)
		{
			try
			{
				regex = new Regex(searchString, regexOptions);
				return regex.Match(text);
			}
			catch
			{
				// catch all; ignore
			}

			regex = null;
			return null;
		}

		private string GetSegmentText(ISegment segment)
		{
			var textVisitor = new SegmentTextVisitor();
			string text;
			if (_customSettings.SearchInTagContent)
			{
				text = _customSettings.SearchInTagContentAndText
					? textVisitor.GetText(segment, SegmentTextVisitor.DetailLevel.Raw)
					: textVisitor.GetText(segment, SegmentTextVisitor.DetailLevel.JustTagContent);
			}
			else
			{
				text = textVisitor.GetText(segment);
			}

			return text;
		}
	}
}
