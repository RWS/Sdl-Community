﻿using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace Sdl.Community.BackupFiles
{
	public class BackupService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public void BackupFilesRecursive(string trimmedBackupName)
		{
			try
			{
				var service = new Service();
				var jsonResult = service.GetJsonInformation();
				var backupModel =
					jsonResult?.BackupModelList?.FirstOrDefault(b => b.TrimmedBackupName.Equals(trimmedBackupName));
				var backupModelList = jsonResult?.BackupDetailsModelList
					?.Where(b => b.TrimmedBackupName.Equals(trimmedBackupName)).ToList();
				if (backupModel != null)
				{
					var fileExtensions = new List<string>();

					if (backupModelList != null)
					{
						foreach (var fileExtension in backupModelList)
						{
							fileExtensions.Add(fileExtension.BackupPattern);
						}
					}

					var splittedSourcePathList = backupModel.BackupFrom.Split(';').ToList<string>();
					var files = new List<string>().ToArray();

					foreach (var sourcePath in splittedSourcePathList)
					{
						if (!string.IsNullOrEmpty(sourcePath))
						{
							// create the directory where to move files
							Directory.CreateDirectory(backupModel.BackupTo);

							// take files depending on defined action
							if (fileExtensions.Any())
							{
								// get all files which have extension set up depending on actions from TMBackupDetails grid
								files = Directory.GetFiles(sourcePath, "*.*")
									.Where(f => fileExtensions
										.Contains(Path.GetExtension(f)))
									.ToArray();
							}
							else
							{
								// take all files
								files = Directory.GetFiles(sourcePath);
							}

							if (files.Length != 0)
							{
								MoveFilesToAcceptedFolder(files, backupModel.BackupTo);
							} //that means we have a subfolder in watch folder
							else
							{
								var subdirectories = Directory.GetDirectories(sourcePath);
								foreach (var subdirectory in subdirectories)
								{
									var currentDirInfo = new DirectoryInfo(subdirectory);
									CheckForSubfolders(currentDirInfo, backupModel.BackupTo, trimmedBackupName);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"{Constants.BackupFilesRecursive} {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void CheckForSubfolders(DirectoryInfo directory, string root, string trimmedBackupName)
		{
			var service = new Service();
			var jsonResult = service.GetJsonInformation();
			var backupModel = jsonResult?.BackupModelList?.FirstOrDefault(b => b.TrimmedBackupName.Equals(trimmedBackupName));
			if (backupModel != null)
			{
				var subdirectories = directory.GetDirectories();
				var path = root + @"\" + directory.Parent;
				var subdirectoryFiles = Directory.GetFiles(directory.FullName);

				if (subdirectoryFiles.Length != 0)
				{
					MoveFilesToAcceptedFolder(subdirectoryFiles, path);
				}

				if (subdirectories.Length != 0)
				{
					foreach (var subdirectory in subdirectories)
					{
						CheckForSubfolders(subdirectory, path, trimmedBackupName);
					}
				}
			}
		}

		private void MoveFilesToAcceptedFolder(string[] files, string acceptedFolderPath)
		{
			foreach (var subFile in files)
			{
				var parentName = new DirectoryInfo(subFile).Parent != null ? new DirectoryInfo(subFile).Parent.Name : string.Empty;

				var fileName = subFile.Substring(subFile.LastIndexOf(@"\", StringComparison.Ordinal));
				var destinationPath = Path.Combine(acceptedFolderPath, parentName);
				if (!Directory.Exists(destinationPath))
				{
					Directory.CreateDirectory(destinationPath);
				}
				try
				{
					File.Copy(subFile, destinationPath + fileName, true);
				}
				catch (Exception ex)
				{
					Logger.Error($"{Constants.MoveFilesToAcceptedFolder} {ex.Message} \n {ex.StackTrace}");
				}
			}
		}
	}
}