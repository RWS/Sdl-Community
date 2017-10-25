using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.TMBackup.Helpers;
using Sdl.Community.TMBackup.Models;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupDetailsForm : Form
	{
	  public TMBackupDetailsForm()
		{
			InitializeComponent();

			dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating_1);
			dataGridView1.RowValidating += new DataGridViewCellCancelEventHandler(dataGridView1_RowValidating);
		}


		private void btn_Add_Click(object sender, EventArgs e)
		{
			List<BackupDetailsModel> backupDetailsModelList = new List<BackupDetailsModel>();

			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				if (!string.IsNullOrEmpty(row.Cells[0].Value.ToString()))
				{
					BackupDetailsModel backupDetailsModel = new BackupDetailsModel();
					backupDetailsModel.BackupAction = row.Cells[0].Value.ToString();
					backupDetailsModel.BackupType = row.Cells[1].Value.ToString();
					backupDetailsModel.BackupType = row.Cells[2].Value.ToString();

					backupDetailsModelList.Add(backupDetailsModel);

					//Persistence persistence = new Persistence();
					//persistence.SaveDetailsFormInfo(backupDetailsModelList);

					
				}
			}
			dataGridView1.Refresh();
			dataGridView1.DataSource = backupDetailsModelList;
			dataGridView1.Rows.Add();
			dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
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
		

		private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
		{
			if (dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryActionColumnIndex].FormattedValue.ToString() == string.Empty)
			{
				e.Cancel = true;
				dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryTypeColumnIndex].ErrorText = Constants.MandatoryValue;
			}
			else if(dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryTypeColumnIndex].FormattedValue.ToString() == string.Empty)
			{
				e.Cancel = true;
				dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryActionColumnIndex].ErrorText = Constants.MandatoryValue;
			}
			else if(dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryPatternColumnIndex].FormattedValue.ToString() == string.Empty)
			{
				e.Cancel = true;
				dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryPatternColumnIndex].ErrorText = Constants.MandatoryValue;
			}
		}

		private void dataGridView1_CellValidating_1(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (e.ColumnIndex == Constants.MandatoryActionColumnIndex || e.ColumnIndex == Constants.MandatoryTypeColumnIndex || e.ColumnIndex == Constants.MandatoryPatternColumnIndex)
			{
				if (e.FormattedValue.ToString() == string.Empty)
				{
					dataGridView1[e.ColumnIndex, e.RowIndex].ErrorText = Constants.MandatoryValue;
					e.Cancel = true;
				}
				else
				{
					dataGridView1[e.ColumnIndex, e.RowIndex].ErrorText = string.Empty;
				}
			}
		}
	}
}
