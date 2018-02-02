using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
			_taskName = taskName;
			InitializeBackupDetails();
		}

		#endregion

		#region Actions
		private void btn_Add_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(_taskName))
			{
				MessageBox.Show(Constants.TaskNameErrorMessage, Constants.InformativeMessage);
			}
			else
			{
				AddValuesFromRows();
				if (_backupDetailsModelList.Any() && _backupDetailsModelList != null)
				{
					var persistence = new Persistence();
					persistence.SaveDetailsFormInfo(_backupDetailsModelList, _taskName);

					_backupDetailsModelList.Clear();

					GetBackupDetailsInfo();
					dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
				}
			}
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
			var persistence = new Persistence();
			var jsonModel = persistence.ReadFormInformation();
		
			if (jsonModel!= null && jsonModel.BackupDetailsModelList!= null && jsonModel.BackupDetailsModelList.Count > 0)
			{
				foreach (var backupDetailModel in jsonModel.BackupDetailsModelList)
				{
					BackupDetailsInfo = BackupDetailsInfo + backupDetailModel.BackupAction + ", " + backupDetailModel.BackupType + ", " + backupDetailModel.BackupPattern + "; ";
				}
				Close();
			}		
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}
				
		// Disable rows from the actions grid which already have values(user can only add/delete actions)
		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (dataGridView1.Rows.Count > 0)
			{
				dataGridView1.Rows[e.RowIndex].ReadOnly = true;

				if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
				{
					dataGridView1.Rows[e.RowIndex].ReadOnly = false;
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

		/// <summary>
		/// Add values from rows
		/// </summary>
		private void AddValuesFromRows()
		{
			if (dataGridView1.Rows.Count > 0)
			{
				var persistence = new Persistence();
				var jsonRequestModel = persistence.ReadFormInformation();

				if (jsonRequestModel != null && jsonRequestModel.BackupDetailsModelList != null && jsonRequestModel.BackupDetailsModelList.Count > 0)
				{
					var backupDetailsList = jsonRequestModel.BackupDetailsModelList.Where(b => b.BackupName.Equals(_taskName)).ToList();
					if (backupDetailsList != null)
					{
						foreach (DataGridViewRow row in dataGridView1.Rows)
						{
							var backupDetail = backupDetailsList.Where(b => b.BackupAction.Equals(row.Cells[0].Value.ToString())
													&& b.BackupType.Equals(row.Cells[1].Value.ToString())
													&& b.BackupPattern.Equals(row.Cells[2].Value.ToString())).FirstOrDefault();

							if (backupDetail == null)
							{
								backupDetail = SetBackupDetailsFromGrid(row);

								// add all values to the model which will be used to persist data into Json file
								if (!string.IsNullOrEmpty(backupDetail.BackupAction)
									&& !string.IsNullOrEmpty(backupDetail.BackupType)
									&& !string.IsNullOrEmpty(backupDetail.BackupPattern))
								{
									_backupDetailsModelList.Add(backupDetail);
								}
							}
							else if(backupDetail != null && row.Cells[2].Selected)
							{
								MessageBox.Show(Constants.ActionAlreadyExist, Constants.InformativeMessage);
							}
						}
					}
				}
				else
				{
					foreach (DataGridViewRow row in dataGridView1.Rows)
					{
						var backupDetailsModel = SetBackupDetailsFromGrid(row);

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

		private BackupDetailsModel SetBackupDetailsFromGrid(DataGridViewRow row)
		{
			var backupDetailsModel = new BackupDetailsModel();
			backupDetailsModel.BackupName = _taskName;
			backupDetailsModel.TrimmedBackupName = string.Concat(_taskName.Where(c => !char.IsWhiteSpace(c)));

			if ((row.Cells[0].Value == null && row.Cells[1].Value == null && row.Cells[2].Value == null)
				|| (string.IsNullOrEmpty(row.Cells[0].Value.ToString()) && string.IsNullOrEmpty(row.Cells[1].Value.ToString()) && string.IsNullOrEmpty(row.Cells[2].Value.ToString())))
			{
				//do nothing in this case because it is about the last empty row added by datagridview
			}
			else
			{
				if (row.Cells[0].Value != null)
				{
					backupDetailsModel.BackupAction = row.Cells[0].Value.ToString();
				}
				else
				{
					MessageBox.Show(Constants.ActionNameErrorMessage, Constants.InformativeMessage);
				}
				if (row.Cells[1].Value != null)
				{
					backupDetailsModel.BackupType = row.Cells[1].Value.ToString();
				}
				else
				{
					MessageBox.Show(Constants.FileTypeErrorMessage, Constants.InformativeMessage);
				}
				if (row.Cells[2].Value != null)
				{
					backupDetailsModel.BackupPattern = row.Cells[2].Value.ToString();
				}
				else
				{
					MessageBox.Show(Constants.PatternErrorMessage, Constants.InformativeMessage);
				}
			}
			return backupDetailsModel;
		}
		#endregion
	}
}