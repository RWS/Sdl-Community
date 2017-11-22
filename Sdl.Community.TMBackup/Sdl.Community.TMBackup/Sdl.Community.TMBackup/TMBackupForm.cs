using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using static Sdl.Community.BackupService.Helpers.Enums;
using System.IO;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupForm : Form
	{
		public TMBackupForm()
		{
			InitializeComponent();

			GetBackupFormInfo();
		}

		private void btn_BackupFrom_Click(object sender, EventArgs e)
		{
			var fromFolderDialog = new FolderSelectDialog();
			txt_BackupFrom.Text = string.Empty;

			if (fromFolderDialog.ShowDialog())
			{
				if (fromFolderDialog.Files.Any())
				{
					foreach (var folderName in fromFolderDialog.Files)
					{
						txt_BackupFrom.Text = txt_BackupFrom.Text + folderName + ";";
					}
					txt_BackupFrom.Text.Remove(txt_BackupFrom.Text.Length - 1);
				}
			}
		}

		private void btn_BackupTo_Click(object sender, EventArgs e)
		{
			var toFolderDialog = new FolderSelectDialog();

			if (toFolderDialog.ShowDialog())
			{
				txt_BackupTo.Text = toFolderDialog.FileName;
			}
		}

		private void btn_Change_Click(object sender, EventArgs e)
		{
			TMBackupChangeForm changeForm = new TMBackupChangeForm();
			changeForm.ShowDialog();

			txt_BackupTime.Text = changeForm.GetBackupTimeInfo();
		}

		private void btn_Details_Click(object sender, EventArgs e)
		{
			TMBackupDetailsForm detailsForm = new TMBackupDetailsForm();
			detailsForm.ShowDialog();

			txt_BackupDetails.Text = TMBackupDetailsForm.BackupDetailsInfo;

			GetBackupFormInfo();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btn_SaveSettings_Click(object sender, EventArgs e)
		{
			BackupModel backupModel = new BackupModel();
			backupModel.BackupFrom = txt_BackupFrom.Text;
			backupModel.BackupTo = txt_BackupTo.Text;
			backupModel.Description = txt_Description.Text;
			backupModel.BackupDetails = txt_BackupDetails.Text;
			backupModel.BackupTime = txt_BackupTime.Text;

			Persistence persistence = new Persistence();
			persistence.SaveBackupFormInfo(backupModel);

			this.Close();

			CreateTaskScheduler();
		}

		private void GetBackupFormInfo()
		{
			Persistence persistence = new Persistence();
			var result = persistence.ReadFormInformation();

			if (result.BackupModel != null)
			{
				txt_BackupFrom.Text = result.BackupModel.BackupFrom;
				txt_BackupTo.Text = result.BackupModel.BackupTo;
				txt_BackupTime.Text = result.BackupModel.BackupTime;
				txt_Description.Text = result.BackupModel.Description;
			}

			if (result.BackupDetailsModelList != null)
			{
				string res = string.Empty;
				foreach (var backupDetail in result.BackupDetailsModelList)
				{
					res = res + backupDetail.BackupAction + ", " + backupDetail.BackupType + ", " + backupDetail.BackupPattern + ";  ";
				}
				txt_BackupDetails.Text = res;
			}

			TMBackupChangeForm tmBackupChangeForm = new TMBackupChangeForm();
			txt_BackupTime.Text = tmBackupChangeForm.GetBackupTimeInfo();
		}

		private void CreateTaskScheduler()
		{
			Persistence persistence = new Persistence();
			var jsonRequestModel = persistence.ReadFormInformation();

			DateTime startDate = DateTime.Now;

			if (jsonRequestModel != null)
			{
				// Create a new task definition for the local machine and assign properties
				TaskDefinition td = TaskService.Instance.NewTask();
				td.RegistrationInfo.Description = "Backup files";

				if (jsonRequestModel.ChangeSettingsModel.IsRealTimeOptionChecked && jsonRequestModel.RealTimeBackupModel != null)
				{
					DailyTrigger daily = new DailyTrigger();

					if (jsonRequestModel.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Hours)))
					{
						startDate = startDate.AddHours(jsonRequestModel.RealTimeBackupModel.BackupInterval);

						using (TaskService ts = new TaskService())
						{
							AddTrigger(daily, startDate, td);
						}
					}

					if (jsonRequestModel.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Minutes)))
					{
						startDate = startDate.AddMinutes(jsonRequestModel.RealTimeBackupModel.BackupInterval);

						using (TaskService ts = new TaskService())
						{
							AddTrigger(daily, startDate, td);
						}
					}

					if (jsonRequestModel.RealTimeBackupModel.TimeType.Equals(Enums.GetDescription(TimeTypes.Seconds)))
					{
						startDate = startDate.AddSeconds(jsonRequestModel.RealTimeBackupModel.BackupInterval);

						using (TaskService ts = new TaskService())
						{
							AddTrigger(daily, startDate, td);
						}
					}
				}

				else if (jsonRequestModel.ChangeSettingsModel.IsPeriodicOptionChecked)
				{

				}

				else
				{

				}
			}
		}
		
		private void AddTrigger(DailyTrigger daily, DateTime startDate, TaskDefinition td)
		{
			using (TaskService ts = new TaskService())
			{
				daily.StartBoundary = startDate;
				td.Triggers.Add(daily);

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