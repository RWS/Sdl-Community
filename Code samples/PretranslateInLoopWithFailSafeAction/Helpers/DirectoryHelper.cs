using System;
using System.IO;
using System.Linq;

namespace PretranslateInLoopWithFailSafeAction.Helpers
{
	public static class DirectoryHelper
	{
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
		{
			sourceDirName = EnsureParentDirectory(sourceDirName);
			destDirName = EnsureParentDirectory(destDirName);

			var dir = new DirectoryInfo(sourceDirName);
			var dirs = dir.GetDirectories();
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			var files = dir.GetFiles();
			foreach (var file in files)
			{
				var temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			if (copySubDirs)
			{
				foreach (var subdir in dirs)
				{
					var temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}

		public static string EnsurePathExists(string destDirName)
		{
			if (Directory.Exists(destDirName)) destDirName = $"{destDirName}_{Guid.NewGuid()}";
			return destDirName;
		}

		public static string GetSdlproj(string projectFolderPath)
					=> Directory.GetFiles(projectFolderPath).ToList().FirstOrDefault(f => Path.GetExtension(f) == ".sdlproj");

		private static string EnsureParentDirectory(string sourceDirName)
		{
			if (sourceDirName.Contains('.')) sourceDirName = Directory.GetParent(sourceDirName).FullName;
			return sourceDirName;
		}
	}
}