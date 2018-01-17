using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupChangeForm : Form
	{
		#region Private fields
		private bool _isNewTask;
		#endregion

		#region Constructors
		public TMBackupChangeForm()
		{
			InitializeComponent();
		}

		public TMBackupChangeForm(bool isNewTask)
		{
			InitializeComponent();

			_isNewTask = isNewTask;

			if (!isNewTask)
			{
				InitializeFormInfo();
			}
		}
		#endregion

		#region Events
		private void btn_TimeDetails_Click(object sender, EventArgs e)
		{
			PeriodicBackupForm periodicBackupForm = new PeriodicBackupForm();
			periodicBackupForm.ShowDialog();
		}
		
		private void radioBtn_TimeChange_CheckedChanged(object sender, EventArgs e)
		{
			btn_TimeDetails.Enabled = true;

			ChangeSettingsModel changeSettingModel = new ChangeSettingsModel();
			changeSettingModel.IsPeriodicOptionChecked = radioBtn_TimeChange.Checked;

			Persistence persistence = new Persistence();
			persistence.SaveChangeSettings(changeSettingModel);
		}

		private void radioBtn_Manually_CheckedChanged(object sender, EventArgs e)
		{
			btn_TimeDetails.Enabled = false;

			ChangeSettingsModel changeSettingModel = new ChangeSettingsModel();
			changeSettingModel.IsManuallyOptionChecked = radioBtn_Manually.Checked;

			Persistence persistence = new Persistence();
			persistence.SaveChangeSettings(changeSettingModel);
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			GetBackupTimeInfo();
			Close();
		}
		#endregion

		#region Methods
		private void InitializeFormInfo()
		{
			Persistence persistence = new Persistence();
			var result = persistence.ReadFormInformation();

			if (result != null)
			{
				radioBtn_TimeChange.Checked = result.ChangeSettingsModel != null ? result.ChangeSettingsModel.IsPeriodicOptionChecked : false;
				radioBtn_Manually.Checked = result.ChangeSettingsModel != null ? result.ChangeSettingsModel.IsManuallyOptionChecked : false;
			}
		}

		public string GetBackupTimeInfo()
		{
			string backupTimeInfo = string.Empty;

			Persistence persistence = new Persistence();
			var jsonResult = persistence.ReadFormInformation();

			if (jsonResult != null && jsonResult.PeriodicBackupModel != null && radioBtn_TimeChange.Checked)
			{
				backupTimeInfo = backupTimeInfo + "Backup interval: " + jsonResult.PeriodicBackupModel.BackupInterval + " " +
					jsonResult.PeriodicBackupModel.TimeType + ", " + "First backup on: " +
					jsonResult.PeriodicBackupModel.FirstBackup + ", " + "at " +
					jsonResult.PeriodicBackupModel.BackupAt + ", ";
			}
			else if (radioBtn_Manually.Checked)
			{
				backupTimeInfo = Constants.ManuallyOption;
			}
			return backupTimeInfo;
		}
		#endregion
	}
}