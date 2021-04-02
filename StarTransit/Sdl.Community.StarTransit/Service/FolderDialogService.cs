using Sdl.Community.StarTransit.Helpers;
using Sdl.Community.StarTransit.Interface;

namespace Sdl.Community.StarTransit.Service
{
	public class FolderDialogService:IFolderDialogService
	{
		public string ShowDialog(string dialogTitle)
		{
			var folderDialog = new FolderSelectDialog
			{
				Title = dialogTitle
			};
			return folderDialog.ShowDialog() ? folderDialog.FileName : string.Empty;
		}
	}
}
