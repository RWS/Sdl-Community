using Sdl.Community.TMBackup.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.TMBackup.Models;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupForm : Form
	{
		public TMBackupForm()
		{
			InitializeComponent();
		}

		private void btn_BackupFrom_Click(object sender, EventArgs e)
		{
			var fromFolderDialog = new FolderSelectDialog();

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
		}

		private void btn_Details_Click(object sender, EventArgs e)
		{
			TMBackupDetailsForm detailsForm = new TMBackupDetailsForm();
			detailsForm.ShowDialog();

			txt_BackupDetails.Text = TMBackupDetailsForm.BackupDetailsInfo;
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
		}
	}
}