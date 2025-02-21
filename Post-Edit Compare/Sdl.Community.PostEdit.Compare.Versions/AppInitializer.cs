using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.PostEdit.Compare.Core
{
    [ApplicationInitializer]
    public class AppInitializer : IApplicationInitializer
    {
        private static EditorController EditorController { get; set; }

        public static string GetActiveFileId() =>
            FileIdentifier.GetFileInfo(EditorController.ActiveDocument.ActiveFile.LocalFilePath);

        public void Execute()
        {
            EventAggregator.Subscribe<StudioWindowCreatedNotificationEvent>(OnStudioWindowCreated);
        }

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj)
        {
            EditorController = SdlTradosStudio.Application.GetController<EditorController>();
            ChangeTracker.TrackChosenTUsFromTMs();
        }
    }
}