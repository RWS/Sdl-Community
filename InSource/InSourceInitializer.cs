using System;
using System.Drawing;
using System.Windows.Input;
using BrightIdeasSoftware;
using Sdl.Community.InSource.Notifications;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Notifications;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.InSource
{
    [ApplicationInitializer]
    class InSourceInitializer : IApplicationInitializer
    {
	    public void Execute()
	    {
		    var contentConnector = SdlTradosStudio.Application.GetController<InSourceViewController>();
		    contentConnector.CheckForProjects();
	    }
    }
}
