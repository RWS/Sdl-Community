using System;
using System.Media;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;

namespace LanguageWeaverProvider.Extensions
{
	public static class ErrorHandling
    {
		public static void ShowDialog(this Exception exception, string title, string message, bool displayDetailedReport = false)
		{
			var passedException = displayDetailedReport ? exception : null;
			var edViewModel = new ErrorDialogViewModel(title, message, passedException);

			var edView = new ErrorDialogView() { DataContext = edViewModel };
			edViewModel.CloseEventRaised += edView.Close;
			SystemSounds.Beep.Play();
			edView.ShowDialog();
		}
    }
}