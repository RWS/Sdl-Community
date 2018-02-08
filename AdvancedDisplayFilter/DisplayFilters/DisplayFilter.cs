using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers;
using Sdl.Community.Toolkit.FileType;
using Sdl.Community.Toolkit.Integration;
using Sdl.Community.Toolkit.Integration.DisplayFilter;
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
			var success = !(!Settings.ShowAllContent && !rowInfo.IsSegment);

			if (rowInfo.IsSegment)
			{
				if (ReverseSearch)
				{
					return Reverse(success, rowInfo);
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


				if (success && Settings.SourceText.Trim() != string.Empty)
				{
					success = rowInfo.IsTextFoundInSource(Settings);
				
					if (Settings.IsRegularExpression)
					{
						var textVisitor = new SegmentTextVisitor();
						var text = textVisitor.GetText(rowInfo.SegmentPair.Source);
						success = ContentHelper.SearchContentRegularExpression(text,
							Settings.SourceText);
					}
				}

				if (success && Settings.TargetText.Trim() != string.Empty)
					success = rowInfo.IsTextFoundInTarget(Settings);


				if (success && !CustomSettings.UseRegexCommentSearch && Settings.CommentText.Trim() != string.Empty)
					success = rowInfo.IsTextFoundInComment(Settings);


				if (success && Settings.CommentAuthor.Trim() != string.Empty)
					success = rowInfo.IsAuthorFoundInComment(Settings);


				if (success && Settings.CommentSeverity > 0)
					success = rowInfo.IsSeverityFoundInComment(Settings);


				if (success && Settings.ContextInfoTypes.Any())
					success = rowInfo.IsContextInfoTypes(Settings);

				// check custom settings
				var rowId = rowInfo.SegmentPair.Properties.Id.Id;
				if (success && CustomSettings.EvenNo)
				{
					success = SegmentNumbersHelper.IsEven(rowId);
				}
				if (success && CustomSettings.OddsNo)
				{
					success = SegmentNumbersHelper.IsOdd(rowId);
				}
				if (success && CustomSettings.SplitSegments)
				{
					success = SegmentNumbersHelper.IsSplitSegment(rowId, ActiveDocument);
				}
				if (success && (CustomSettings.MergedSegments||CustomSettings.MergedAcross))
				{
					success = SegmentNumbersHelper.IsMergedSegment(rowId, ActiveDocument,CustomSettings.MergedAcross);
				}
				
				if (success && CustomSettings.SourceEqualsTarget)
				{
					success = SegmentNumbersHelper.IsSourceEqualsToTarget(rowInfo.SegmentPair, CustomSettings.IsEqualsCaseSensitive);
				}
				if (success && CustomSettings.Grouped && !string.IsNullOrWhiteSpace(CustomSettings.GroupedList))
				{
					success = SegmentNumbersHelper.IdInRange(rowId, CustomSettings.GroupedList);
				}
				if (success && CustomSettings.UseRegexCommentSearch &&
				    !string.IsNullOrWhiteSpace(CustomSettings.CommentRegex))
				{
					//create a list with source and target comments
					var commentsList = rowInfo.SegmentPair.Source.GetComments();
					commentsList.AddRange(rowInfo.SegmentPair.Target.GetComments());

					success = CommentsHelper.IsCommentTextFoundWithRegex(commentsList, CustomSettings.CommentRegex);
				}
				if (success && CustomSettings.Colors.Count > 0)
				{
					try
					{
						success = ColorPickerHelper.ContainsColor(rowInfo, CustomSettings.Colors);
					}catch(Exception e) { }
				}

				//fuzzy
				if (success && !string.IsNullOrWhiteSpace(CustomSettings.FuzzyMin) &&
				    !string.IsNullOrWhiteSpace(CustomSettings.FuzzyMax))
				{
					success = FuzzyHelper.IsInFuzzyRange(rowInfo, CustomSettings.FuzzyMin, CustomSettings.FuzzyMax);

				}
				if (success && CustomSettings.ContainsTags)
				{
					var containsTagVisitor = new TagVisitor();
					success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
				}
				//unique 
				if (success && CustomSettings.Unique)
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

					var isRepeted = rowInfo.SegmentPair.Properties.TranslationOrigin.IsRepeated;

					if (!isRepeted)
					{
						return true;
					}

					return false;
				}
				//created by
				if (success && CustomSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(CustomSettings.CreatedBy))
				{
					var userVisitor = new UserVisitor();
					success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, CustomSettings.CreatedBy);
				}
				//modify by
				if (success && CustomSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(CustomSettings.ModifiedBy))
				{
					var userVisitor = new UserVisitor();
					success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, CustomSettings.ModifiedBy);
				}
				if (success && CustomSettings.EditedFuzzy)
				{
					var editedFuzzyVisitor = new EditedFuzzyVisitor();
					success = editedFuzzyVisitor.IsEditedFuzzy(rowInfo.SegmentPair.Target);
				}
			}
			return success;
		}




		private bool Reverse(bool success, DisplayFilterRowInfo rowInfo)
		{
			success = false;
			if (!success && Settings.SegmentReviewTypes != null && Settings.SegmentReviewTypes.Any())
				success = rowInfo.IsSegmentReviewTypes(Settings);


			if (!success && Settings.ConfirmationLevels != null && Settings.ConfirmationLevels.Any())
				success = rowInfo.IsConfirmationLevelFound(Settings);


			if (!success && Settings.OriginTypes != null && Settings.OriginTypes.Any())
				success = rowInfo.IsOriginTypeFound(Settings);


			if (!success && Settings.PreviousOriginTypes != null && Settings.PreviousOriginTypes.Any())
				success = rowInfo.IsPreviousOriginTypeFound(Settings);


			if (!success && Settings.RepetitionTypes != null && Settings.RepetitionTypes.Any())
				success = rowInfo.IsRepetitionTypes(Settings);


			if (!success && Settings.SegmentLockingTypes != null && Settings.SegmentLockingTypes.Any())
				success = rowInfo.IsSegmentLockingTypes(Settings);


			if (!success && Settings.SegmentContentTypes != null && Settings.SegmentContentTypes.Any())
				success = rowInfo.IsSegmentContentTypes(Settings);


			if (!success && Settings.SourceText.Trim() != string.Empty)
				success = rowInfo.IsTextFoundInSource(Settings);


			if (!success && Settings.TargetText.Trim() != string.Empty)
				success = rowInfo.IsTextFoundInTarget(Settings);


			if (!success && !CustomSettings.UseRegexCommentSearch && Settings.CommentText.Trim() != string.Empty)
				success = rowInfo.IsTextFoundInComment(Settings);


			if (!success && Settings.CommentAuthor.Trim() != string.Empty)
				success = rowInfo.IsAuthorFoundInComment(Settings);


			if (!success && Settings.CommentSeverity > 0)
				success = rowInfo.IsSeverityFoundInComment(Settings);


			if (!success && Settings.ContextInfoTypes.Any())
				success = rowInfo.IsContextInfoTypes(Settings);

			// check custom settings
			var rowId = rowInfo.SegmentPair.Properties.Id.Id;
			if (!success && CustomSettings.EvenNo)
			{
				success = SegmentNumbersHelper.IsEven(rowId);
			}
			if (!success && CustomSettings.OddsNo)
			{
				success = SegmentNumbersHelper.IsOdd(rowId);
			}
			if (!success && CustomSettings.SplitSegments)
			{
				success = SegmentNumbersHelper.IsSplitSegment(rowId, ActiveDocument);
			}
			if (!success && (CustomSettings.MergedSegments||CustomSettings.MergedAcross))
			{
				success = SegmentNumbersHelper.IsMergedSegment(rowId, ActiveDocument,CustomSettings.MergedAcross);
			}
			if (!success && CustomSettings.SourceEqualsTarget)
			{
				success = SegmentNumbersHelper.IsSourceEqualsToTarget(rowInfo.SegmentPair, CustomSettings.IsEqualsCaseSensitive);
			}
			if (!success && CustomSettings.Grouped && !string.IsNullOrWhiteSpace(CustomSettings.GroupedList))
			{
				success = SegmentNumbersHelper.IdInRange(rowId, CustomSettings.GroupedList);
			}
			if (!success && CustomSettings.UseRegexCommentSearch &&
			    !string.IsNullOrWhiteSpace(CustomSettings.CommentRegex))
			{
				//create a list with source and target comments
				var commentsList = rowInfo.SegmentPair.Source.GetComments();
				commentsList.AddRange(rowInfo.SegmentPair.Target.GetComments());

				success = CommentsHelper.IsCommentTextFoundWithRegex(commentsList, CustomSettings.CommentRegex);
			}
			if (!success && CustomSettings.Colors.Count > 0)
			{
				try
				{
					success = ColorPickerHelper.ContainsColor(rowInfo, CustomSettings.Colors);
				}catch(Exception e) { }
			}

			//fuzzy
			if (!success && !string.IsNullOrWhiteSpace(CustomSettings.FuzzyMin) &&
			    !string.IsNullOrWhiteSpace(CustomSettings.FuzzyMax))
			{
				success = FuzzyHelper.IsInFuzzyRange(rowInfo, CustomSettings.FuzzyMin, CustomSettings.FuzzyMax);

			}
			//tags
			if (!success && CustomSettings.ContainsTags)
			{
				var containsTagVisitor = new TagVisitor();
				success = containsTagVisitor.ContainsTag(rowInfo.SegmentPair.Source);
			}
			if (!success && CustomSettings.CreatedByChecked && !string.IsNullOrWhiteSpace(CustomSettings.CreatedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.CreatedBy(rowInfo.SegmentPair.Source, CustomSettings.CreatedBy);
			}
			//modify by
			if (!success && CustomSettings.ModifiedByChecked && !string.IsNullOrWhiteSpace(CustomSettings.ModifiedBy))
			{
				var userVisitor = new UserVisitor();
				success = userVisitor.ModifiedBy(rowInfo.SegmentPair.Source, CustomSettings.ModifiedBy);
			}
			return !success;
		}
	}
}
