using System.Collections.Generic;

namespace SDLTM.Import.Interface
{
    public interface IDialogService
    {
	    List<string> ShowFileDialog(string filter);
	    string ShowFolderDialog(string dialogTitle);
	    MessageDialogResult ShowYesNoDialogResult(string title, string message);
    }
}
