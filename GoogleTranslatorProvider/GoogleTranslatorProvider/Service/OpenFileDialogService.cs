using System.Windows.Forms;
using GoogleTranslatorProvider.Interfaces;
using Microsoft.Win32;

namespace GoogleTranslatorProvider.Service
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