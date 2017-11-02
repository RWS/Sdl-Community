using Sdl.Community.TMBackup.Models;
using System;
using System.Windows.Forms;
using Sdl.Community.TMBackup.Helpers;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupChangeForm : Form
	{
		public TMBackupChangeForm()
		{
			InitializeComponent();

			InitializeFormInfo();
		}

		private void btn_RealTimeDetails_Click(object sender, EventArgs e)
		{
			RealTimeParametersForm realTimeParamform = new RealTimeParametersForm();
			realTimeParamform.ShowDialog();
		}

		private void btn_TimeDetails_Click(object sender, EventArgs e)
		{
			PeriodicBackupForm periodicBackupForm = new PeriodicBackupForm();
			periodicBackupForm.ShowDialog();
		}

		private void radioBtn_RealTimeChange_CheckedChanged(object sender, EventArgs e)
		{
			btn_TimeDetails.Enabled = false;
			btn_RealTimeDetails.Enabled = true;

			ChangeSettingsModel changeSettingModel = new ChangeSettingsModel();
			changeSettingModel.IsRealTimeOptionChecked = radioBtn_RealTimeChange.Checked;

			Persistence persistence = new Persistence();
			persistence.SaveChangeSettings(changeSettingModel);
		}

		private void radioBtn_TimeChange_CheckedChanged(object sender, EventArgs e)
		{
			btn_RealTimeDetails.Enabled = false;
			btn_TimeDetails.Enabled = true;

			ChangeSettingsModel changeSettingModel = new ChangeSettingsModel();
			changeSettingModel.IsPeriodicOptionChecked = radioBtn_TimeChange.Checked;

			Persistence persistence = new Persistence();
			persistence.SaveChangeSettings(changeSettingModel);
		}

		private void radioBtn_Manually_CheckedChanged(object sender, EventArgs e)
		{
			btn_RealTimeDetails.Enabled = false;
			btn_TimeDetails.Enabled = false;

			ChangeSettingsModel changeSettingModel = new ChangeSettingsModel();
			changeSettingModel.IsManuallyOptionChecked = radioBtn_Manually.Checked;

			Persistence persistence = new Persistence();
			persistence.SaveChangeSettings(changeSettingModel);
		}

		private void InitializeFormInfo()
		{
			Persistence persistence = new Persistence();
			var result = persistence.ReadFormInformation();

			if(result != null)
			{
				radioBtn_RealTimeChange.Checked = result.ChangeSettingsModel != null ? result.ChangeSettingsModel.IsRealTimeOptionChecked : false;
				radioBtn_TimeChange.Checked = result.ChangeSettingsModel != null ?  result.ChangeSettingsModel.IsPeriodicOptionChecked : false;
				radioBtn_Manually.Checked = result.ChangeSettingsModel != null ? result.ChangeSettingsModel.IsManuallyOptionChecked : false;
			}
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			GetBackupTimeInfo();
			this.Close();
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

				if (jsonResult.PeriodicBackupModel.IsRunOption)
				{
					backupTimeInfo = backupTimeInfo + Constants.RunOption;
				}
				else
				{
					backupTimeInfo = backupTimeInfo + Constants.WaitOption;
				}

			}
			else if (jsonResult != null && jsonResult.RealTimeBackupModel != null && radioBtn_RealTimeChange.Checked)
			{
				backupTimeInfo = "At: " + jsonResult.RealTimeBackupModel.BackupInterval + " " + jsonResult.RealTimeBackupModel.TimeType;
			}
			else if (radioBtn_Manually.Checked)
			{
				backupTimeInfo = Constants.ManuallyOption;
			}
			return backupTimeInfo;
		}
	}
}