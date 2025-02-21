using System.Collections.Generic;

namespace SDLTM.Import.Interface
{
    public interface IOpenFileDialogService
    {
	    void ShowDialog(string filter);
	    List<string> FileNames { get; set; }
	}
}
