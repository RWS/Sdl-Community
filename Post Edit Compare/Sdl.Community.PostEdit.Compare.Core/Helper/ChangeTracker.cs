using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
    public static class ChangeTracker
    {
        private const string OriginalTuKey = "original_tu";

        private static EditorController _editorController;
        private static EditorController EditorController => _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();

        public static void TrackChosenTUsFromTMs() =>
            EditorController.TranslationResultsController.TranslationFinished += OnSegmentsConfirmationLevelChanged;

        private static void OnSegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            var activeSegmentPair = EditorController.ActiveDocument.GetActiveSegmentPair();
            var activeSegmentTranslationOrigin = activeSegmentPair?.Properties.TranslationOrigin;
            if (activeSegmentTranslationOrigin is null) return;

            if (activeSegmentTranslationOrigin.OriginType?.ToUpperInvariant() != "TM" ||
                activeSegmentTranslationOrigin.MetaDataContainsKey(OriginalTuKey)) return;

            activeSegmentTranslationOrigin.SetMetaData(OriginalTuKey, activeSegmentPair.Target.ToString());
            EditorController.ActiveDocument.UpdateSegmentPairProperties(activeSegmentPair,
                activeSegmentPair.Properties);
        }
    }
}