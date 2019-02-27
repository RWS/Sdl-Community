using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class Remove
	{
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
			foreach (var folder in foldersToBackup)
			{
				if (!folder.BackupFilePath.Contains("projects.xml"))
				{
					//creates original folders if doesn't exist
					if (!Directory.Exists(folder.OriginalFilePath))
					{
						Directory.CreateDirectory(folder.OriginalFilePath);
					}
					try
					{
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
					catch (Exception e)
					{
					}

				}
				else
				{
					File.Copy(folder.BackupFilePath, folder.OriginalFilePath, true);
				}
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
					    await Task.Run(()=>Empty(directoryInfo));
				    }
			    }
		    }
		    catch (Exception e)
		    {
			    throw e;
		    }
	    }

	    private static void CreateBackupFolder(List<LocationDetails> foldersToBackup)
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
			    }
			    else
			    {
				    try
				    {
					    if (Directory.Exists(folder.OriginalFilePath))
					    {
						    //Get files 
						    var files = Directory.GetFiles(folder.OriginalFilePath);
						    if (files.Length > 0)
						    {
							    MoveToBackUp(files, folder.BackupFilePath);
						    }

						    //check for subdirectories
						    var subdirectories = Directory.GetDirectories(folder.OriginalFilePath);
						    foreach (var subdirectory in subdirectories)
						    {
							    var currentDirInfo = new DirectoryInfo(subdirectory);
							    CheckForSubfolders(currentDirInfo, folder.BackupFilePath);
						    }
					    }
				    }
				    catch (Exception e)
				    {
				    }
			    }
		    }
	    }

	    private static void CheckForSubfolders(DirectoryInfo currentDirInfo, string backupFolderRoot)
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
			    catch (Exception e)
			    {
			    }
		    }
	    }
	    private static void Empty(DirectoryInfo directoryInfo)
	    {
		    if (directoryInfo.Exists)
		    {
			    SetAttributesNormal(directoryInfo);
			    directoryInfo.Delete(true);
			}
			
		}

	    private static void SetAttributesNormal(DirectoryInfo directory)
	    {
			foreach (var subDir in directory.GetDirectories())
				SetAttributesNormal(subDir);
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
