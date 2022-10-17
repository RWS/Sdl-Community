using Microsoft.Win32;
using MTEnhancedMicrosoftProvider.Interfaces;

namespace MTEnhancedMicrosoftProvider.Service
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