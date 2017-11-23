using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.IO;
using static Sdl.Community.BackupService.Helpers.Enums;
using System.Globalization;

namespace Sdl.Community.BackupService
{
	public class Service
	{
		public JsonRequestModel GetJsonInformation()
		{
			Persistence persistence = new Persistence();
			JsonRequestModel result = persistence.ReadFormInformation();

			return result;
		}

		// Create task scheduler for the backup files process.
		public void CreateTaskScheduler(bool isWindowsInitialize)
		{
			var jsonRequestModel = GetJsonInformation();

			DateTime startDate = DateTime.Now;

			if (jsonRequestModel != null && jsonRequestModel.ChangeSettingsModel != null)
			{
				// Create a new task definition for the local machine and assign properties
				TaskDefinition td = TaskService.Instance.NewTask();
				td.RegistrationInfo.Description = "Backup files";

				if (jsonRequestModel.ChangeSettingsModel.IsRealTimeOptionChecked && jsonRequestModel.RealTimeBackupModel != null)
				{
					AddRealTimeScheduler(jsonRequestModel, startDate, td, isWindowsInitialize);
				}

				else if (jsonRequestModel.ChangeSettingsModel.IsPeriodicOptionChecked && jsonRequestModel.PeriodicBackupModel != null)
				{
					AddPeriodicTimeScheduler(jsonRequestModel, startDate, td, isWindowsInitialize);
				}

				else
				{   // To do: implement manually scheduler				
					// Get info from the UI to manually start scheduler					
					using (TaskService ts = new TaskService())
					{
						//td.Actions.Add(new ExecAction("Sdl.Community.TmBackup.BackupFilesExe.Sdl.Community.BackupFiles.exe"), "Daily"));
						td.Actions.Add(new ExecAction(Path.Combine(@"C:\Repos\Sdl.Community.TMBackup\Sdl.Community.TMBackup\Sdl.Community.BackupFiles\bin\Debug", "Sdl.Community.BackupFiles.exe"), "Daily"));
						try
						{
							ts.RootFolder.RegisterTaskDefinition("DailyScheduler", td);
						}
						catch (Exception ex)
						{
							MessageLogger.LogFileMessage(ex.Message);
						}
					}
				}
			}
		}

		// Add trigger which executes the backup files console application.
		private void AddTrigger(DailyTrigger daily, DateTime startDate, TaskDefinition td, bool isWindowsInitialize)
		{
			using (TaskService ts = new TaskService())
			{
				daily.StartBoundary = startDate;
				td.Triggers.Add(daily);

				if (isWindowsInitialize)
				{
					//td.Actions.Add(new ExecAction("Sdl.Community.TmBackup.BackupFilesExe.Sdl.Community.BackupFiles.exe"), "WindowsInitialize"));
					td.Actions.Add(new ExecAction(Path.Combine(@"C:\Repos\Sdl.Community.TMBackup\Sdl.Community.TMBackup\Sdl.Community.BackupFiles\bin\Debug", "Sdl.Community.BackupFiles.exe"), "WindowsInitialize"));
				}
				else
				{
					// above line used for deploy
					//td.Actions.Add(new ExecAction("Sdl.Community.TmBackup.BackupFilesExe.Sdl.Community.BackupFiles.exe"), "Daily"));
					td.Actions.Add(new ExecAction(Path.Combine(@"C:\Repos\Sdl.Community.TMBackup\Sdl.Community.TMBackup\Sdl.Community.BackupFiles\bin\Debug", "Sdl.Community.BackupFiles.exe"), "Daily"));
				}

				try
				{
					ts.RootFolder.RegisterTaskDefinition("DailyScheduler", td);
				}
				catch (Exception ex)
				{
					MessageLogger.LogFileMessage(ex.Message);
				}
			}
		}

		// Add real time scheduler for options: seconds, minutes, hours.
		private void AddRealTimeScheduler(JsonRequestModel jsonRequestModel, DateTime startDate, TaskDefinition td, bool isWindowsInitialize)
		{
			DailyTrigger daily = new DailyTrigger();

			if (jsonRequestModel.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Hours)))
			{
				startDate = startDate.AddHours(jsonRequestModel.RealTimeBackupModel.BackupInterval);

				using (TaskService ts = new TaskService())
				{
					AddTrigger(daily, startDate, td, isWindowsInitialize);
				}
			}

			if (jsonRequestModel.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Minutes)))
			{
				startDate = startDate.AddMinutes(jsonRequestModel.RealTimeBackupModel.BackupInterval);

				using (TaskService ts = new TaskService())
				{
					AddTrigger(daily, startDate, td, isWindowsInitialize);
				}
			}

			if (jsonRequestModel.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Seconds)))
			{
				startDate = startDate.AddSeconds(jsonRequestModel.RealTimeBackupModel.BackupInterval);

				using (TaskService ts = new TaskService())
				{
					AddTrigger(daily, startDate, td, isWindowsInitialize);
				}
			}
		}

		// Add periodic time scheduler depending on user setup.
		private void AddPeriodicTimeScheduler(JsonRequestModel jsonRequestModel, DateTime startDate, TaskDefinition td, bool isWindowsInitialize)
		{
			DailyTrigger daily = new DailyTrigger();

			if (jsonRequestModel.PeriodicBackupModel.IsRunOption)
			{
				startDate = DateTime.Now;
				using (TaskService ts = new TaskService())
				{
					AddTrigger(daily, startDate, td, isWindowsInitialize);
				}
			}
			else
			{
				DateTime atScheduleTime = DateTime.Parse(jsonRequestModel.PeriodicBackupModel.BackupAt, CultureInfo.InvariantCulture);
				startDate = jsonRequestModel.PeriodicBackupModel.FirstBackup.Date + new TimeSpan(atScheduleTime.Hour, atScheduleTime.Minute, atScheduleTime.Second);

				if (jsonRequestModel.PeriodicBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Hours)))
				{
					startDate = startDate.AddHours(jsonRequestModel.PeriodicBackupModel.BackupInterval);

					using (TaskService ts = new TaskService())
					{
						AddTrigger(daily, startDate, td, isWindowsInitialize);
					}
				}

				if (jsonRequestModel.PeriodicBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Minutes)))
				{
					startDate = startDate.AddMinutes(jsonRequestModel.PeriodicBackupModel.BackupInterval);
					using (TaskService ts = new TaskService())
					{
						AddTrigger(daily, startDate, td, isWindowsInitialize);
					}
				}

				if (jsonRequestModel.PeriodicBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Seconds)))
				{
					startDate = startDate.AddSeconds(jsonRequestModel.PeriodicBackupModel.BackupInterval);

					using (TaskService ts = new TaskService())
					{
						AddTrigger(daily, startDate, td, isWindowsInitialize);
					}
				}
			}
		}
	}
}