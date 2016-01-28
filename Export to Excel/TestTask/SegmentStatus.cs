using Sdl.Core.Globalization;

namespace ExportToExcel
{
    public class SegmentStatus
    {
        private ConfirmationLevel _status;

        public ConfirmationLevel Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public SegmentStatus(ConfirmationLevel status)
        {
            _status = status;
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
