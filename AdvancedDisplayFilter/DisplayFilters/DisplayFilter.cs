using System;
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
	    public string FileType { get; set; }


        #endregion
        #region  |  Private  |

        private Document ActiveDocument { get; set; }

        #endregion
        #region  |  Constructor  |
        public DisplayFilter(DisplayFilterSettings settings, CustomFilterSettings customSettings,Document document)
        {
            ActiveDocument = document;

            Settings = settings;
	        CustomSettings = customSettings;
	        if (document.ActiveFileProperties != null)
	        {
				FileType = document.ActiveFileProperties.FileConversionProperties.FileTypeDefinitionId.Id;
			}
	        
        }

        #endregion

        
        public bool EvaluateRow(DisplayFilterRowInfo rowInfo)
        {
            var success = !(!Settings.ShowAllContent && !rowInfo.IsSegment);

	        if (rowInfo.IsSegment)
	        {

		        if (success && Settings.SegmentReviewTypes != null && Settings.SegmentReviewTypes.Any())
			        success = rowInfo.IsSegmentReviewTypes(Settings);


		        if (success && Settings.ConfirmationLevels != null && Settings.ConfirmationLevels.Any())
			        success = rowInfo.IsConfirmationLevelFound(Settings);


		        if (success && Settings.OriginTypes != null && Settings.OriginTypes.Any())
			        success = rowInfo.IsOriginTypeFound(Settings);


		        if (success && Settings.PreviousOriginTypes != null && Settings.PreviousOriginTypes.Any())
			        success = rowInfo.IsPreviousOriginTypeFound(Settings);


		        if (success && Settings.RepetitionTypes != null && Settings.RepetitionTypes.Any())
			        success = rowInfo.IsRepetitionTypes(Settings);


		        if (success && Settings.SegmentLockingTypes != null && Settings.SegmentLockingTypes.Any())
			        success = rowInfo.IsSegmentLockingTypes(Settings);


		        if (success && Settings.SegmentContentTypes != null && Settings.SegmentContentTypes.Any())
			        success = rowInfo.IsSegmentContentTypes(Settings);

				// if is revert search use custom helper method 
		        if (success && !CustomSettings.RevertSerach)
		        {
					if (success && Settings.SourceText.Trim() != string.Empty)
				        success = rowInfo.IsTextFoundInSource(Settings);


			        if (success && Settings.TargetText.Trim() != string.Empty)
				        success = rowInfo.IsTextFoundInTarget(Settings);

				}

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
				//revert search
		        if (success && CustomSettings.RevertSerach && Settings.SourceText.Trim() != string.Empty)
		        {
			        success = ContentHelper.ReverseSearch(rowInfo.SegmentPair.Source.GetString(),Settings.SourceText.Trim());

		        }
		        if (success && CustomSettings.RevertSerach && Settings.TargetText.Trim() != string.Empty)
		        {
			        success = ContentHelper.ReverseSearch(rowInfo.SegmentPair.Target.GetString(), Settings.TargetText.Trim());

		        }
		        if (success && CustomSettings.Colors.Count>0)
		        {
			        if (FileType != null)
			        {
						if (FileType.Contains("IDML"))
						{
							success = ColorPickerHelper.ContainsColorForIdmlFileType(rowInfo, CustomSettings.Colors);
						}
						else
						{
							success = ColorPickerHelper.ContainsColor(rowInfo, CustomSettings.Colors);
						}
					}
			     
		        }

				//fuzzy
		        if (success && !string.IsNullOrWhiteSpace(CustomSettings.FuzzyMin) &&
		            !string.IsNullOrWhiteSpace(CustomSettings.FuzzyMax))
		        {
			        success = FuzzyHelper.IsInFuzzyRange(rowInfo, CustomSettings.FuzzyMin, CustomSettings.FuzzyMax);

		        }

			}

	        return success;
        }

    }
}
