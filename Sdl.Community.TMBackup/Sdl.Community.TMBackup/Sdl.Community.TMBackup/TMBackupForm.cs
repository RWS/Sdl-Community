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

			Service service = new Service();
			service.CreateTaskScheduler();
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
	}
}