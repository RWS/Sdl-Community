using System;
using System.IO;

namespace Sdl.Community.FileTypeSupport.MXLIFF.Utils
{
	public class Helper
	{
		private readonly string _communityFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore", "MXLIFF File Type Support", "Native MXLIFF Files");

		// Backup the native .mxliff file locally
		// When user creates a project using the new project wizard, the native file is used when user saves the translated file using "Save Target As" option
		public void BackupNativeFile(string nativeFilePath)
		{
			Directory.CreateDirectory(_communityFolder);

			var backupFile = Path.Combine(_communityFolder, Path.GetFileName(nativeFilePath));
			File.Copy(nativeFilePath, backupFile, true);
		}

		// Get the corresponding mxliff file from the backup  native files folder
		public string GetBackupFile(string fileNamePath)
		{
			var fileName = Path.GetFileNameWithoutExtension(fileNamePath);
			var mxliffFiles = Directory.GetFiles(_communityFolder);

			foreach (var file in mxliffFiles)
			{
				if (Path.GetFileNameWithoutExtension(file).Equals(fileName))
				{
					return file;
				}
			}

			return string.Empty;
		}
	}
}