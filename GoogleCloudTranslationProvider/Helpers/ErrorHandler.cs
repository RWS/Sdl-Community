using System;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleCloudTranslationProvider.Helpers
{
	public static class ErrorHandler
	{
		public static void HandleError(string errorMessage, string source)
		{
			Task.Run(() => { var dialogResult = MessageBox.Show(errorMessage, source); });
		}

		public static void HandleError(Exception exception)
		{
			HandleError($"An unexpected error occured.\nThe error was logged at {Constants.AppDataFolder}.\n\n{exception.Message}", "Unexpected error");
		}
	}
}