using Sdl.Core.Globalization;

namespace ExportToExcel
{
    public class SegmentStatus
    {
	    public ConfirmationLevel Status { get; set; }

	    public SegmentStatus(ConfirmationLevel status)
        {
            Status = status;
        }

        /// <summary>
        /// Take the information from Resources file
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Status)
            {
                case ConfirmationLevel.ApprovedSignOff:
                    return PluginResources.ApprovedSignOff;
                case ConfirmationLevel.ApprovedTranslation:
                    return PluginResources.ApprovedTranslation;
                case ConfirmationLevel.Draft:
                    return PluginResources.Draft;
                case ConfirmationLevel.RejectedSignOff:
                    return PluginResources.RejectedSignOff;
                case ConfirmationLevel.RejectedTranslation:
                    return PluginResources.RejectedTranslation;
                case ConfirmationLevel.Translated:
                    return PluginResources.Translated;
                case ConfirmationLevel.Unspecified:
                    return PluginResources.Unspecified;
                default:
                    return PluginResources.Unspecified;
            }
        }
    }
}
