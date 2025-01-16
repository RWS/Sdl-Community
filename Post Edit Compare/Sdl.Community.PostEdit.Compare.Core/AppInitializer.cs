using Sdl.Community.PostEdit.Compare.Core.ActionsFromReport;
using Sdl.Community.PostEdit.Compare.Core.TrackChanges;
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
        public void Execute()
        {
            SdlTradosStudio.Application.GetService<IStudioEventAggregator>()
                .GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);

            Listener.StartHttpListener();
        }

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj) => ChangeTracker.TrackChosenTUsFromTMs();
    }
}