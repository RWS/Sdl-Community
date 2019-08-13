using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers;
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
					success = rowInfo.IsSegmentReviewTypes(Settings);

				if (success && Settings.ConfirmationLevels != null && Settings.ConfirmationLevels.Any())
					success = rowInfo.IsConfirmationLevelFound(Settings);

				if (success && Settings.OriginTypes != null && Settings.OriginTypes.Any())
				{
					if (!Settings.OriginTypes.Contains("EditedF") && !Settings.OriginTypes.Contains("UneditedF"))
					{
						success = rowInfo.IsOriginTypeFound(Settings);
					}
				}

				if (success && Settings.PreviousOriginTypes != null && Settings.PreviousOriginTypes.Any())
					success = rowInfo.IsPreviousOriginTypeFound(Settings);

				if (success && Settings.RepetitionTypes != null && Settings.RepetitionTypes.Any())
				{
					if (!Settings.RepetitionTypes.Contains("Unique"))
					{
						success = rowInfo.IsRepetitionTypes(Settings);
					}
				}

				if (success && Settings.SegmentLockingTypes != null && Settings.SegmentLockingTypes.Any())
					success = rowInfo.IsSegmentLockingTypes(Settings);


				if (success && Settings.SegmentContentTypes != null && Settings.SegmentContentTypes.Any())
					success = rowInfo.IsSegmentContentTypes(Settings);


				if (success && !string.IsNullOrEmpty(Settings.SourceText))
				{
					success = IsExpressionFound(Settings.SourceText, rowInfo.SegmentPair.Source);
				}
				if (success && !string.IsNullOrEmpty(Settings.TargetText))
				{
					success = IsExpressionFound(Settings.TargetText, rowInfo.SegmentPair.Target);
				}

				if (success && !CustomSettings.UseRegexCommentSearch && !string.IsNullOrEmpty(Settings.CommentText))
					success = rowInfo.IsTextFoundInComment(Settings);


				if (success && !string.IsNullOrEmpty(Settings.CommentAuthor))
					success = rowInfo.IsAuthorFoundInComment(Settings);


				if (success && Settings.CommentSeverity > 0)
					success = rowInfo.IsSeverityFoundInComment(Settings);


				if (success && Settings.ContextInfoTypes.Any())
					success = rowInfo.IsContextInfoTypes(Settings);

				// check custom settings
				if (success)
				{
					success = CustomFilterHelper.Filter(CustomSettings, Settings, rowInfo, true, ActiveDocument);
				}
			}
			return success;
		}

		private bool IsExpressionFound(string searchString, ISegment segment)
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

			var regexOptions = Settings.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
			var textSearchOptions = Settings.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			var success = Settings.IsRegularExpression
				? ContentHelper.SearchContentRegularExpression(text, searchString, regexOptions)
				: text.IndexOf(searchString, textSearchOptions) > -1;

			return success;
		}
	}
}
