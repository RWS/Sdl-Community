using System;
using System.IO;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	public static class ProjectBackup
	{
		public static void CreateProjectBackup(string projectPath)
		{
			var projectFolder = Path.GetDirectoryName(projectPath);
			var projectName = Path.GetFileNameWithoutExtension(projectPath);

			var communityFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community");
			var projectFolderPath = Path.Combine(communityFolderPath, "SDLProjectAnonymizer");
			var backupFolderPath = Path.Combine(projectFolderPath, "Backup");

			if (!Directory.Exists(communityFolderPath))
			{
				Directory.CreateDirectory(communityFolderPath);
			}

			if (!Directory.Exists(projectFolderPath))
			{
				Directory.CreateDirectory(projectFolderPath);
			}

			if (!Directory.Exists(backupFolderPath))
			{
				Directory.CreateDirectory(backupFolderPath);
			}

			var projectBackupPath = Path.Combine(backupFolderPath, projectName ?? throw new InvalidOperationException());

			//Create a backup of initial project
			//Create it only once
			if (!Directory.Exists(projectBackupPath))
			{
				Directory.CreateDirectory(projectBackupPath);
				DirectoryCopy(projectFolder, projectBackupPath, true);
			}
		}

		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			var dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					string.Format(StringResources.Source_directory_does_not_exist_or_could_not_be_found_0, sourceDirName));
			}

			var dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			var files = dir.GetFiles();
			foreach (var file in files)
			{
				var temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (var subdir in dirs)
				{
					var temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}