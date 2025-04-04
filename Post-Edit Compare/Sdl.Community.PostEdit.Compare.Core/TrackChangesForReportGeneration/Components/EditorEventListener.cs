using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration.Components
{
    public class EditorEventListener
    {
        public static event Action ActiveSegmentChanged;

        public static event Action ActiveSegmentConfirmationLevelChanged;

        public static IStudioDocument ActiveDocument { get; set; }
        private static EditorController EditorController => SdlTradosStudio.Application.GetController<EditorController>();

        public void StartListening()
        {
            EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
            SetUpActiveDocument();
        }

        private static void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
        {
            ActiveSegmentChanged?.Invoke();
        }

        private static void ActiveDocument_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            ActiveSegmentConfirmationLevelChanged?.Invoke();
        }

        private static void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            SetUpActiveDocument();
        }

        private static void SetUpActiveDocument()
        {
            StopListeningPreviousDocument();
            StartListeningCurrentDocument();
        }

        private static void StartListeningCurrentDocument()
        {
            if (EditorController.ActiveDocument is null) return;
            ActiveDocument = EditorController.ActiveDocument;

            ActiveDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
            ActiveDocument.SegmentsConfirmationLevelChanged += ActiveDocument_SegmentsConfirmationLevelChanged;
            ActiveDocument_ActiveSegmentChanged(null, null);
        }

        private static void StopListeningPreviousDocument()
        {
            if (ActiveDocument is null) return;

            ActiveDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
            ActiveDocument.SegmentsConfirmationLevelChanged -= ActiveDocument_SegmentsConfirmationLevelChanged;
        }
    }
}