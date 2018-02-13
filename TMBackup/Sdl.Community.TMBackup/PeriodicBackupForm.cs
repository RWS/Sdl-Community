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
					PeriodicBackupModel periodicBackupModel = new PeriodicBackupModel();
					periodicBackupModel.BackupInterval = int.Parse(txtBox_TimeInterval.Text);
					periodicBackupModel.TimeType = cmbBox_Interval.SelectedItem.ToString();
					periodicBackupModel.FirstBackup = dateTimePicker_FirstBackup.Value;
					periodicBackupModel.BackupAt = timePicker_At.Text;
					periodicBackupModel.BackupName = _taskName;
					periodicBackupModel.TrimmedBackupName = string.Concat(_taskName.Where(c => !char.IsWhiteSpace(c)));
					_periodicBackupModelList.Add(periodicBackupModel);

					Persistence persistence = new Persistence();
					persistence.SavePeriodicBackupInfo(_periodicBackupModelList, _taskName);

					Close();
				}
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
			timePicker_At.Text = string.Concat(currentDate.Hour + ":" + currentDate.Minute + ":" + currentDate.Second + " " + CultureInfo.InvariantCulture);
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