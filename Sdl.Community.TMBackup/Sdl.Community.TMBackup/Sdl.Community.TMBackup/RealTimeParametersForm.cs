using Sdl.Community.TMBackup.Helpers;
using Sdl.Community.TMBackup.Models;
using System;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class RealTimeParametersForm : Form
	{
		public RealTimeParametersForm()
		{
			InitializeComponent();

			InitializeFormData();
		}

		private void InitializeFormData()
		{
			cmbBox_Interval.DataSource = EnumHelper.GetTimeTypeDescription();

			Persistence persistence = new Persistence();
			var result = persistence.ReadFormInformation();
			if(result != null)
			{
				cmbBox_Interval.SelectedItem = result.RealTimeBackupModel != null ? result.RealTimeBackupModel.TimeType : string.Empty;
				txtBox_Interval.Text = result.RealTimeBackupModel != null ? result.RealTimeBackupModel.BackupInterval.ToString() : string.Empty;
			}		
		}

		private void btn_Close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btn_Set_Click(object sender, EventArgs e)
		{
			RealTimeBackupModel realTimeModel = new RealTimeBackupModel();
			realTimeModel.BackupInterval = int.Parse(txtBox_Interval.Text);
			realTimeModel.TimeType = cmbBox_Interval.SelectedItem.ToString();

			Persistence persistence = new Persistence();
			persistence.SaveRealTimeInfo(realTimeModel);

			this.Close();
		}
	}
}