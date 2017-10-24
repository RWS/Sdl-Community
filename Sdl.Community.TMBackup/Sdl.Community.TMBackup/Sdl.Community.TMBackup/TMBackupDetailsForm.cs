using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.TMBackup.Models;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupDetailsForm : Form
	{
		public TMBackupDetailsForm()
		{
			InitializeComponent();
		}

		private void btn_Add_Click(object sender, EventArgs e)
		{
			BackupDetailsModel backupDetailsModel = new BackupDetailsModel();
			backupDetailsModel.BackupAction = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0].Value.ToString();
			backupDetailsModel.BackupType = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[1].Value.ToString();
			backupDetailsModel.BackupPattern = dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[2].Value.ToString();
			
			Persistence persistence = new Persistence();
			persistence.SaveDetailsFormInfo(backupDetailsModel);
		}

		private void btn_Delete_Click(object sender, EventArgs e)
		{

		}

		private void btn_Reset_Click(object sender, EventArgs e)
		{

		}

		private void btn_DownArrow_Click(object sender, EventArgs e)
		{

		}

		private void btn_UpArrow_Click(object sender, EventArgs e)
		{

		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{

		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
