using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    public enum SegStatus
    {
        /// <summary>
        /// Represents Seg-Source Segment status
        /// </summary>
        Unspecified,

        /// <summary>
        /// Represents Seg-Source Segment status
        /// </summary>
        Draft,

        /// <summary>
        /// Represents Seg-Source Segment status
        /// </summary>
        Translated,

        /// <summary>
        /// Represents Seg-Source Segment status
        /// </summary>
        RejectedTranslation,

        /// <summary>
        /// Represents Seg-Source Segment status
        /// </summary>
        ApprovedTranslation,

        /// <summary>
        /// Represents Seg-Source Segment status
        /// </summary>
        RejectedSignOff,

        /// <summary>
        /// Represents Seg-Source Segment status
        /// </summary>
        ApprovedSignOff,

	    /// <summary>
	    /// Represents Unlocked Segment status
	    /// </summary>
		Unlocked
	}

    // not needed
    public static class TagSegStatus
    {
        public static SegStatus getTagSegStatus(string status)
        {
            switch (status)
            {
                case "Draft":
                    return SegStatus.Draft;
                case "Translated":
                    return SegStatus.Translated;
                case "Translation Rejected":
                    return SegStatus.RejectedTranslation;
                case "Translation Approved":
                    return SegStatus.ApprovedTranslation;
                case "Sign-off Rejected":
                    return SegStatus.RejectedSignOff;
                case "Signed Off":
                    return SegStatus.ApprovedSignOff;
				case "Unlocked":
					return SegStatus.Unlocked;
                default:
                    return SegStatus.Unspecified;
            }
        }

        public static string getTagSegStatus(SegStatus status)
        {
            switch (status)
            {
                case SegStatus.Draft:
                    return "Draft";
                case SegStatus.Translated:
                    return "Translated";
                case SegStatus.RejectedTranslation:
                    return "Translation Rejected";
                case SegStatus.ApprovedTranslation:
                    return "Translation Approved";
                case SegStatus.RejectedSignOff:
                    return "Sign-off Rejected";
                case SegStatus.ApprovedSignOff:
                    return "Signed Off";
				case SegStatus.Unlocked:
					return "Unlocked";
				default:
                    return "Not Translated";
            }
        }
    }
}
