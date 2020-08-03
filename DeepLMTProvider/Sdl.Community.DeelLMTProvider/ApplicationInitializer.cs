using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.DeepLMTProvider
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