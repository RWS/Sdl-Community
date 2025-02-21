using System;
using System.Linq;
using System.Windows;
using Sdl.Versioning;

namespace Sdl.Community.HunspellDictionaryManager.Helpers
{
	public static class Utils
	{
		public static readonly Log Log = Log.Instance;

		public static void InitializeWpfApplicationSettings()
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

        /// <summary>
        /// Get installed version for Studio 2024
        /// </summary>
        /// <returns></returns>
        public static string GetInstalledStudioPath()
        {
			try
			{
				var studio = new StudioVersionService().GetInstalledStudioVersions()?.Where(v => v.Version.Equals("Studio18")).FirstOrDefault();
				if (studio != null)
				{
					return studio.InstallPath;
				}

				MessageBox.Show(Constants.Studio2024ErrorMessage, Constants.InformativeMessage, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetInstalledStudioPath}: {ex.Message}\n {ex.StackTrace}");
			}
			return string.Empty;
		}
	}
}