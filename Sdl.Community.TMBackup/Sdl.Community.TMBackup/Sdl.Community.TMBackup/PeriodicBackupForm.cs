using Sdl.Community.TMBackup.Helpers;
using System;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class PeriodicBackupForm : Form
	{
		public PeriodicBackupForm()
		{
			InitializeComponent();

			InitializeTimeTypeDropDown();
			SetDateTimeFormat();
		}	
		
		private void SetDateTimeFormat()
		{
			timePicker_At.Format = DateTimePickerFormat.Custom;
			timePicker_At.CustomFormat = "HH:mm:ss tt";
			timePicker_At.ShowUpDown = true;
		}

		private void InitializeTimeTypeDropDown()
		{			
			cmbBox_Interval.DataSource = EnumHelper.GetTimeTypeDescription();
		}

		private void btn_Close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btn_Set_Click(object sender, EventArgs e)
		{

		}
	}
}