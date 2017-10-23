using System;
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
			cmbBox_Interval.DataSource = EnumHelper.GetTimeTypeDescription();
		}

		private void btn_Close_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}