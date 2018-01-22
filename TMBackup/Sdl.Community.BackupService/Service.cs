using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.IO;
using static Sdl.Community.BackupService.Helpers.Enums;
using System.Globalization;
using System.Linq;

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
		public void CreateTaskScheduler(string backupName)
		{
			var jsonRequestModel = GetJsonInformation();
			var changeSettingsModelItem = jsonRequestModel != null ? jsonRequestModel.ChangeSettingsModelList != null ? jsonRequestModel.ChangeSettingsModelList.Where(c => c.BackupName.Equals(backupName)).FirstOrDefault() 
																   : null : null;
			var periodicBackupModelItem = jsonRequestModel != null ? jsonRequestModel.PeriodicBackupModelList != null ? jsonRequestModel.PeriodicBackupModelList.Where(c => c.BackupName.Equals(backupName)).FirstOrDefault() 
															       : null : null;

			DateTime startDate = DateTime.Now;
			var tr = Trigger.CreateTrigger(TaskTriggerType.Time);

			if (jsonRequestModel != null && changeSettingsModelItem != null && periodicBackupModelItem != null)
			{
				// Create a new task definition for the local machine and assign properties
				TaskDefinition td = TaskService.Instance.NewTask();
				td.RegistrationInfo.Description = "Backup files";

				if (changeSettingsModelItem.IsPeriodicOptionChecked)
				{
					AddPeriodicTimeScheduler(jsonRequestModel, startDate, td, tr, backupName);
				}
				if (changeSettingsModelItem.IsManuallyOptionChecked)
				{
					AddManuallyTimeScheduler(td, tr, jsonRequestModel, backupName);
				}
			}
		}

		// Add trigger which executes the backup files console application.
		private void AddTrigger(Trigger trigger, TaskDefinition td, string taskName)
		{
			using (TaskService ts = new TaskService())
			{
				td.Triggers.Add(trigger);

				td.Actions.Add(new ExecAction(Path.Combine(Constants.DeployPath, "Sdl.Community.BackupFiles.exe"), taskName));

				try
				{
					ts.RootFolder.RegisterTaskDefinition(taskName, td);
				}
				catch (Exception ex)
				{
					MessageLogger.LogFileMessage(ex.Message);
				}
			}
		}

		// Add periodic time scheduler depending on user setup.
		private void AddPeriodicTimeScheduler(JsonRequestModel jsonRequestModel, DateTime startDate, TaskDefinition td, Trigger tr, string backupName)
		{
			var periodicBackupModel = jsonRequestModel != null ? jsonRequestModel.PeriodicBackupModelList != null ? jsonRequestModel.PeriodicBackupModelList.Where(p => p.BackupName.Equals(backupName)).FirstOrDefault()
															   : null : null;
			if (periodicBackupModel != null)
			{
				DateTime atScheduleTime = DateTime.Parse(periodicBackupModel.BackupAt, CultureInfo.InvariantCulture);
				tr.StartBoundary = periodicBackupModel.FirstBackup.Date + new TimeSpan(atScheduleTime.Hour, atScheduleTime.Minute, atScheduleTime.Second);

				SetupRealDateTime(tr);

				if (periodicBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Hours)))
				{
					tr.Repetition.Interval = TimeSpan.FromHours(periodicBackupModel.BackupInterval);
					AddTrigger(tr, td, periodicBackupModel.BackupName);
				}

				if (periodicBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Minutes)))
				{
					tr.Repetition.Interval = TimeSpan.FromMinutes(periodicBackupModel.BackupInterval);
					AddTrigger(tr, td, periodicBackupModel.BackupName);
				}
			}
		}

		private void AddManuallyTimeScheduler(TaskDefinition td, Trigger tr, JsonRequestModel jsonRequestModel, string backupName)
		{
			tr.StartBoundary = DateTime.Now.Date + new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

			SetupRealDateTime(tr);

			tr.Repetition.Interval = TimeSpan.FromMinutes(2);
			tr.EndBoundary = DateTime.Now.AddMinutes(10); ;
			AddTrigger(tr, td, backupName);
		}

		// Method used in order to start trigger at the current date time when Now button is pressed in the Periodic window.
		// The 10 seconds are added as a short delay to ensure that the backup is done at the current date time after the Main window is closed.
		private void SetupRealDateTime(Trigger tr)
		{
			var dateTimeResult = DateTime.Compare(tr.StartBoundary, DateTime.UtcNow);

			if (dateTimeResult > 0)
			{
				tr.StartBoundary = DateTime.UtcNow.AddSeconds(10);
			}
		}
	}
}