using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.PostEdit.Versions
{
    [ApplicationInitializer]
    public class AppInitializer : IApplicationInitializer
    {
        public static EditorController EditorController { get; set; }

        public static string GetActiveFileId() =>
            FileIdentifier.GetFileInfo(EditorController.ActiveDocument.ActiveFile.LocalFilePath);

        public void Execute() => EventAggregator.Subscribe<StudioWindowCreatedNotificationEvent>(OnStudioWindowCreated);

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj)
        {
            try
            {
                EditorController = SdlTradosStudio.Application.GetController<EditorController>();
                ChangeTracker.TrackChosenTUsFromTMs();
                SdlTradosStudio.Application.GetController<PostEditCompareViewController>().Initialize();
                SdlTradosStudio.Application.GetController<PostEditCompareViewController>().CheckEnabledObjects();
            }
            catch (Exception ex)
            {
                ErrorHandler.ShowError(ex);
            }
        }
    }
}