using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;
using System;

namespace Sdl.Community.Studio.Time.Tracker
{
    [ApplicationInitializer]
    public class ApplicationInitializer : IApplicationInitializer
    {
        private static StudioTimeTrackerViewController _studioTimeTrackerViewController;

        private SdlTradosStudioApplication SdlTradosStudioApplication => SdlTradosStudio.Application;

        private StudioTimeTrackerViewController StudioTimeTrackerViewController => _studioTimeTrackerViewController ??=
                            SdlTradosStudioApplication.GetController<StudioTimeTrackerViewController>();

        public void Execute() =>
            SdlTradosStudioApplication.GetService<IStudioEventAggregator>()
                .GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent @event) => StudioTimeTrackerViewController.Initialize();
    }
}