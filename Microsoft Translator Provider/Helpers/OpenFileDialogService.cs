using Microsoft.Win32;
using MicrosoftTranslatorProvider.Interfaces;

namespace MicrosoftTranslatorProvider.Helpers
{
	public class OpenFileDialogService : IOpenFileDialogService
	{
		public string ShowDialog(string filter)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = filter,
				Multiselect = false
			};

			return (bool)openFileDialog.ShowDialog() ? openFileDialog.FileName
													 : string.Empty;
		}
	}
}