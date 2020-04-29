using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using static Sdl.Community.BackupService.Helpers.Enums;

namespace Sdl.Community.BackupService
{
	public class Service
	{
		public static readonly Log Log = Log.Instance;

		public JsonRequestModel GetJsonInformation()
		{
			var persistence = new Persistence();
			var result = persistence.ReadFormInformation();
			return result;
		}

		// Create task scheduler for the backup files process.
		public void CreateTaskScheduler(string backupName, bool isStartedManually)
		{
			var jsonRequestModel = GetJsonInformation();
			var changeSettingsModelItem = jsonRequestModel?.ChangeSettingsModelList?.FirstOrDefault(c => c.BackupName.Equals(backupName));

			var tr = Trigger.CreateTrigger(TaskTriggerType.Time);

			if (jsonRequestModel != null && changeSettingsModelItem != null)
			{
				// Create a new task definition for the local machine and assign properties
				var td = TaskService.Instance.NewTask();
				td.RegistrationInfo.Description = "Backup files";

				if (changeSettingsModelItem.IsPeriodicOptionChecked)
				{
					AddPeriodicTimeScheduler(jsonRequestModel, td, tr, backupName);
				}
				if (changeSettingsModelItem.IsManuallyOptionChecked)
				{
					AddManuallyTimeScheduler(td, tr, backupName, changeSettingsModelItem.TrimmedBackupName, isStartedManually);
				}
			}
		}

		public void AddManuallyTimeScheduler(TaskDefinition td, Trigger tr, string backupName, string trimmedBackupName, bool isStartedManually)
		{
			if (!isStartedManually)
			{
				tr.Enabled = false;
			}
			else
			{
				tr.StartBoundary = DateTime.Now.Date + new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second + 10);
			}
			AddTrigger(tr, td, backupName, trimmedBackupName);
		}

		// Add trigger which executes the backup files console application.
		private void AddTrigger(Trigger trigger, TaskDefinition td, string backupName, string trimmedBackupName)
		{
			using (var ts = new TaskService())
			{
				td.Triggers.Add(trigger);

				td.Actions.Add(new ExecAction(Path.Combine(Constants.DeployPath, "Sdl.Community.BackupFiles.exe"), trimmedBackupName));

				try
				{
					ts.RootFolder.RegisterTaskDefinition(string.Concat(Constants.TaskDetailValue, backupName), td);
				}
				catch (Exception ex)
				{
					Log.Logger.Error($"{ Constants.AddTrigger} {ex.Message} \n {ex.StackTrace}");
				}
			}
		}

		// Add periodic time scheduler depending on user setup.
		private void AddPeriodicTimeScheduler(JsonRequestModel jsonRequestModel, TaskDefinition td, Trigger tr, string backupName)
		{
			var periodicBackupModel = jsonRequestModel?.PeriodicBackupModelList?.FirstOrDefault(p => p.BackupName.Equals(backupName));
			if (periodicBackupModel != null)
			{
				var atScheduleTime = DateTime.Parse(periodicBackupModel.BackupAt, CultureInfo.InvariantCulture);
				tr.StartBoundary = periodicBackupModel.FirstBackup.Date + new TimeSpan(atScheduleTime.Hour, atScheduleTime.Minute, atScheduleTime.Second);

				SetupRealDateTime(tr, periodicBackupModel.IsNowPressed);

				if (periodicBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Hours)))
				{
					tr.Repetition.Interval = TimeSpan.FromHours(periodicBackupModel.BackupInterval);
					AddTrigger(tr, td, backupName, periodicBackupModel.TrimmedBackupName);
				}

				if (periodicBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Minutes)))
				{
					tr.Repetition.Interval = TimeSpan.FromMinutes(periodicBackupModel.BackupInterval);
					AddTrigger(tr, td, backupName, periodicBackupModel.TrimmedBackupName);
				}
			}
		}

		// Method used in order to start trigger at the current date time when Now button is pressed in the Periodic window.
		// The 10 seconds are added as a short delay to ensure that the backup is done at the current date time after the Main window is closed.
		private void SetupRealDateTime(Trigger tr, bool isNowPressed)
		{
			if (isNowPressed)
			{
				tr.StartBoundary = DateTime.UtcNow.AddSeconds(10);
			}
		}
	}
}