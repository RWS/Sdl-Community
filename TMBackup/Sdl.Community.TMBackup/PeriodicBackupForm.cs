using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;

namespace Sdl.Community.TMBackup
{
	public partial class PeriodicBackupForm : Form
	{
		private string _taskName;
		private List<PeriodicBackupModel> _periodicBackupModelList = new List<PeriodicBackupModel>();
		private bool _isNowPressed = false;

		public PeriodicBackupForm(string taskName)
		{
			InitializeComponent();
			SetDateTimeFormat();
			InitializeFormData();
			SetDateTimeValue();
			_taskName = taskName;
		}

		private void SetDateTimeFormat()
		{
			timePicker_At.Format = DateTimePickerFormat.Custom;
			timePicker_At.CustomFormat = Constants.TimeFormat;
			timePicker_At.ShowUpDown = true;
		}

		private void btn_Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_Set_Click(object sender, EventArgs e)
		{
			var isNotNumeric = CheckIfNotNumeric();
			if (isNotNumeric)
			{
				MessageBox.Show(Constants.BackupIntervalErrorMessage, Constants.InformativeMessage, MessageBoxButtons.OK);
			}
			else
			{
				if (string.IsNullOrEmpty(txtBox_TimeInterval.Text))
				{
					MessageBox.Show(Constants.IntervalErrorMessage, Constants.InformativeMessage);
				}
				else
				{
					var atScheduleTime = DateTime.Parse(timePicker_At.Text, CultureInfo.InvariantCulture);
					//set isNowPressed to false in case user pressed Now button and after that he has changed the 'At' time manually
					if (dateTimePicker_FirstBackup.Value.Day.Equals(DateTime.Now.Day) && dateTimePicker_FirstBackup.Value.Month.Equals(DateTime.Now.Month) && dateTimePicker_FirstBackup.Value.Year.Equals(DateTime.Now.Year)
						&& (atScheduleTime.Hour != DateTime.Now.Hour || atScheduleTime.Minute != DateTime.Now.Minute && _isNowPressed))
					{
						_isNowPressed = false;
					}

					if (!dateTimePicker_FirstBackup.Value.Day.Equals(DateTime.Now.Day) && _isNowPressed
						|| !dateTimePicker_FirstBackup.Value.Month.Equals(DateTime.Now.Month) && _isNowPressed
						|| !dateTimePicker_FirstBackup.Value.Year.Equals(DateTime.Now.Year) && _isNowPressed)
					{
						_isNowPressed = false;
					}

					var periodicBackupModel = new PeriodicBackupModel();
					periodicBackupModel.BackupInterval = int.Parse(txtBox_TimeInterval.Text);
					periodicBackupModel.TimeType = cmbBox_Interval.SelectedItem.ToString();
					periodicBackupModel.FirstBackup = new DateTime(dateTimePicker_FirstBackup.Value.Year,
														  dateTimePicker_FirstBackup.Value.Month,
														  dateTimePicker_FirstBackup.Value.Day) + new TimeSpan(atScheduleTime.Hour, atScheduleTime.Minute, atScheduleTime.Second);
					periodicBackupModel.BackupAt = timePicker_At.Text;
					periodicBackupModel.BackupName = _taskName;
					periodicBackupModel.TrimmedBackupName = string.Concat(_taskName.Where(c => !char.IsWhiteSpace(c)));
					periodicBackupModel.IsNowPressed = _isNowPressed;
					_periodicBackupModelList.Add(periodicBackupModel);

					var persistence = new Persistence();
					persistence.SavePeriodicBackupInfo(_periodicBackupModelList, _taskName);

					Close();
				}
			}
		}

		private void btn_Now_Click(object sender, EventArgs e)
		{
			_isNowPressed = true;
			SetDateTimeValue();
		}

		private void InitializeFormData()
		{
			cmbBox_Interval.DataSource = EnumHelper.GetTimeTypeDescription();

			var persistence = new Persistence();
			var result = persistence.ReadFormInformation();

			if (result.PeriodicBackupModelList != null)
			{
				var periodicBackupModelItem = result.PeriodicBackupModelList?.Count > 0
					? result.PeriodicBackupModelList[0] != null
						? result.PeriodicBackupModelList.FirstOrDefault(p => p.BackupName.Equals(_taskName))
						: null : null;

				cmbBox_Interval.SelectedItem = periodicBackupModelItem != null ? periodicBackupModelItem.TimeType : string.Empty;
				txtBox_TimeInterval.Text = periodicBackupModelItem?.BackupInterval.ToString() ?? string.Empty;
				dateTimePicker_FirstBackup.Value = periodicBackupModelItem?.FirstBackup ?? DateTime.Now;
				timePicker_At.Text = periodicBackupModelItem != null ? periodicBackupModelItem.BackupAt : string.Empty;
			}
		}

		private void SetDateTimeValue()
		{
			dateTimePicker_FirstBackup.Value = DateTime.Now;

			var currentDate = DateTime.Now;
			timePicker_At.Text = string.Concat(currentDate.Hour + ":" + currentDate.Minute + ":" + currentDate.Second);
		}

		private void txtBox_TimeInterval_KeyPress(object sender, KeyPressEventArgs e)
		{
			for (int h = 58; h <= 127; h++)
			{
				if (e.KeyChar == h)             //58 to 127 are alphabets which will be blocked
				{
					e.Handled = true;
				}
			}
			for (int k = 32; k <= 47; k++)
			{
				if (e.KeyChar == k)              //32 to 47 are special characters that will be blocked
				{


					e.Handled = true;
				}
			}
		}

		private bool CheckIfNotNumeric()
		{
			int i;
			if (!int.TryParse(txtBox_TimeInterval.Text, out i))
			{
				return true;
			}
			return false;
		}
	}
}