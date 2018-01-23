using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupChangeForm : Form
	{
		#region Private fields
		private bool _isNewTask;
		private string _taskName;
		private List<ChangeSettingsModel> _changeSettingsModelList = new List<ChangeSettingsModel>();
		private List<PeriodicBackupModel> _periodicBackupModelList = new List<PeriodicBackupModel>();
		#endregion

		#region Constructors
		public TMBackupChangeForm()
		{
			InitializeComponent();
		}

		public TMBackupChangeForm(bool isNewTask, string taskName)
		{
			InitializeComponent();

			_isNewTask = isNewTask;
			_taskName = taskName;

			if (!isNewTask)
			{
				InitializeFormInfo();
			}
		}
		#endregion

		#region Events
		private void btn_TimeDetails_Click(object sender, EventArgs e)
		{
			var periodicBackupForm = new PeriodicBackupForm(_taskName);
			periodicBackupForm.ShowDialog();
		}
		
		private void radioBtn_TimeChange_CheckedChanged(object sender, EventArgs e)
		{
			btn_TimeDetails.Enabled = true;

			var changeSettingModel = new ChangeSettingsModel();
			changeSettingModel.IsPeriodicOptionChecked = radioBtn_TimeChange.Checked;
			changeSettingModel.BackupName = _taskName;
			changeSettingModel.TrimmedBackupName = string.Concat(_taskName.Where(c => !char.IsWhiteSpace(c)));
			_changeSettingsModelList.Add(changeSettingModel);

			var persistence = new Persistence();
			persistence.SaveChangeSettings(_changeSettingsModelList, _taskName);
		}

		private void radioBtn_Manually_CheckedChanged(object sender, EventArgs e)
		{
			btn_TimeDetails.Enabled = false;

			var changeSettingModel = new ChangeSettingsModel();
			changeSettingModel.IsManuallyOptionChecked = radioBtn_Manually.Checked;
			changeSettingModel.BackupName = _taskName;
			changeSettingModel.TrimmedBackupName = string.Concat(_taskName.Where(c => !char.IsWhiteSpace(c)));
			_changeSettingsModelList.Add(changeSettingModel);

			var persistence = new Persistence();
			persistence.SaveChangeSettings(_changeSettingsModelList, _taskName);
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
			var persistence = new Persistence();
			var jsonResult = persistence.ReadFormInformation();

			if (jsonResult != null && jsonResult.ChangeSettingsModelList != null && jsonResult.ChangeSettingsModelList[0] != null)
			{
				var changeSettingsModel = jsonResult.ChangeSettingsModelList.Where(c => c.BackupName.Equals(_taskName)).FirstOrDefault();

				radioBtn_TimeChange.Checked = changeSettingsModel != null ? changeSettingsModel.IsPeriodicOptionChecked : false;
				radioBtn_Manually.Checked = changeSettingsModel != null ? changeSettingsModel.IsManuallyOptionChecked : false;
			}
		}

		public string GetBackupTimeInfo()
		{
			string backupTimeInfo = string.Empty;

			var persistence = new Persistence();
			var jsonResult = persistence.ReadFormInformation();
			var periodicBackupModel = jsonResult != null
				? jsonResult.PeriodicBackupModelList != null
				? jsonResult.PeriodicBackupModelList[0] != null
				? jsonResult.PeriodicBackupModelList.Where(p => p.BackupName.Equals(_taskName)).FirstOrDefault()
			    : null : null : null;

			if (periodicBackupModel != null && radioBtn_TimeChange.Checked)
			{			
				backupTimeInfo = backupTimeInfo + "Backup interval: " + periodicBackupModel.BackupInterval + " " +
					periodicBackupModel.TimeType + ", " + "First backup on: " +
					periodicBackupModel.FirstBackup + ", " + "at " +
					periodicBackupModel.BackupAt + ", ";
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