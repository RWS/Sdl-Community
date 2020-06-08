using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Interfaces;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class DialogService : IDialogService
	{
		public List<string> ShowFileDialog(string filter, string title, string initialDirectory)
		{
			try
			{				
				var openFileDialog = new OpenFileDialog
				{
					Filter = filter.ToLower(),
					Multiselect = true,
					Title = title,
					InitialDirectory = initialDirectory
				};
				return openFileDialog.ShowDialog() != true
					? new List<string>()
					: openFileDialog.FileNames.ToList();
			}
			catch
			{
				return new List<string>();
			}
		}

		public string ShowFolderDialog(string dialogTitle, string initialDirectory)
		{			
			if (string.IsNullOrEmpty(initialDirectory) || !Directory.Exists(initialDirectory))
			{
				initialDirectory = null;
			}

			var folderDialog = new FolderSelectDialog
			{
				Title = dialogTitle,
				InitialDirectory = initialDirectory
			};

			return folderDialog.ShowDialog()
				? folderDialog.FileName
				: string.Empty;
		}
	}
}
