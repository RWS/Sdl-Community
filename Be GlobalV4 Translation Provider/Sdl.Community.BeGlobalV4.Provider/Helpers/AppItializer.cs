using System.Windows;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public static class AppItializer
	{
		public static void EnsureInitializer()
		{
			if (Application.Current == null)
			{
				var app = new Application
				{
					ShutdownMode = ShutdownMode.OnExplicitShutdown
				};
			}
			else
			{
				var app = Application.Current;
			}
		}
	}
}
