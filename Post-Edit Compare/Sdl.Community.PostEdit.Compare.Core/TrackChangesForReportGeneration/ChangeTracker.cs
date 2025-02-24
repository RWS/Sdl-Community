using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration
{
    public static class ChangeTracker
    {
        private static EditorController EditorController => SdlTradosStudio.Application.GetController<EditorController>();

        public static void TrackChosenTUsFromTMs() =>
            EditorController.TranslationResultsController.TranslationFinished += OnSegmentsConfirmationLevelChanged;

        private static void OnSegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            var activeSegmentPair = SdlTradosStudio.Application.GetController<EditorController>().ActiveDocument.GetActiveSegmentPair();
            var activeSegmentTranslationOrigin = activeSegmentPair?.Properties.TranslationOrigin;
            if (activeSegmentTranslationOrigin is null) return;

            if (activeSegmentTranslationOrigin.OriginType?.ToUpperInvariant() != "TM" ||
                activeSegmentTranslationOrigin.MetaDataContainsKey(Constants.OriginalTuKey)) return;

            activeSegmentTranslationOrigin.SetMetaData(Constants.OriginalTuKey, activeSegmentPair.Target.ToString());
            SdlTradosStudio.Application.GetController<EditorController>().ActiveDocument.UpdateSegmentPairProperties(activeSegmentPair,
                activeSegmentPair.Properties);
        }
    }
}