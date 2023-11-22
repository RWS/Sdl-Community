using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NLog;

namespace MicrosoftTranslatorProvider.Helpers
{
	public static class ErrorHandler
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public static void HandleError(string errorMessage, string source)
		{
			MessageBox.Show(errorMessage,
							source,
							MessageBoxButtons.OK,
							MessageBoxIcon.Error,
							MessageBoxDefaultButton.Button1);
		}

		public static void HandleError(Exception exception)
		{
			_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {exception}");
			HandleError($"An unexpected error occured.\nThe error was logged at {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PluginResources.LogsFolderPath, PluginResources.AppLogFolder)}.\n\n{exception.Message}", "Unexpected error");
		}
	}
}