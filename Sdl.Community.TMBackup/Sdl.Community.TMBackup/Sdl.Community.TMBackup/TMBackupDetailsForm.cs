using Sdl.Community.TMBackup.Helpers;
using Sdl.Community.TMBackup.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupDetailsForm : Form
	{
		private List<BackupDetailsModel> _backupDetailsModelList = new List<BackupDetailsModel>();

		public static string BackupDetailsInfo { get; set; }

		public TMBackupDetailsForm()
		{
			InitializeComponent();

			GetBackupDetailsInfo();
		}

		private void btn_Add_Click(object sender, EventArgs e)
		{
			Persistence persistence = new Persistence();
			persistence.SaveDetailsFormInfo(_backupDetailsModelList);

			GetBackupDetailsInfo();
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

			foreach (var backupDetailModel in _backupDetailsModelList)
			{
				BackupDetailsInfo = BackupDetailsInfo + backupDetailModel.BackupAction + ", " + backupDetailModel.BackupType + ", " + backupDetailModel.BackupPattern + "; ";
			}

			this.Close();						
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

		private void GetBackupDetailsInfo()
		{
			Persistence persistence = new Persistence();
			var request = persistence.ReadFormInformation();

			if(request != null && request.BackupDetailsModelList != null)
			{
				// create backupModel which is used as a new row where user can add another Action
				BackupDetailsModel emtpyModel = new BackupDetailsModel { BackupAction = string.Empty, BackupType = string.Empty, BackupPattern = string.Empty };
				request.BackupDetailsModelList.Insert(request.BackupDetailsModelList.Count, emtpyModel);

				dataGridView1.DataSource = request.BackupDetailsModelList;		
			}
		}
		
		private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex > -1)
			{
				var dataGrid = (DataGridView)sender;
				var row = dataGrid.Rows[e.RowIndex];
				BackupDetailsModel backupDetailsModel = new BackupDetailsModel();

				if (row != null)
				{
					if (row.Cells[0].Value != null)
					{
						backupDetailsModel.BackupAction = row.Cells[0].Value.ToString();
					}
					if (row.Cells[1].Value != null)
					{
						backupDetailsModel.BackupType = row.Cells[1].Value.ToString();
					}
					if (row.Cells[2].Value != null)
					{
						backupDetailsModel.BackupPattern = row.Cells[2].Value.ToString();
					}

					// add all values to the model which will be used to persist data into Json file
					if (!string.IsNullOrEmpty(backupDetailsModel.BackupAction)
						&& !string.IsNullOrEmpty(backupDetailsModel.BackupType)
						&& !string.IsNullOrEmpty(backupDetailsModel.BackupPattern))
					{
						_backupDetailsModelList.Add(backupDetailsModel);
					}
				}
			}
		}
	}
}