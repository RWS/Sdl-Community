using System.IO;
using System.Linq;
using System.Windows;

namespace Sdl.Community.StarTransit.UI.Helpers
{
	public static class Utils
	{
		public static bool IsFolderEmpty(string folderPath)
		{
			if (string.IsNullOrEmpty(folderPath)) return false;

			return !Directory
				.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
				.Any();
		}

		public static void EnsureApplicationResources()
		{
			if (Application.Current == null)
			{
				new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
			}
		}
	}
}