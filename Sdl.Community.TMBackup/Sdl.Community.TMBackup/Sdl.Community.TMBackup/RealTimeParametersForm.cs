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
	public partial class RealTimeParametersForm : Form
	{
		public RealTimeParametersForm()
		{
			InitializeComponent();

			InitializeTimeTypeDropDown();
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
