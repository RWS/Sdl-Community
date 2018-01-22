using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.TMBackup
{
	public partial class PeriodicBackupForm : Form
	{
		private string _taskName { get; set; }
		private List<PeriodicBackupModel> _periodicBackupModelList = new List<PeriodicBackupModel>();

		public PeriodicBackupForm(string taskName)
		{
			InitializeComponent();

			_taskName = taskName;

			SetDateTimeFormat();

			InitializeFormData();

			SetDateTimeValue();
		}

		private void SetDateTimeFormat()
		{
			timePicker_At.Format = DateTimePickerFormat.Custom;
			timePicker_At.CustomFormat = "HH:mm:ss tt";
			timePicker_At.ShowUpDown = true;
		}

		private void btn_Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_Set_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtBox_TimeInterval.Text))
			{
				MessageBox.Show(Constants.IntervalErrorMessage, Constants.InformativeMessage);
			}
			else
			{
				PeriodicBackupModel periodicBackupModel = new PeriodicBackupModel();
				periodicBackupModel.BackupInterval = int.Parse(txtBox_TimeInterval.Text);
				periodicBackupModel.TimeType = cmbBox_Interval.SelectedItem.ToString();
				periodicBackupModel.FirstBackup = dateTimePicker_FirstBackup.Value;
				periodicBackupModel.BackupAt = timePicker_At.Text;
				_periodicBackupModelList.Add(periodicBackupModel);

				Persistence persistence = new Persistence();				
				persistence.SavePeriodicBackupInfo(_periodicBackupModelList, _taskName);

				Close();
			}
		}

		private void btn_Now_Click(object sender, EventArgs e)
		{
			SetDateTimeValue();
		}

		private void InitializeFormData()
		{
			cmbBox_Interval.DataSource = EnumHelper.GetTimeTypeDescription();

			var persistence = new Persistence();
			var result = persistence.ReadFormInformation();
			var periodicBackupModelItem = result != null ? result.PeriodicBackupModelList != null ? result.PeriodicBackupModelList.Where(p => p.BackupName.Equals(_taskName)).FirstOrDefault() 
														 : null : null;

			cmbBox_Interval.SelectedItem = periodicBackupModelItem != null ? periodicBackupModelItem.TimeType : string.Empty;
			txtBox_TimeInterval.Text = periodicBackupModelItem != null ? periodicBackupModelItem.BackupInterval.ToString() : string.Empty;
			dateTimePicker_FirstBackup.Value = periodicBackupModelItem != null ? periodicBackupModelItem.FirstBackup : DateTime.Now;
			timePicker_At.Text = periodicBackupModelItem != null ? periodicBackupModelItem.BackupAt : string.Empty;
		}

		private void SetDateTimeValue()
		{
			dateTimePicker_FirstBackup.Value = DateTime.Now;

			var currentDate = DateTime.Now;
			timePicker_At.Text = string.Concat(currentDate.Hour + ":" + currentDate.Minute + ":" + currentDate.Second + " " + CultureInfo.InvariantCulture);
		}
	}
}