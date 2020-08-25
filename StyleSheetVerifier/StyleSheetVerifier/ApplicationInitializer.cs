using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.StyleSheetVerifier
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