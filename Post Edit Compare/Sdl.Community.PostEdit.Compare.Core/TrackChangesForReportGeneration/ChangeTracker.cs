using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using System;

namespace Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration
{
    public static class ChangeTracker
    {
        public static void TrackChosenTUsFromTMs() =>
            AppInitializer.EditorController.TranslationResultsController.TranslationFinished += OnSegmentsConfirmationLevelChanged;

        private static void OnSegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            var activeSegmentPair = AppInitializer.EditorController.ActiveDocument.GetActiveSegmentPair();
            var activeSegmentTranslationOrigin = activeSegmentPair?.Properties.TranslationOrigin;
            if (activeSegmentTranslationOrigin is null) return;

            if (activeSegmentTranslationOrigin.OriginType?.ToUpperInvariant() != "TM" ||
                activeSegmentTranslationOrigin.MetaDataContainsKey(Constants.OriginalTuKey)) return;

            activeSegmentTranslationOrigin.SetMetaData(Constants.OriginalTuKey, activeSegmentPair.Target.ToString());
            AppInitializer.EditorController.ActiveDocument.UpdateSegmentPairProperties(activeSegmentPair,
                activeSegmentPair.Properties);
        }
    }
}