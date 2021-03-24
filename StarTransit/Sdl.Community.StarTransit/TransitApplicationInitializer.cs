using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.StarTransit
{
	[ApplicationInitializer]
	public class TransitApplicationInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			Log.Setup();
		}
	}
}
