using Sdl.Community.PostEdit.Compare.Core.Helper;
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
        public void Execute() => SdlTradosStudio.Application.GetService<IStudioEventAggregator>().GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj) => ChangeTracker.TrackChosenTUsFromTMs();
    }
}