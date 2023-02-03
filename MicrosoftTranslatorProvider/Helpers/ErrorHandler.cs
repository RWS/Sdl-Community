using System;
using System.IO;
using System.Windows.Forms;

namespace MicrosoftTranslatorProvider.Helpers
{
	public static class ErrorHandler
	{
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
			HandleError($"An unexpected error occured.\nThe error was logged at {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PluginResources.LogsFolderPath, PluginResources.AppLogFolder)}.\n\n{exception.Message}", "Unexpected error");
		}
	}
}