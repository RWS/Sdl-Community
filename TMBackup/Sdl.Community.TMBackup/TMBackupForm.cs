using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupForm : Form
	{
		private string _taskName;
		private bool _isNewTask;

		public TMBackupForm(bool isNewTask, string taskName)
		{
			InitializeComponent();

			_taskName = taskName;
			_isNewTask = isNewTask;

			if (!isNewTask)
			{
				GetBackupFormInfo(taskName);
			}
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
			GetBackupFormInfo(_taskName);
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_SaveSettings_Click(object sender, EventArgs e)
		{
			txt_BackupFromError.Visible = string.IsNullOrEmpty(txt_BackupFrom.Text) ? true : false;
			txt_BackupToError.Visible = string.IsNullOrEmpty(txt_BackupTo.Text) ? true : false;
			txt_BackupNameError.Visible = string.IsNullOrEmpty(txt_BackupName.Text) ? true: false;

			if (!txt_BackupFromError.Visible && !txt_BackupToError.Visible && !txt_BackupNameError.Visible)
			{
				BackupModel backupModel = new BackupModel();
				backupModel.BackupName = string.Concat(Constants.TaskDetailValue, txt_BackupName.Text);
				backupModel.BackupFrom = txt_BackupFrom.Text;
				backupModel.BackupTo = txt_BackupTo.Text;
				backupModel.Description = txt_Description.Text;
				backupModel.BackupDetails = txt_BackupDetails.Text;
				backupModel.BackupTime = txt_BackupTime.Text;

				Persistence persistence = new Persistence();
				persistence.SaveBackupFormInfo(backupModel);

				Hide();

				Service service = new Service();
				service.CreateTaskScheduler();

				TMBackupTasksForm tmBackupTasksForm = new TMBackupTasksForm();
				tmBackupTasksForm.ShowDialog();
			}			
		}

		private void GetBackupFormInfo(string taskName)
		{
			Persistence persistence = new Persistence();
			var result = persistence.ReadFormInformation();

			if (result.BackupModel != null && !_isNewTask)
			{
				txt_BackupName.Text = result.BackupModel.BackupName;
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

			TMBackupChangeForm tmBackupChangeForm = new TMBackupChangeForm(_isNewTask);
			txt_BackupTime.Text = tmBackupChangeForm.GetBackupTimeInfo();
		}		
	}
}