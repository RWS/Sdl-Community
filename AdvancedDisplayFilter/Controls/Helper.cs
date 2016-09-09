using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.Plugins.AdvancedDisplayFilter;
using Sdl.Community.Toolkit.Integration.DisplayFilter;
using Sdl.Community.Toolkit.FileType;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
    public class Helper
    {
        public static string GetTypeName(DisplayFilterSettings.ConfirmationLevel type)
        {
            switch (type)
            {
                case DisplayFilterSettings.ConfirmationLevel.Unspecified: return StringResources.DisplayFilterControl_Not_Translated;
                case DisplayFilterSettings.ConfirmationLevel.Draft: return StringResources.DisplayFilterControl_Draft;
                case DisplayFilterSettings.ConfirmationLevel.Translated: return StringResources.DisplayFilterControl_Translated;
                case DisplayFilterSettings.ConfirmationLevel.RejectedTranslation: return StringResources.DisplayFilterControl_Translation_Rejected;
                case DisplayFilterSettings.ConfirmationLevel.ApprovedTranslation: return StringResources.DisplayFilterControl_Translation_Approved;
                case DisplayFilterSettings.ConfirmationLevel.RejectedSignOff: return StringResources.DisplayFilterControl_Sign_off_Rejected;
                case DisplayFilterSettings.ConfirmationLevel.ApprovedSignOff: return StringResources.DisplayFilterControl_Signed_Off;       
                default: return StringResources.DisplayFilterControl_Not_Translated;
            }
        }

        public static string GetTypeName(OriginType type)
        {
            switch (type)
            {
                case OriginType.Source:
                    return StringResources.DisplayFilterControl_Copied_from_source;
                case OriginType.AutoPropagated:
                    return StringResources.DisplayFilterControl_Auto_propagated;
                case OriginType.Exact:
                    return StringResources.DisplayFilterControl_Exact_matches;
                case OriginType.Fuzzy:
                    return StringResources.DisplayFilterControl_Fuzzy_matches;
                case OriginType.PM:
                    return StringResources.DisplayFilterControl_Perfect_matches;
                case OriginType.CM:
                    return StringResources.DisplayFilterControl_Context_matches;
                case OriginType.AT:
                    return StringResources.DisplayFilterControl_Automated_translations;
                default:
                    return type.ToString();
            }
        }

        public static string GetTypeName(DisplayFilterSettings.RepetitionType type)
        {
            switch (type)
            {
                case DisplayFilterSettings.RepetitionType.All:
                    return StringResources.DisplayFilterControl_All;
                case DisplayFilterSettings.RepetitionType.FirstOccurrences:
                    return StringResources.DisplayFilterControl_First_Occurrences;
                case DisplayFilterSettings.RepetitionType.ExcludeFirstOccurrences:
                    return StringResources.DisplayFilterControl_Exclude_first_occurrences;
                default:
                    return StringResources.DisplayFilterControl_None;
            }
        }

        public static string GetTypeName(DisplayFilterSettings.SegmentReviewType type)
        {
            switch (type)
            {
                case DisplayFilterSettings.SegmentReviewType.WithFeedbackMessages:
                    return StringResources.DisplayFilterControl_With_messages;
                case DisplayFilterSettings.SegmentReviewType.WithComments:
                    return StringResources.DisplayFilterControl_With_comments;
                case DisplayFilterSettings.SegmentReviewType.WithTrackedChanges:
                    return StringResources.DisplayFilterControl_With_tracked_changes;
                case DisplayFilterSettings.SegmentReviewType.WithTQA:
                    return StringResources.DisplayFilterControl_With_TQA;
                default:
                    return StringResources.DisplayFilterControl_None;
            }
        }

        public static string GetTypeName(DisplayFilterSettings.SegmentLockingType type)
        {
            switch (type)
            {
                case DisplayFilterSettings.SegmentLockingType.Locked:
                    return StringResources.DisplayFilterControl_Locked;
                case DisplayFilterSettings.SegmentLockingType.Unlocked:
                    return StringResources.DisplayFilterControl_Unlocked;
                default:
                    return StringResources.DisplayFilterControl_None;
            }
        }

        public static string GetTypeName(DisplayFilterSettings.SegmentContentType type)
        {
            switch (type)
            {
                case DisplayFilterSettings.SegmentContentType.NumbersOnly:
                    return StringResources.DisplayFilterControl_Number_Only;
                case DisplayFilterSettings.SegmentContentType.ExcludeNumberOnly:
                    return StringResources.DisplayFilterControl_Exclude_number_only;
                default:
                    return StringResources.DisplayFilterControl_None;
            }
        }
    }
}
