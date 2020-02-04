using Sdl.Core.Globalization;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public static class SegmentTypesHelper
	{
		public static bool IsNewContent(DisplayFilterRowInfo rowInfo)
		{
			var isFuzzy = rowInfo.SegmentPair.Properties.TranslationOrigin == null ||
			              rowInfo.SegmentPair.Properties.TranslationOrigin.MatchPercent < 100;

			var isInteractiveTransaltion = rowInfo.SegmentPair.Properties.TranslationOrigin == null ||
			                               rowInfo.SegmentPair.Properties.TranslationOrigin.OriginType?.ToLower() == "interactive";

			var confirmationLevel = rowInfo.SegmentPair.Properties.ConfirmationLevel;
			var isConfirmed = confirmationLevel == ConfirmationLevel.ApprovedSignOff ||
			                  confirmationLevel == ConfirmationLevel.ApprovedTranslation ||
			                  confirmationLevel == ConfirmationLevel.Translated;

			// show all segments that have been edited by user
			return isConfirmed && (isInteractiveTransaltion || isFuzzy);
		}
	}
}
