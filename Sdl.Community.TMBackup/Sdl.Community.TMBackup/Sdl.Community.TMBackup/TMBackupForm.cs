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
	public partial class TMBackupForm : Form
	{
		private DateTimePicker fromTimePicker;
		private DateTimePicker toTimePicker;


		public TMBackupForm()
		{
			InitializeFromTimePicker();
			InitializeToTimePicker();

			InitializeComponent();
		}

		private void InitializeFromTimePicker()
		{
			fromTimePicker = new DateTimePicker();
			fromTimePicker.Format = DateTimePickerFormat.Time;
			fromTimePicker.ShowUpDown = true;
			fromTimePicker.Location = new Point(100, 200);
			fromTimePicker.Width = 100;
			Controls.Add(fromTimePicker);
		}

		private void InitializeToTimePicker()
		{
			toTimePicker = new DateTimePicker();
			toTimePicker.Format = DateTimePickerFormat.Time;
			toTimePicker.ShowUpDown = true;
			toTimePicker.Location = new Point(100, 230);
			toTimePicker.Width = 100;
			Controls.Add(toTimePicker);
		}
	}
}
