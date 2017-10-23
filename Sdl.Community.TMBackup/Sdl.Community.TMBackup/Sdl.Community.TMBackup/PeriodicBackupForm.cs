using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class PeriodicBackupForm : Form
	{
		public PeriodicBackupForm()
		{
			InitializeComponent();

			SetDateTimeFormat();
		}	
		
		private void SetDateTimeFormat()
		{
			timePicker_At.Format = DateTimePickerFormat.Custom;
			timePicker_At.CustomFormat = "HH:mm:ss tt";
			timePicker_At.ShowUpDown = true;
		}
	}
}
