namespace SDLTM.Import.Interface
{
    public interface ISelectFolderDialogService
    {
	    void ShowDialog(string dialogTitle);
	    string FolderPath { get; set; }
	}
}
