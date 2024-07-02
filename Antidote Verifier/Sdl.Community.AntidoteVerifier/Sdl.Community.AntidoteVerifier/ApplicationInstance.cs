using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.AntidoteVerifier
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
