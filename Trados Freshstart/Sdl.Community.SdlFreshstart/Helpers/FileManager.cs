using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NLog;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class FileManager
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
					if (!folder.BackupFilePath.Contains("projects.xml") && !folder.BackupFilePath.Contains("Sdl.ProjectApi.xml"))
					{
						//creates original folders if doesn't exist
						if (!Directory.Exists(folder.OriginalPath))
						{
							Directory.CreateDirectory(folder.OriginalPath);
						}

						//Get files  from backup
						var files = Directory.GetFiles(folder.BackupFilePath);
						if (files.Length > 0)
						{
							MoveToBackUp(files, folder.OriginalPath);
						}
						var subdirectories = Directory.GetDirectories(folder.BackupFilePath);
						foreach (var subdirectory in subdirectories)
						{
							var currentDirInfo = new DirectoryInfo(subdirectory);
							CheckForSubfolders(currentDirInfo, folder.OriginalPath);
						}
					}
					else
					{
						File.Copy(folder.BackupFilePath, folder.OriginalPath, true);
					}
				}
			}
			catch(Exception ex)
			{
				Logger.Error($"{Constants.RestoreFiles} {ex.Message}\n {ex.StackTrace}");
			}
		}

		public static void RemoveFromSelectedFolderLocations(List<LocationDetails> foldersToRemove)
	    {
		    try
		    {
			    foreach (var folder in foldersToRemove)
			    {
				    if (Directory.Exists(folder.OriginalPath))
				    {
						var directoryInfo = new DirectoryInfo(folder.OriginalPath);
						RemoveDirectoryInfo(directoryInfo);
				    }
				    else
				    {
					    if (!File.Exists(folder.OriginalPath)) continue;
					    File.Delete(folder.OriginalPath);
				    }
			    }
		    }
		    catch (Exception ex)
		    {
			    Logger.Error($"{Constants.FromSelectedLocations} {ex.Message}\n {ex.StackTrace}");
				throw;
			}
		}

	    private static void CreateBackupFolder(List<LocationDetails> foldersToBackup)
	    {
			try
			{
				foreach (var folder in foldersToBackup)
				{
					if  (folder is null) continue;

					if (folder.BackupFilePath.Contains("Sdl.ProjectApi.xml") && !File.Exists(folder.BackupFilePath))
					{
						File.Copy(folder.OriginalPath, folder.BackupFilePath);
					}
					else if (!Directory.Exists(folder.BackupFilePath))
					{
						if (folder.OriginalPath != null && !folder.OriginalPath.Contains("projects.xml"))
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
							File.Copy(folder.OriginalPath, folder.BackupFilePath, true);
						}
						CopyFiles(folder);
					}
					else
					{
						if (Directory.Exists(folder.OriginalPath))
						{
							CopyFiles(folder);
						}

						if (File.Exists(folder.OriginalPath))
						{
							File.Copy(folder.OriginalPath, folder.BackupFilePath);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Logger.Error($"{Constants.FromSelectedLocations} {ex.Message}\n {ex.StackTrace}");
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
				var files = Directory.GetFiles(folder.OriginalPath);
				if (files.Length > 0)
				{
					MoveToBackUp(files, folder.BackupFilePath);
				}

				// Check for subdirectories
				var subdirectories = Directory.GetDirectories(folder.OriginalPath);
				foreach (var subdirectory in subdirectories)
				{
					var currentDirInfo = new DirectoryInfo(subdirectory);
					CheckForSubfolders(currentDirInfo, folder.BackupFilePath);
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"{Constants.CopyFiles} {ex.Message}\n {ex.StackTrace}");
			}
		}

	    private static void CheckForSubfolders(DirectoryInfo currentDirInfo, string backupFolderRoot)
	    {
			try
			{
				var subdirectories = currentDirInfo.GetDirectories();
				if (currentDirInfo.Parent != null)
				{
					var pathToCorrespondingBackupFolder = Path.Combine(backupFolderRoot, currentDirInfo.Name);
					var subDirectoryFiles = Directory.GetFiles(currentDirInfo.FullName);
					if (subDirectoryFiles.Length > 0)
					{
						if (!Directory.Exists(pathToCorrespondingBackupFolder))
						{
							Directory.CreateDirectory(pathToCorrespondingBackupFolder);
						}
						MoveToBackUp(subDirectoryFiles, pathToCorrespondingBackupFolder);
					}
					if (subdirectories.Length > 0)
					{
						foreach (var subDirectory in subdirectories)
						{
							CheckForSubfolders(subDirectory, pathToCorrespondingBackupFolder);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Logger.Error($"{Constants.CheckForSubfolders} {ex.Message}\n {ex.StackTrace}");
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
				    Logger.Error($"{Constants.MoveToBackUp} {ex.Message}\n {ex.StackTrace}");
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
				Logger.Error($"{Constants.RemoveDirectoryInfo} {ex.Message}\n {ex.StackTrace}");
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