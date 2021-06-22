using Microsoft.Win32;
using Sdl.Community.StarTransit.Helpers;
using Sdl.Community.StarTransit.Interface;

namespace Sdl.Community.StarTransit.Service
{
	public class DialogService:IDialogService
	{
		public string ShowFileDialog(string filter,string title)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = filter,
				Title = title,
				Multiselect = false
			};
			return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
		}

		public string ShowFolderDialog(string dialogTitle)
		{
			var folderDialog = new FolderSelectDialog
			{
				Title = dialogTitle
			};
			return folderDialog.ShowDialog() ? folderDialog.FileName : string.Empty;
		}
	}
}
