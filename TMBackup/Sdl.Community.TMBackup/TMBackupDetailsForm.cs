using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupDetailsForm : Form
	{
		#region Private fields
		private BindingSource _source = new BindingSource();
		private List<BackupDetailsModel> _backupDetailsModelList = new List<BackupDetailsModel>();
		private string _taskName;
		#endregion

		#region Public properties
		public static string BackupDetailsInfo { get; set; }

		#endregion

		#region Constructors
		public TMBackupDetailsForm(string taskName)
		{
			InitializeComponent();
			InitializeBackupDetails();
			_taskName = taskName;
		}
				
		#endregion

		#region Actions
		private void btn_Add_Click(object sender, EventArgs e)
		{
			var persistence = new Persistence();
			persistence.SaveDetailsFormInfo(_backupDetailsModelList, _taskName);

			_backupDetailsModelList.Clear();

			GetBackupDetailsInfo();
			dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
		}

		private void btn_Delete_Click(object sender, EventArgs e)
		{
			var removedActionsList = new List<BackupDetailsModel>();
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
				var persistence = new Persistence();
				persistence.DeleteDetailsFromInfo(removedActionsList, _taskName);

				GetBackupDetailsInfo();
			}
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			foreach (var backupDetailModel in _backupDetailsModelList)
			{
				BackupDetailsInfo = BackupDetailsInfo + backupDetailModel.BackupAction + ", " + backupDetailModel.BackupType + ", " + backupDetailModel.BackupPattern + "; ";
			}
			Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
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
			var isEmptyRow = false;

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
		#endregion

		#region Private methods
		private void GetBackupDetailsInfo()
		{
			var persistence = new Persistence();
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
				var backupDetailsModel = new BackupDetailsModel();

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

		// Add rows to a BindingList which can be used when moving up/down the selected row on the grid
		private BindingList<DataGridViewRow> AddRowsToList()
		{
			_backupDetailsModelList.Clear();
			var rowList = new BindingList<DataGridViewRow>();

			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				if (!string.IsNullOrEmpty(row.Cells[0].Value.ToString()) && !string.IsNullOrEmpty(row.Cells[1].Value.ToString()) && !string.IsNullOrEmpty(row.Cells[2].Value.ToString()))
				{
					rowList.Add(row);
				}
			}
			if (dataGridView1.RowCount <= 0 || dataGridView1.SelectedRows.Count <= 0)
			{
				return null;
			}
			return rowList;
		}

		// Add rows information from grid to a BindingList which will be used as data source when moving row up/down
		private void AddRowsInformation(BindingList<DataGridViewRow> rowList)
		{
			var bindingSource = new BindingSource();
			bindingSource.DataSource = rowList;

			foreach (DataGridViewRow item in bindingSource)
			{
				var bdm = new BackupDetailsModel();
				bdm.BackupAction = item.Cells[0].Value != null ? item.Cells[0].Value.ToString() : null;
				bdm.BackupType = item.Cells[1].Value != null ? item.Cells[1].Value.ToString() : null;
				bdm.BackupPattern = item.Cells[2].Value != null ? item.Cells[2].Value.ToString() : null;

				_backupDetailsModelList.Add(bdm);
			}
			dataGridView1.DataSource = _backupDetailsModelList;

			var persistence = new Persistence();
			persistence.UpdateBackupDetailsForm(_backupDetailsModelList, _taskName);

			GetBackupDetailsInfo();
			dataGridView1.ClearSelection();
		}

		// Initizialize the Backup Details grid when opening with exiting data from json
		private void InitializeBackupDetails()
		{
			var persistence = new Persistence();
			var request = persistence.ReadFormInformation();
			var backupDetails = request != null
				? request.BackupDetailsModelList != null
				? request.BackupDetailsModelList.Count > 0
				? request.BackupDetailsModelList[0] != null
				? request.BackupDetailsModelList.Where(b => b.BackupName.Equals(_taskName)).ToList()
				: null : null : null : null;

			if (backupDetails != null && backupDetails.Count > 0)
			{
				// create backupModel which is used as a new row where user can add another Action
				var emtpyModel = new BackupDetailsModel { BackupAction = string.Empty, BackupType = string.Empty, BackupPattern = string.Empty };
				request.BackupDetailsModelList.Insert(request.BackupDetailsModelList.Count, emtpyModel);

				dataGridView1.DataSource = request.BackupDetailsModelList;
			}
		}

		// Disable rows from the actions grid which already have values(user can only add/delete actions)
		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			dataGridView1.Rows[e.RowIndex].ReadOnly = true;

			if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
			{
				dataGridView1.Rows[e.RowIndex].ReadOnly = false;
			}
		}
		#endregion
	}
}