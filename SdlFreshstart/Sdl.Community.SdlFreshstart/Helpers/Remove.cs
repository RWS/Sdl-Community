using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class Remove
	{
		public static readonly Log Log = Log.Instance;

		public static async Task BackupFiles(List<LocationDetails> foldersToBackup)
		{
			await Task.Run(() => CreateBackupFolder(foldersToBackup));
		}

		public static async Task RestoreBackupFiles(List<LocationDetails> foldersToBackup)
		{
			await Task.Run(() => RestoreFiles(foldersToBackup));
		}

		private static void RestoreFiles(List<LocationDetails> foldersToBackup)
		{
			try
			{
				foreach (var folder in foldersToBackup)
				{
					if (!folder.BackupFilePath.Contains("projects.xml"))
					{
						//creates original folders if doesn't exist
						if (!Directory.Exists(folder.OriginalFilePath))
						{
							Directory.CreateDirectory(folder.OriginalFilePath);
						}

						//Get files  from backup
						var files = Directory.GetFiles(folder.BackupFilePath);
						if (files.Length > 0)
						{
							MoveToBackUp(files, folder.OriginalFilePath);
						}
						var subdirectories = Directory.GetDirectories(folder.BackupFilePath);
						foreach (var subdirectory in subdirectories)
						{
							var currentDirInfo = new DirectoryInfo(subdirectory);
							CheckForSubfolders(currentDirInfo, folder.OriginalFilePath);
						}
					}
					else
					{
						File.Copy(folder.BackupFilePath, folder.OriginalFilePath, true);
					}
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.RestoreFiles} {ex.Message}\n {ex.StackTrace}");
			}
		}

		public static async Task FromSelectedLocations(List<LocationDetails> foldersToRemove)
	    {
		    try
		    {
			    foreach (var folder in foldersToRemove)
			    {
				    var directory = await Task.FromResult(IsDirectory(folder.OriginalFilePath));

				    if (!directory)
				    {
					    File.Delete(folder.OriginalFilePath);
				    }
				    else
				    {
						var directoryInfo = new DirectoryInfo(folder.OriginalFilePath);
					    await Task.Run(() => RemoveDirectoryInfo(directoryInfo));
				    }
			    }
		    }
		    catch (Exception ex)
		    {
				Log.Logger.Error($"{Constants.FromSelectedLocations} {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

	    private static void CreateBackupFolder(List<LocationDetails> foldersToBackup)
	    {
			try
			{
				foreach (var folder in foldersToBackup)
				{
					if (!Directory.Exists(folder.BackupFilePath))
					{
						if (!folder.OriginalFilePath.Contains("projects.xml"))
						{
							Directory.CreateDirectory(folder.BackupFilePath);
						}
						else
						{
							//take any projects.xml and copy it
							var directoryInfo = new DirectoryInfo(folder.BackupFilePath);
							if (directoryInfo.Parent != null)
							{
								Directory.CreateDirectory(directoryInfo.Parent.FullName);
							}
							File.Copy(folder.OriginalFilePath, folder.BackupFilePath, true);
						}
						CopyFiles(folder);
					}
					else
					{
						if (Directory.Exists(folder.OriginalFilePath))
						{
							CopyFiles(folder);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.FromSelectedLocations} {ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Copy files from the original folder to the backup folder
		/// </summary>
		/// <param name="folder">folder details</param>
		private static void CopyFiles(LocationDetails folder)
		{
			try
			{
				// Get files 
				var files = Directory.GetFiles(folder.OriginalFilePath);
				if (files.Length > 0)
				{
					MoveToBackUp(files, folder.BackupFilePath);
				}

				// Check for subdirectories
				var subdirectories = Directory.GetDirectories(folder.OriginalFilePath);
				foreach (var subdirectory in subdirectories)
				{
					var currentDirInfo = new DirectoryInfo(subdirectory);
					CheckForSubfolders(currentDirInfo, folder.BackupFilePath);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.CopyFiles} {ex.Message}\n {ex.StackTrace}");
			}
		}

	    private static void CheckForSubfolders(DirectoryInfo currentDirInfo, string backupFolderRoot)
	    {
			try
			{
				var subdirectories = currentDirInfo.GetDirectories();
				if (currentDirInfo.Parent != null)
				{
					var pathToCorespondingBackupFolder = Path.Combine(backupFolderRoot, currentDirInfo.Name);
					var subdirectoryFiles = Directory.GetFiles(currentDirInfo.FullName);
					if (subdirectoryFiles.Length > 0)
					{
						if (!Directory.Exists(pathToCorespondingBackupFolder))
						{
							Directory.CreateDirectory(pathToCorespondingBackupFolder);
						}
						MoveToBackUp(subdirectoryFiles, pathToCorespondingBackupFolder);
					}
					if (subdirectories.Length > 0)
					{
						foreach (var subdirectory in subdirectories)
						{
							CheckForSubfolders(subdirectory, pathToCorespondingBackupFolder);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.CheckForSubfolders} {ex.Message}\n {ex.StackTrace}");
			}
		}

	    private static void MoveToBackUp(string[] files, string backupPath)
	    {
		    foreach (var file in files)
		    {
			    var fileName = Path.GetFileName(file);
			    try
			    {
				    if (fileName != null)
				    {
					    File.Copy(file, Path.Combine(backupPath, fileName), true);
				    }
			    }
			    catch (Exception ex)
			    {
					Log.Logger.Error($"{Constants.MoveToBackUp} {ex.Message}\n {ex.StackTrace}");
				}
			}
	    }

	    private static void RemoveDirectoryInfo(DirectoryInfo directoryInfo)
	    {
			try
			{
				if (directoryInfo.Exists)
				{
					SetAttributesNormal(directoryInfo);
					directoryInfo.Delete(true);
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.RemoveDirectoryInfo} {ex.Message}\n {ex.StackTrace}");
			}
		}

	    private static void SetAttributesNormal(DirectoryInfo directory)
	    {
			foreach (var subDir in directory.GetDirectories())
			{
				SetAttributesNormal(subDir);
			}
		    foreach (var file in directory.GetFiles())
		    {
			    file.Attributes = FileAttributes.Normal;
		    }
		}

	    private static bool IsDirectory(string path)
	    {
		    var attributes = File.GetAttributes(path);
		    return attributes.HasFlag(FileAttributes.Directory);
	    }
    }
}