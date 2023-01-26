using System;
using System.Windows.Forms;

namespace GoogleCloudTranslationProvider.Helpers
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
			HandleError($"An unexpected error occured.\nThe error was logged at {Constants.AppDataFolder}.\n\n{exception.Message}", "Unexpected error");
		}
	}
}