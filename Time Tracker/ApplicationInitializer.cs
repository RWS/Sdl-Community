using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.Studio.Time.Tracker
{
    [ApplicationInitializer]
    public class ApplicationInitializer : IApplicationInitializer
    {
        private static StudioTimeTrackerViewController _studioTimeTrackerViewController;

        private StudioTimeTrackerViewController StudioTimeTrackerViewController => _studioTimeTrackerViewController ??=
                    SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>();

        public void Execute() =>
            SdlTradosStudio.Application.GetService<IStudioEventAggregator>()
                .GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent @event) => StudioTimeTrackerViewController.Initialize();
    }
}