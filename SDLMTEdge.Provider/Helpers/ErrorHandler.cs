using System.Windows.Forms;
using System;

namespace Sdl.Community.MTEdge.Provider.Helpers
{
	public class ErrorHandler
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
			HandleError($"An unexpected error occured.\nThe error was logged at -to be set-.\n\n{exception.Message}", "Unexpected error");
		}
	}
}