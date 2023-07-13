using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    [ApplicationInitializer]
    internal class ApplicationInstance : IApplicationInitializer
    {
        public void Execute()
        {
            Log.Setup();
        }        
    }
}
