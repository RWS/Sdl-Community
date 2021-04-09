using Microsoft.Win32;
using Sdl.Community.StarTransit.Interface;

namespace Sdl.Community.StarTransit.Service
{
	public class OpenFileDialogService : IOpenFileDialogService
	{
		public string ShowDialog(string filter)
		{
			var openFileDialog = new OpenFileDialog {Filter = filter, Multiselect = false};
			return openFileDialog.ShowDialog() != true ? string.Empty : openFileDialog.FileName;
		}
	}
}
