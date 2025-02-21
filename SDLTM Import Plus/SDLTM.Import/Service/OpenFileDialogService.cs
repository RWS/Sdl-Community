using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using SDLTM.Import.Interface;

namespace SDLTM.Import.Service
{
	public class OpenFileDialogService : IOpenFileDialogService
	{
		public List<string> FileNames { get; set; }

		/// <summary>
		/// Open File Dialog window
		/// </summary>
		/// <param name="filter">Files extensions</param>
		public void ShowDialog(string filter)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = filter,
				Multiselect = true
			};
			if (openFileDialog.ShowDialog() != true)
			{
				FileNames = new List<string>();
			}
			FileNames = openFileDialog.FileNames.ToList();
		}
	}
}
