using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class PeriodicBackupForm : Form
	{
		public PeriodicBackupForm()
		{
			InitializeComponent();

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

				Persistence persistence = new Persistence();
				persistence.SavePeriodicBackupInfo(periodicBackupModel);

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

			if(result != null)
			{
				cmbBox_Interval.SelectedItem = result.PeriodicBackupModel != null ? result.PeriodicBackupModel.TimeType : string.Empty;
				txtBox_TimeInterval.Text = result.PeriodicBackupModel != null ? result.PeriodicBackupModel.BackupInterval.ToString() : string.Empty;
				dateTimePicker_FirstBackup.Value = result.PeriodicBackupModel != null ? result.PeriodicBackupModel.FirstBackup : DateTime.Now;
				timePicker_At.Text = result.PeriodicBackupModel != null ? result.PeriodicBackupModel.BackupAt : string.Empty;
			}
		}

		private void SetDateTimeValue()
		{
			dateTimePicker_FirstBackup.Value = DateTime.Now;

			var currentDate = DateTime.Now;
			timePicker_At.Text = string.Concat(currentDate.Hour + ":" + currentDate.Minute + ":" + currentDate.Second + " " + CultureInfo.InvariantCulture);
		}
	}
}