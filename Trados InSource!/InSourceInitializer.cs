using System;
using Sdl.Community.InSource.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.InSource
{
    [ApplicationInitializer]
    public class InSourceInitializer : IApplicationInitializer
    {
	    private IStudioEventAggregator _eventAggregator;
		public void Execute()
	    {
			Log.Setup();
		    _eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
		    _eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);
	    }

	    private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent e)
	    {
		    var contentConnector = SdlTradosStudio.Application.GetController<InSourceViewController>();
		    contentConnector.CheckForProjects();
	    }
	}
}
