using Sdl.Community.PostEdit.Compare.Core.HTMLReportIntegration.Components;
using Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.PostEdit.Compare.Core
{
    [ApplicationInitializer]
    public class AppInitializer : IApplicationInitializer
    {
        public static EditorController EditorController { get; set; }

        public void Execute()
        {
            SdlTradosStudio.Application.GetService<IStudioEventAggregator>()
                .GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);

            ReportInteractionListener.StartListening();
        }

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj)
        {
            EditorController = SdlTradosStudio.Application.GetController<EditorController>();
            ChangeTracker.TrackChosenTUsFromTMs();
            StudioInteractionListener.StartListening();
        }
    }
}