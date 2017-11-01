using Sdl.Community.TMBackup.Models;
using System;
using System.Windows.Forms;

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
	}
}