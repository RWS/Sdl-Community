using System.IO;

namespace Sdl.Community.Utilities.TMTool.Lib.TMHelpers
{
	public static class FileHelper
	{
		/// <summary>
		/// changes file name for unique (using numbers)
		/// </summary>
		/// <param name="originalName">original file name</param>
		/// <param name="pattern">pattern to be changed by; has to contain 3 object instances</param>
		/// <returns>new changed file name (if original one already exists)</returns>
		public static string ChangeFileName(string originalName, string pattern)
		{
			int cnt = 1;
			string filePathNew = originalName;
			while (FileExists(filePathNew))
			{
				filePathNew = string.Format(pattern,
					Path.GetDirectoryName(originalName),
					Path.GetFileNameWithoutExtension(originalName),
					++cnt);
			}

			return filePathNew;
		}

		/// <summary>
		/// checks if file exists
		/// </summary>
		/// <param name="filePath">file name</param>
		/// <returns>true - if exists</returns>
		public static bool FileExists(string filePath)
		{
			if (Directory.Exists(Path.GetDirectoryName(filePath)))
				if (File.Exists(filePath))
					return true;

			return false;
		}

		/// <summary>
		/// copies file to indicated dir
		/// </summary>
		/// <param name="fromPath">file to copy</param>
		/// <param name="toPath">file path to copy to</param>
		/// <param name="isOverwrite">overwrite if exists</param>
		/// <returns>true - if succeeded</returns>
		public static bool CopyFile(string fromPath, string toPath, bool isOverwrite)
		{
			if (FileExists(fromPath))
				try
				{
					File.Copy(fromPath, toPath, true);
					return true;
				}
				catch { }

			return false;
		}
	}
}