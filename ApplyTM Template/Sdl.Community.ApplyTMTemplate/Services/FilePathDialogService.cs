using System;
using System.IO;
using System.Windows.Forms;
using Sdl.Community.ApplyTMTemplate.Utilities;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace Sdl.Community.ApplyTMTemplate.Services
{
	public class FilePathDialogService
	{
		public string[] GetFilePathInputFromUser(string title = "", string initialDirectory = "", string filter = "", bool multiselect = false)
		{
			initialDirectory = "";
			GetInitialPath(initialDirectory);
			var dlg = new OpenFileDialog
			{
				Title = title,
				Filter = filter,
				InitialDirectory = GetInitialPath(initialDirectory),
				Multiselect = multiselect,
				RestoreDirectory = true,
			};

			var result = dlg.ShowDialog();
			if (!(result ?? false))
			{
				return null;
			}
			return dlg.FileNames;
		}

		private string GetInitialPath(string initialDirectory)
		{
			return string.IsNullOrWhiteSpace(initialDirectory)
				? ""
				: initialDirectory.Contains(".")
					? initialDirectory.Substring(0, initialDirectory.LastIndexOf('\\') + 1)
					: initialDirectory;
		}

		public string[] GetFilesFromFolderInputByUser(string title = "", string initialPath = "")
		{
			var dlg = new FolderSelectDialog
			{
				Title = title,
				InitialDirectory = initialPath,
			};

			if (!dlg.ShowDialog())
			{
				return null;
			}
			
			return Directory.GetFiles(dlg.FileName);
		}

		public void GetSaveLocationInputFromUser(out string saveLocation, string title = "", string filter = "", string fileName = "")
		{
			var dlg = new SaveFileDialog
			{
				Title = title,
				Filter = filter,
				FileName = fileName,
				AddExtension = false,
				RestoreDirectory = true
			};

			if (dlg.ShowDialog() != DialogResult.OK)
			{
				saveLocation = null;
				return;
			}

			saveLocation = dlg.FileName;
			if (File.Exists(saveLocation))
			{
				try
				{
					File.Delete(saveLocation);
				}
				catch
				{
					saveLocation = CreateNewFile(saveLocation);
					throw;
				}
			}
		}

		private string CreateNewFile(string filePath)
		{
			var index = 0;
			while (true)
			{
				if (index == 0)
				{
					filePath = filePath.Insert(filePath.IndexOf(".xlsx", StringComparison.OrdinalIgnoreCase),
						$"_{index}");
				}
				else
				{
					filePath = filePath.Replace((index - 1).ToString(), index.ToString());
				}

				if (File.Exists(filePath))
				{
					index++;
					continue;
				}

				break;
			}

			return filePath;
		}
	}
}