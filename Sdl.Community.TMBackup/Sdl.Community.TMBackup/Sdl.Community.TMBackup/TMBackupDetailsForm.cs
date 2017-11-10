using Sdl.Community.TMBackup.Helpers;
using Sdl.Community.TMBackup.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupDetailsForm : Form
	{
		private BindingSource _source = new BindingSource();

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

			_backupDetailsModelList.Clear();

			GetBackupDetailsInfo();
			dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
		}

		private void btn_Delete_Click(object sender, EventArgs e)
		{
			List<BackupDetailsModel> removedActionsList = new List<BackupDetailsModel>();
			if (dataGridView1.SelectedRows.Count > 0)
			{
				foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
				{
					removedActionsList.Add(new BackupDetailsModel
					{
						BackupAction = selectedRow.Cells[0].Value.ToString(),
						BackupType = selectedRow.Cells[1].Value.ToString(),
						BackupPattern = selectedRow.Cells[2].Value.ToString()
					});
				}
				Persistence persistence = new Persistence();
				persistence.DeleteDetailsFromInfo(removedActionsList);

				GetBackupDetailsInfo();
			}
		}

		private void btn_Reset_Click(object sender, EventArgs e)
		{

		}

		private void btn_DownArrow_Click(object sender, EventArgs e)
		{
			//MoveRow(false);

			_backupDetailsModelList.Clear();
			BindingList<DataGridViewRow> rowList = new BindingList<DataGridViewRow>();

			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				if (!string.IsNullOrEmpty(row.Cells[0].Value.ToString()) && !string.IsNullOrEmpty(row.Cells[1].Value.ToString()) && !string.IsNullOrEmpty(row.Cells[2].Value.ToString()))
				{
					rowList.Add(row);
				}
			}

			if (dataGridView1.RowCount <= 0 || dataGridView1.SelectedRows.Count <= 0)
			{
				return;
			}

			// check if selected row is the last row with values from the grid
			var index = dataGridView1.SelectedCells[0].OwningRow.Index;			
			if (index == dataGridView1.Rows.Count - 2)
			{
				return;
			}

			var nextRow = rowList[index + 1];
			rowList.Remove(nextRow);
			rowList.Insert(index, nextRow);

			var bindingSource = new BindingSource();
			bindingSource.DataSource = rowList;

			foreach (DataGridViewRow item in bindingSource)
			{
				BackupDetailsModel bdm = new BackupDetailsModel();
				bdm.BackupAction = item.Cells[0].Value != null ? item.Cells[0].Value.ToString() : null;
				bdm.BackupType = item.Cells[1].Value != null ? item.Cells[1].Value.ToString() : null;
				bdm.BackupPattern = item.Cells[2].Value != null ? item.Cells[2].Value.ToString() : null;

				_backupDetailsModelList.Add(bdm);
			}
			dataGridView1.DataSource = _backupDetailsModelList;

			Persistence persistence = new Persistence();
			persistence.UpdateBackupDetailsForm(_backupDetailsModelList);

			GetBackupDetailsInfo();

			dataGridView1.ClearSelection();
			dataGridView1.Rows[index + 1].Selected = true;
			dataGridView1.CurrentCell = dataGridView1.Rows[index + 1].Cells[0];
		}

		private void btn_UpArrow_Click(object sender, EventArgs e)
		{
			// To do: refactorize functionality into MoveRow method for both DownArrow and UpArrow
			//MoveRow(true);

			_backupDetailsModelList.Clear();
			BindingList<DataGridViewRow> rowList = new BindingList<DataGridViewRow>();

			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				if (!string.IsNullOrEmpty(row.Cells[0].Value.ToString()) && !string.IsNullOrEmpty(row.Cells[1].Value.ToString()) && !string.IsNullOrEmpty(row.Cells[2].Value.ToString()))
				{
					rowList.Add(row);
				}
			}

			if (dataGridView1.RowCount <= 0 || dataGridView1.SelectedRows.Count <= 0)
			{
				return;
			}

			var index = dataGridView1.SelectedCells[0].OwningRow.Index;
			if (index == 0)
			{
				return;
			}

			var prevRow = rowList[index - 1];
			rowList.Remove(prevRow);
			rowList.Insert(index, prevRow);

			var bindingSource = new BindingSource();
			bindingSource.DataSource = rowList;

			foreach (DataGridViewRow item in bindingSource)
			{
				BackupDetailsModel bdm = new BackupDetailsModel();
				bdm.BackupAction = item.Cells[0].Value != null ? item.Cells[0].Value.ToString() : null;
				bdm.BackupType = item.Cells[1].Value != null ? item.Cells[1].Value.ToString() : null;
				bdm.BackupPattern = item.Cells[2].Value != null ? item.Cells[2].Value.ToString() : null;

				_backupDetailsModelList.Add(bdm);
			}
			dataGridView1.DataSource = _backupDetailsModelList;

			Persistence persistence = new Persistence();
			persistence.UpdateBackupDetailsForm(_backupDetailsModelList);

			GetBackupDetailsInfo();

			dataGridView1.ClearSelection();
			dataGridView1.Rows[index - 1].Selected = true;
			dataGridView1.CurrentCell = dataGridView1.Rows[index - 1].Cells[0];
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
			if (dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryActionColumnIndex].FormattedValue.ToString() == string.Empty &&
				!string.IsNullOrEmpty(dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryTypeColumnIndex].FormattedValue.ToString()) &&
				!string.IsNullOrEmpty(dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryPatternColumnIndex].FormattedValue.ToString()))
			{
				e.Cancel = true;
				dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryTypeColumnIndex].ErrorText = Constants.MandatoryValue;
			}
			else if (dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryTypeColumnIndex].FormattedValue.ToString() == string.Empty)
			{
				e.Cancel = true;
				dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryActionColumnIndex].ErrorText = Constants.MandatoryValue;
			}
			else if (dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryPatternColumnIndex].FormattedValue.ToString() == string.Empty)
			{
				e.Cancel = true;
				dataGridView1.Rows[e.RowIndex].Cells[Constants.MandatoryPatternColumnIndex].ErrorText = Constants.MandatoryValue;
			}
		}

		private void dataGridView1_CellValidating_1(object sender, DataGridViewCellValidatingEventArgs e)
		{
			var dataGrid = (DataGridView)sender;
			bool isEmptyRow = false;
			foreach (DataGridViewCell cell in dataGrid.Rows[dataGrid.Rows.Count - 1].Cells)
			{
				if (cell.Value == null)
				{
					isEmptyRow = true;
				}
			}
			if (!isEmptyRow && e.ColumnIndex == Constants.MandatoryActionColumnIndex || e.ColumnIndex == Constants.MandatoryTypeColumnIndex || e.ColumnIndex == Constants.MandatoryPatternColumnIndex)
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

			if (request != null && request.BackupDetailsModelList != null)
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

		private void MoveRow(bool isPreviewsRow)
		{
			_backupDetailsModelList.Clear();
			BindingList<DataGridViewRow> rowList = new BindingList<DataGridViewRow>();

			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				rowList.Add(row);
			}

			if (dataGridView1.RowCount <= 0 || dataGridView1.SelectedRows.Count <= 0)
			{
				return;
			}

			var index = dataGridView1.SelectedCells[0].OwningRow.Index;
			if (index == 0)
			{
				return;
			}

			if (isPreviewsRow)
			{
				var prevRow = rowList[index - 1];
				rowList.Remove(prevRow);
				rowList.Insert(index, prevRow);
			}
			else
			{
				var nextRow = rowList[index + 1];
				rowList.Remove(nextRow);
				rowList.Insert(index, nextRow);
			}

			var bindingSource = new BindingSource();
			bindingSource.DataSource = rowList;

			foreach (DataGridViewRow item in bindingSource)
			{
				BackupDetailsModel bdm = new BackupDetailsModel();
				bdm.BackupAction = item.Cells[0].Value != null ? item.Cells[0].Value.ToString() : null;
				bdm.BackupType = item.Cells[1].Value != null ? item.Cells[1].Value.ToString() : null;
				bdm.BackupPattern = item.Cells[2].Value != null ? item.Cells[2].Value.ToString() : null;

				_backupDetailsModelList.Add(bdm);
			}
			dataGridView1.DataSource = _backupDetailsModelList;

			dataGridView1.ClearSelection();
			dataGridView1.Rows[index - 1].Selected = true;
		}
	}
}