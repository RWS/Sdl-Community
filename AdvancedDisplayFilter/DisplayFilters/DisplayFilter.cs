using System.Linq;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;
using Sdl.Community.Toolkit.Integration;
using Sdl.Community.Toolkit.Integration.DisplayFilter;

namespace Sdl.Community.AdvancedDisplayFilter.DisplayFilters
{
    public class DisplayFilter : IDisplayFilter
    {
        #region  |  Public  |

        /// <summary>
        /// Display filter settings
        /// </summary>
        public DisplayFilterSettings Settings { get; private set; }


        #endregion
        #region  |  Private  |

        private Document ActiveDocument { get; set; }

        #endregion
        #region  |  Constructor  |
        public DisplayFilter(DisplayFilterSettings settings, Document document)
        {
            ActiveDocument = document;
            Settings = settings;
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


                if (success && Settings.SourceText.Trim() != string.Empty)
                    success = rowInfo.IsTextFoundInSource(Settings);


                if (success && Settings.TargetText.Trim() != string.Empty)
                    success = rowInfo.IsTextFoundInTarget(Settings);


                if (success && Settings.CommentText.Trim() != string.Empty)
                    success = rowInfo.IsTextFoundInComment(Settings);


                if (success && Settings.CommentAuthor.Trim() != string.Empty)
                    success = rowInfo. IsAuthorFoundInComment(Settings);


                if (success && Settings.CommentSeverity > 0)
                    success = rowInfo.IsSeverityFoundInComment(Settings);

                
                if (success && Settings.ContextInfoTypes.Any())
                    success = rowInfo.IsContextInfoTypes(Settings);
            }

            return success;
        }

    }
}
