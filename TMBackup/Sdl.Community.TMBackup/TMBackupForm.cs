using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupForm : Form
	{
		private string _taskName;
		private bool _isNewTask;

		private List<BackupModel> _backupModelList = new List<BackupModel>();

		public TMBackupForm(bool isNewTask, string taskName)
		{
			InitializeComponent();

			_taskName = taskName;
			_isNewTask = isNewTask;

			if (!isNewTask)
			{
				txt_BackupName.Enabled = false;
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
			var changeForm = new TMBackupChangeForm(_isNewTask, txt_BackupName.Text);
			changeForm.ShowDialog();
			
			txt_BackupTime.Text = changeForm.GetBackupTimeInfo();
		}

		private void btn_Details_Click(object sender, EventArgs e)
		{
			var detailsForm = new TMBackupDetailsForm(txt_BackupName.Text);
			detailsForm.ShowDialog();

			txt_BackupDetails.Text = TMBackupDetailsForm.BackupDetailsInfo;
			GetBackupFormInfo(txt_BackupName.Text);
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_SaveSettings_Click(object sender, EventArgs e)
		{
			var persistence = new Persistence();
			var service = new Service();

			if (_isNewTask)
			{
				txt_TaskNameError.Visible = CheckTask(txt_BackupName.Text);
				txt_BackupFromError.Visible = string.IsNullOrEmpty(txt_BackupFrom.Text) ? true : false;
				txt_BackupToError.Visible = string.IsNullOrEmpty(txt_BackupTo.Text) ? true : false;
				txt_BackupNameError.Visible = string.IsNullOrEmpty(txt_BackupName.Text) ? true : false;

				if (!txt_TaskNameError.Visible
					&& !txt_BackupFromError.Visible
					&& !txt_BackupToError.Visible
					&& !txt_BackupNameError.Visible)
				{
					var backupModel = new BackupModel();
					backupModel.BackupName = (txt_BackupName.Text);
					backupModel.TrimmedBackupName = string.Concat(txt_BackupName.Text.Where(c => !char.IsWhiteSpace(c)));

					SetBackupModelInfo(backupModel, persistence, service);
				}
			}
			else
			{
				//update settings
				var jsonRequestModel = persistence.ReadFormInformation();
				if(jsonRequestModel!=null)
				{
					var backupModel = jsonRequestModel.BackupModelList != null 
						? jsonRequestModel.BackupModelList.Where(b => b.BackupName.Equals(txt_BackupName.Text)).FirstOrDefault()
						: null;

					if (backupModel != null)
					{
						SetBackupModelInfo(backupModel, persistence, service);
					}
				}
			}
		}

		private void GetBackupFormInfo(string taskName)
		{
			var persistence = new Persistence();
			var result = persistence.ReadFormInformation();
			var backupModel = result != null
				? result.BackupModelList !=null
				? result.BackupModelList.Count > 0
				? result.BackupModelList[0] != null
				? result.BackupModelList.Where(b => b.BackupName.Equals(taskName)).FirstOrDefault()
				: null : null : null : null;

			if (backupModel != null && !_isNewTask)
			{
				txt_BackupName.Text = backupModel.BackupName;
				txt_BackupFrom.Text = backupModel.BackupFrom;
				txt_BackupTo.Text = backupModel.BackupTo;
				txt_BackupTime.Text = backupModel.BackupTime;
				txt_Description.Text = backupModel.Description;
			}

			if (result.BackupDetailsModelList != null)
			{
				var backupDetailsList = result.BackupDetailsModelList.Where(b => b.BackupName.Equals(txt_BackupName.Text)).ToList();
				string res = string.Empty;
				foreach (var backupDetail in backupDetailsList)
				{
					res = res + backupDetail.BackupAction + ", " + backupDetail.BackupType + ", " + backupDetail.BackupPattern + ";  ";
				}
				txt_BackupDetails.Text = res;
			}			
		}

		private bool CheckTask(string taskName)
		{
			var persistence = new Persistence();
			var result = persistence.ReadFormInformation();
			var backupModel = result != null
				? result.BackupModelList != null 
				? result.BackupModelList.Count > 0 
				? result.BackupModelList[0] != null
				? result.BackupModelList.Where(b => b.BackupName.Equals(taskName)).FirstOrDefault()
				: null :null :null : null;

			if (backupModel != null && backupModel.BackupName.Contains(taskName))
			{
				return true;
			}
			return false;
		}
		
		private void SetBackupModelInfo(BackupModel backupModel, Persistence persistence, Service service)
		{
			backupModel.BackupFrom = txt_BackupFrom.Text;
			backupModel.BackupTo = txt_BackupTo.Text;
			backupModel.Description = txt_Description.Text;
			backupModel.BackupDetails = txt_BackupDetails.Text;
			backupModel.BackupTime = txt_BackupTime.Text;
			_backupModelList.Add(backupModel);

			persistence.SaveBackupFormInfo(_backupModelList, _isNewTask);

			Hide();

			service.CreateTaskScheduler(backupModel.BackupName);

			var tmBackupTasksForm = new TMBackupTasksForm();
			tmBackupTasksForm.ShowDialog();
		}
	}
}