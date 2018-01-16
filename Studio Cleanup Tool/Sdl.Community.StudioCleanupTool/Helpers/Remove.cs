using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioCleanupTool.Model;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
    public static class Remove
    {
	    private static string _backupFolderPath =
		    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup");
	    public static async Task BackupFiles(List<StudioDetails> foldersToBackup)
	    {
		    await Task.Run(() => CreateBackupFolder(foldersToBackup));
	    }
		
	    public static async Task FromSelectedLocations(List<StudioDetails> foldersToRemove)
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
			    
		    }
	    }

	    private static void CreateBackupFolder(List<StudioDetails> foldersToBackup)
	    {
			    foreach (var folder in foldersToBackup)
			    {
				    if (!Directory.Exists(folder.BackupFilePath))
				    {
					    Directory.CreateDirectory(folder.BackupFilePath);
				    }
				    try
				    {
					    //Get files 
					    var files = Directory.GetFiles(folder.OriginalFilePath);
					    if (files.Length > 0)
					    {
						    MoveToBackUp(files, folder.BackupFilePath);
					    }
					    else
					    {
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
					    throw e;
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
				    throw e;
			    }
		    }
	    }
	    private static void Empty(DirectoryInfo directoryInfo)
	    {
			//removes all files from root directory
		    foreach (var file in directoryInfo.GetFiles())
		    {
			    file.Delete();
		    }

			//removes all the directories from root directory
		    foreach (var directory in directoryInfo.GetDirectories())
		    {
			    directory.Delete(true);
		    }
	    }

	    private static bool IsDirectory(string path)
	    {
		    var attributes = File.GetAttributes(path);
		    return attributes.HasFlag(FileAttributes.Directory);
	    }
    }
}
