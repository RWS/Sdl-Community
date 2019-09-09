using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Models;
using Sdl.Community.Toolkit.Integration;
using Sdl.Community.Toolkit.Integration.DisplayFilter;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters
{
	public class DisplayFilter : IDisplayFilter
	{
		#region  |  Public  |

		/// <summary>
		/// Display filter settings
		/// </summary>
		public DisplayFilterSettings Settings { get; }

		public CustomFilterSettings CustomSettings { get; }

		#endregion

		#region  |  Private  |

		private Document ActiveDocument { get; }
		private bool ReverseSearch { get; }

		#endregion

		#region  |  Constructor  |

		public DisplayFilter(DisplayFilterSettings settings, CustomFilterSettings customSettings, bool reverseSearch,
			Document document)
		{
			ActiveDocument = document;

			Settings = settings;
			CustomSettings = customSettings;
			ReverseSearch = reverseSearch;

		}

		#endregion

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
						if (CustomSettings.SourceTargetFilterLogicalOperator == CustomFilterSettings.FilterLogicalOperators.Or)
						{
							var successSearchOnSource = IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source);

							var successSearchOnTarget = false;
							if (!successSearchOnSource)
							{
								successSearchOnTarget = IsExpressionFound(Settings.TargetText, rowInfo.SegmentPair.Target);
							}

							success = successSearchOnSource || successSearchOnTarget;
						}
						else
						{
							var appliedBackReferencesFilter = false;
							if (CustomSettings.UseBackReferences && Settings.IsRegularExpression)
							{
								var foundSourceBackReferencesInTarget = FoundBackReferences(Settings.SourceText, Settings.TargetText);
								var foundTargetBackReferencesInSource = FoundBackReferences(Settings.TargetText, Settings.SourceText);

								if (foundSourceBackReferencesInTarget || foundTargetBackReferencesInSource)
								{
									appliedBackReferencesFilter = true;
									// priority to using source back references in the target content
									if (foundSourceBackReferencesInTarget)
									{
										//match source regex then target regex


										List<CapturedGroup> capturedGroups;
										success = IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source, out capturedGroups);
										if (success)
										{
											//TODO
										}
									}
									else
									{
										//TODO
										//match source regex then target regex


									}
								}								
							}

							if (!appliedBackReferencesFilter)
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

		private bool FilterOnSourceAndTarget(DisplayFilterRowInfo rowInfo, bool success)
		{			
			if (success && !string.IsNullOrEmpty(Settings.SourceText))
			{
				success = IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source);
			}

			if (success && !string.IsNullOrEmpty(Settings.TargetText))
			{
				success = IsExpressionFound(Settings.TargetText, rowInfo.SegmentPair.Target);
			}

			return success;
		}

		private bool FoundBackReferences(string regex1, string regex2)
		{
			var foundBackReferences = false;
			
			var regex = new Regex(regex1);
			var groupNumbers = regex.GetGroupNumbers();
			if (groupNumbers.Length > 1)
			{
				for (var i = 1; i < groupNumbers.Length; i++)
				{
					foundBackReferences = regex2.Contains("$" + i);
					if (foundBackReferences)
					{
						break;
					}
				}
			}

			return foundBackReferences;
		}

		private bool IsExpressionFound(string searchString, ISegment segment, out List<CapturedGroup> capturedGroups)
		{
			capturedGroups = new List<CapturedGroup>();
			var text = GetSegmentText(segment);

			var regexOptions = Settings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
			var textSearchOptions = Settings.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			var success = false;
			if (Settings.IsRegularExpression)
			{
				var match = ContentHelper.SearchContentRegularExpression(text, searchString, regexOptions);
				if (match != null)
				{
					foreach (Group matchGroup in match.Groups)
					{
						
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

		private bool IsExpressionFound(string searchString, ISegment segment)
		{
			var text = GetSegmentText(segment);

			var regexOptions = Settings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
			var textSearchOptions = Settings.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			bool success;
			if (Settings.IsRegularExpression)
			{
				var match = ContentHelper.SearchContentRegularExpression(text, searchString, regexOptions);				
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
