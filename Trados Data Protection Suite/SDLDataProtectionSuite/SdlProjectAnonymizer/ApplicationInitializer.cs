using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			Log.Setup();
		}
	}
}
