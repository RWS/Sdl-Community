using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
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
		public static string GetTypeName(DisplayFilterSettings.OriginType type)
		{
			switch (type)
			{ case DisplayFilterSettings.OriginType.Source:
					return StringResources.DisplayFilterControl_Copied_from_source;
				case DisplayFilterSettings.OriginType.AutoPropagated:
					return StringResources.DisplayFilterControl_Auto_propagated;
				case DisplayFilterSettings.OriginType.Exact:
					return StringResources.DisplayFilterControl_Exact_matches;
				case DisplayFilterSettings.OriginType.Fuzzy:
					return StringResources.DisplayFilterControl_Fuzzy_matches;
				case DisplayFilterSettings.OriginType.PM:
					return StringResources.DisplayFilterControl_Perfect_matches;
				case DisplayFilterSettings.OriginType.CM:
					return StringResources.DisplayFilterControl_Context_matches;
				case DisplayFilterSettings.OriginType.AT:
					return StringResources.DisplayFilterControl_Automated_translations;
				case DisplayFilterSettings.OriginType.FuzzyMatchRepair:
					return StringResources.DisplayFilterControl_Fuzzy_Match_Repair;
				case DisplayFilterSettings.OriginType.EditedFuzzy:
					return StringResources.DisplayFilterControl_Edited_Fuzzy;
				case DisplayFilterSettings.OriginType.UneditedFuzzy:
					return StringResources.DisplayFilterControl_Unedited_Fuzzy;
				case DisplayFilterSettings.OriginType.NMT:
					return StringResources.DisplayFilterControl_Neural_Machine_Translation;
				case DisplayFilterSettings.OriginType.NewTranslated:
					return StringResources.DisplayFilter_Control_New_Translated;
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
			    case DisplayFilterSettings.RepetitionType.UniqueOccurrences:
				    return StringResources.DisplayFilterControl_Unique_Occurrences;
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
				case DisplayFilterSettings.SegmentReviewType.WithSourceTrackedChanges:
					return StringResources.DisplayFilterControl_With_Source_Tracked_changes;
	            case DisplayFilterSettings.SegmentReviewType.WithTargetTrackedChanges:
		            return StringResources.DisplayFilterControl_With_Target_Tracked_changes;
	            case DisplayFilterSettings.SegmentReviewType.WithSourceAndTargetTrackedChanges:
		            return StringResources.DisplayFilterControl_With_SourceAndTarget_Tracked_changes;
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

	    public static string GetTypeName(DisplayFilterSettings.ContentLocation type)
	    {
			switch (type)
			{
				case DisplayFilterSettings.ContentLocation.SourceAndTarget:
					return "Source and Target Segment";
				case DisplayFilterSettings.ContentLocation.SourceOrTarget:
					return "Source or Target Segment";
				case DisplayFilterSettings.ContentLocation.Source:
					return "Source Segment";
				case DisplayFilterSettings.ContentLocation.Target:
					return "Target Segment";
				default:
					return StringResources.DisplayFilterControl_None;
			}
		}
	}
}
