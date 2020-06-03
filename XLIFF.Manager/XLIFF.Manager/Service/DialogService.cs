using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Interfaces;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class DialogService:IDialogService
	{
		public List<string> ShowFileDialog(string filter,string title)
		{
			try
			{
				var openFileDialog = new OpenFileDialog
				{
					Filter = filter.ToLower(),
					Multiselect = true,
					Title = title
				};
				return openFileDialog.ShowDialog() != true ? new List<string>() : openFileDialog.FileNames.ToList();
			}
			catch (Exception e)
			{
				return new List<string>();
			}
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
