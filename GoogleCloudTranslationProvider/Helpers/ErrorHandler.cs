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
			HandleError($"The error was also logged on the AppData folder.\n{exception.Message}\n{exception.StackTrace}", "Unexpected error");
		}
	}
}