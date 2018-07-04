using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
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
