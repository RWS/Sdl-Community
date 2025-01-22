using Sdl.Community.PostEdit.Compare.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Components
{
    public static class StudioInteractionListener
    {
        public static EditorController EditorController { get; set; } = AppInitializer.EditorController;

        private static IStudioDocument ActiveDocument { get; set; }

        public static void StartListening() =>
            EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

        private static void ActiveDocument_ContentChanged(object sender, DocumentContentEventArgs e)
        {
            //if (e.ContentChangeType == DocumentContentChangeType.SegmentPairPropertiesChanged)
            //{
            //    var segmentPair = ActiveDocument.GetActiveSegmentPair();
            //    if (segmentPair is null) return;
            //    var segment = segmentPair.Target;
            //    var segmentPairProperties = segment.Properties;
            //    AddCommentToReport(segment.Id, segmentPairProperties.Comment);
            //}
        }

        private static void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            if (ActiveDocument is not null)
            {
                ActiveDocument.ContentChanged -= ActiveDocument_ContentChanged;
                ActiveDocument.ActiveFilePropertiesChanged -= ActiveDocument_ActiveFilePropertiesChanged;
                EditorController.ActiveDocument.ContentChanged += ActiveDocument_ContentChanged1;
            }

            if (EditorController.ActiveDocument is null) return;

            ActiveDocument = EditorController.ActiveDocument;
            ActiveDocument.ContentChanged += ActiveDocument_ContentChanged; ;
            ActiveDocument.ActiveFilePropertiesChanged += ActiveDocument_ActiveFilePropertiesChanged;
        }

        private static void ActiveDocument_ContentChanged1(object sender, DocumentContentEventArgs e)
        {
            
        }

        private static void ActiveDocument_ActiveFilePropertiesChanged(object sender, System.EventArgs e)
        {

        }
    }
}