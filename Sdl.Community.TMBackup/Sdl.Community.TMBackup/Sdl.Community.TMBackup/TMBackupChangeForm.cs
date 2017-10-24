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
	public partial class TMBackupChangeForm : Form
	{
		public TMBackupChangeForm()
		{
			InitializeComponent();
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
	}
}
