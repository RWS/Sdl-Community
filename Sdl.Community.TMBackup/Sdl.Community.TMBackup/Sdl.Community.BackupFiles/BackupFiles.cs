using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using static Sdl.Community.BackupService.Helpers.Enums;

namespace Sdl.Community.BackupFiles
{
	public class BackupFiles
	{
		static void Main(string[] args)
		{
			BackupFilesRecursive();

			if (args[0].Equals("Daily"))
			{
				UpdateDailyTaskTrigger();
			}
		}

		#region Private methods
		private static string GetAcceptedRequestsFolder(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		private static void BackupFilesRecursive()
		{
			Service service = new Service();
			var jsonResult = service.GetJsonInformation();

			List<string> splittedSourcePathList = jsonResult.BackupModel.BackupFrom.Split(';').ToList<string>();

			foreach (var sourcePath in splittedSourcePathList)
			{
				if (!string.IsNullOrEmpty(sourcePath))
				{
					var acceptedRequestFolder = GetAcceptedRequestsFolder(sourcePath);

					// create the directory "Accepted request"
					if (!Directory.Exists(jsonResult.BackupModel.BackupTo))
					{
						Directory.CreateDirectory(jsonResult.BackupModel.BackupTo);
					}

					var files = Directory.GetFiles(sourcePath);
					if (files.Length != 0)
					{
						MoveFilesToAcceptedFolder(files, jsonResult.BackupModel.BackupTo);
					} //that means we have a subfolder in watch folder
					else
					{
						var subdirectories = Directory.GetDirectories(sourcePath);
						foreach (var subdirectory in subdirectories)
						{
							var currentDirInfo = new DirectoryInfo(subdirectory);
							CheckForSubfolders(currentDirInfo, jsonResult.BackupModel.BackupTo);
						}
					}
				}
			}
		}

		private static void CheckForSubfolders(DirectoryInfo directory, string root)
		{
			Service service = new Service();
			var jsonResult = service.GetJsonInformation();

			var sourcePath = jsonResult.BackupModel.BackupFrom;
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
					CheckForSubfolders(subdirectory, path);
				}
			}
		}

		private static void MoveFilesToAcceptedFolder(string[] files, string acceptedFolderPath)
		{
			foreach (var subFile in files)
			{
				var dirName = new DirectoryInfo(subFile).Name;
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
				catch (Exception e)
				{
					MessageBox.Show("Files were not copied correctly. Please try again!", "Informative message");
				}
			}
		}

		private static void UpdateDailyTaskTrigger()
		{
			var startDate = DateTime.Now;
			Service service = new Service();
			var jsonResult = service.GetJsonInformation();

			if (jsonResult.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Hours)))
			{
				startDate = startDate.AddHours(jsonResult.RealTimeBackupModel.BackupInterval);
				UpdateTaskByInterval(startDate);
			}

			if (jsonResult.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Minutes)))
			{
				startDate = startDate.AddMinutes(jsonResult.RealTimeBackupModel.BackupInterval);
				UpdateTaskByInterval(startDate);
			}

			if (jsonResult.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Seconds)))
			{
				startDate = startDate.AddSeconds(jsonResult.RealTimeBackupModel.BackupInterval);
				UpdateTaskByInterval(startDate);
			}
		}

		private static void UpdateTaskByInterval(DateTime startDate)
		{
			using (TaskService ts = new TaskService())
			{
				Task task = ts.GetTask("DailyScheduler");
				TaskDefinition td = task.Definition;
				foreach (Trigger trigger in task.Definition.Triggers)
				{
					trigger.StartBoundary = startDate;
				}
				ts.RootFolder.RegisterTaskDefinition("DailyScheduler", td);
			}
		}
		#endregion
	}
}
