using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Interfaces
{
	public interface IDialogService
	{
		/// <summary>
		/// Open File Dialog window
		/// </summary>
		/// <param name="filter">Files extensions</param>
		List<string> ShowFileDialog(string filter,string title);

		/// <summary>
		/// Display folder dialog window
		/// </summary>
		/// <returns>Selected folder path</returns>
		string ShowFolderDialog(string dialogTitle);
	}
}
