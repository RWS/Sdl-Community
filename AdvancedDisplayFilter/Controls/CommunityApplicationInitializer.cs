using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
	[ApplicationInitializer]
	public  class CommunityApplicationInitializer: IApplicationInitializer
	{
		internal static DisplayFilterControl DisplayFilterControl { get; private set; }
		public void Execute()
		{
			DisplayFilterControl = new DisplayFilterControl();
		}
	}
}
