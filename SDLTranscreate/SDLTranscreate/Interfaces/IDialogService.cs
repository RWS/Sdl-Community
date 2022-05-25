using System.Collections.Generic;

namespace Trados.Transcreate.Interfaces
{
	public interface IDialogService
	{
		/// <summary>
		/// Open File Dialog window
		/// </summary>
		/// <param name="filter">Files extensions</param>
		/// <param name="title">The window title</param>
		/// <param name="initialDirectory">The initial directory that is selected</param>
		List<string> ShowFileDialog(string filter, string title, string initialDirectory = null);

		/// <summary>
		/// Display folder dialog window
		/// </summary>
		/// <param name="title">The window title</param>
		/// <param name="initialDirectory">The initial directory that is selected</param>
		/// <returns>Selected folder path</returns>
		string ShowFolderDialog(string title, string initialDirectory = null);
	}
}
