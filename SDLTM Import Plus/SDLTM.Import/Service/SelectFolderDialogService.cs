using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;

namespace SDLTM.Import.Service
{
    public class SelectFolderDialogService:ISelectFolderDialogService
    {
	    public string FolderPath { get; set; }

		public void ShowDialog(string dialogTitle)
	    {
		    FolderPath = string.Empty;
			var folderDialog = new FolderSelectDialog
		    {
			    Title = dialogTitle
		    };
		    if (folderDialog.ShowDialog())
		    {
			    FolderPath = folderDialog.FileName;
		    }
		}
    }
}
