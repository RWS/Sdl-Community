using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.TMBackup.Helpers;

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
			cmbBox_Interval.DataSource = Enum.GetValues(typeof(Enums.TimeTypes));
		}

		private void btn_Close_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
