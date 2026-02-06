using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
    [ApplicationInitializer]
    public class CommunityApplicationInitializer : IApplicationInitializer
    {
        private IStudioEventAggregator _eventAggregator;
        internal static DisplayFilterControl DisplayFilterControl { get; private set; }

        public void Execute()
        {
            _eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
            _eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);
        }

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent studioWindowCreatedNotificationEvent)
        {
            DisplayFilterControl = new DisplayFilterControl();
        }
    }
}