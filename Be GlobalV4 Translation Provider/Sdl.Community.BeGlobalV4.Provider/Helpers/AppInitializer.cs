using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	[ApplicationInitializer]
	public sealed class AppInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			if (Application.Current == null)
			{
				new Application();
			}

			if (Application.Current != null)
			{
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			}
		}
	}
}