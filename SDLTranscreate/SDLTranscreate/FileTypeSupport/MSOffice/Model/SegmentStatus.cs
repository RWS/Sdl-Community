using Sdl.Core.Globalization;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
	public class SegmentStatus
	{
		public ConfirmationLevel Status { get; set; }

		public SegmentStatus(ConfirmationLevel status)
		{
			Status = status;
		}

		public override string ToString()
		{
			switch (Status)
			{
				case ConfirmationLevel.ApprovedSignOff:
					return StringResources.ApprovedSignOff;
				case ConfirmationLevel.ApprovedTranslation:
					return StringResources.ApprovedTranslation;
				case ConfirmationLevel.Draft:
					return StringResources.Draft;
				case ConfirmationLevel.RejectedSignOff:
					return StringResources.RejectedSignOff;
				case ConfirmationLevel.RejectedTranslation:
					return StringResources.RejectedTranslation;
				case ConfirmationLevel.Translated:
					return StringResources.Translated;
				case ConfirmationLevel.Unspecified:
					return StringResources.Unspecified;
				default:
					return StringResources.Unspecified;
			}
		}
	}
}
