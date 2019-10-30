using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupTasksForm : Form
	{
		public static readonly Log Log = Log.Instance;

		#region Private fields
		private JsonRequestModel _jsonRquestModel = new JsonRequestModel();
		private string _taskRunType = string.Empty;
		#endregion

		#region Constructors
		public TMBackupTasksForm()
		{
			InitializeComponent();
			GetBackupTasks();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Get all backup tasks from Windows Task Scheduler and display in the TMBackup interface
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Task> GetBackupTasks()
		{
			var backupTasks = new List<Task>();
			try
			{
				var persistence = new Persistence();
				var jsonRequestModel = persistence.ReadFormInformation();

				using (var ts = new TaskService())
				{
					var tasks = new List<TaskDefinitionModel>();
					if (ts.AllTasks != null)
					{
						if (jsonRequestModel != null && jsonRequestModel.ChangeSettingsModelList != null)
						{
							foreach (var task in ts.AllTasks)
							{
								var index = task.Name.IndexOf(" ") + 1;
								var taskName = task.Name.Substring(index);

								if (task.Name.Contains(Constants.TaskDetailValue))
								{
									var changeSettingModel = jsonRequestModel.ChangeSettingsModelList.FirstOrDefault(c => c.BackupName.Equals(taskName));
									if (changeSettingModel != null)
									{
										_taskRunType = changeSettingModel.IsManuallyOptionChecked ? "Manually" : "Automatically";
									}

									var triggerInfo = string.Empty;
									foreach (var trigger in task.Definition.Triggers)
									{
										if (trigger.Repetition.Interval.Hours == 0
											&& trigger.Repetition.Interval.Minutes == 0
											&& trigger.Repetition.Interval.Seconds == 0
											&& trigger.Repetition.Interval.Days == 0)
										{
											// Display interval for manually task
											triggerInfo = $"At: '{trigger.StartBoundary}'";
										}
										else
										{
											// Display interval for automatically task
											triggerInfo = $"At: '{trigger.StartBoundary}'. After triggered, repeat every '{trigger.Repetition.Interval}'";
										}
									}

									tasks.Add(new TaskDefinitionModel
									{
										TaskName = taskName,
										TaskRunType = _taskRunType,
										Status = task.State.ToString(),
										LastRun = task.LastRunTime.Year.Equals(1999) ? DateTime.MinValue : task.LastRunTime,
										NextRun = task.NextRunTime,
										Interval = _taskRunType.Equals("Manually") ? "Never" : triggerInfo
									});
									backupTasks.Add(task);
								}
							}
						}
					}
					// if any tasks does not exist and json is still populated it means that user has removed the tasks from the Windows Task Scheduler interface
					// in this way, json needs to be cleaned up 
					if (tasks == null || tasks.Count == 0)
					{
						if (jsonRequestModel?.BackupModelList != null) { jsonRequestModel.BackupModelList.Clear(); }
						if (jsonRequestModel?.BackupDetailsModelList != null) { jsonRequestModel.BackupDetailsModelList.Clear(); }
						if (jsonRequestModel?.PeriodicBackupModelList != null) { jsonRequestModel.PeriodicBackupModelList.Clear(); }
						if (jsonRequestModel?.ChangeSettingsModelList != null) { jsonRequestModel.ChangeSettingsModelList.Clear(); }

						persistence.WriteJsonRequestModel(jsonRequestModel);
					}
					dataGridView1.DataSource = tasks;
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.GetBackupTasks} {ex.Message} \n {ex.StackTrace}");
			}
			return backupTasks;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Set info for the manually tasks when trying to start running the backup of one or more tasks 
		/// </summary>
		/// <param name="persistence"></param>
		/// <param name="service"></param>
		/// <param name="row"></param>
		private Dictionary<string, bool> SetManuallyTasksInfo(Persistence persistence, Service service, DataGridViewRow row)
		{
			var backupInfo = new Dictionary<string, bool>();
			_taskRunType = row.Cells[1].Value.ToString();
			if (_taskRunType.Equals("Manually"))
			{
				var backupName = row.Cells[0].Value.ToString();

				var backupModel = _jsonRquestModel.BackupModelList.FirstOrDefault(b => b.BackupName.Equals(backupName));
				if (backupModel != null)
				{
					backupInfo.Add(backupModel.BackupName, true);
					persistence.RemoveDataFromJson(backupModel.BackupName);
				}
			}
			return backupInfo;
		}

		/// <summary>
		/// Add new information to the Json when running all/specific tasks
		/// </summary>
		/// <param name="persistence"></param>
		/// <param name="service"></param>
		private void AddInfoIntoJson(Persistence persistence, Service service, Dictionary<string, bool> backupInfo)
		{
			try
			{
				foreach (var entry in backupInfo)
				{
					if (_jsonRquestModel.BackupModelList != null)
					{
						persistence.SaveBackupModel(_jsonRquestModel.BackupModelList.FirstOrDefault(b => b.BackupName.Equals(entry.Key)));
					}
					if (_jsonRquestModel.ChangeSettingsModelList != null)
					{
						persistence.SaveChangeModel(_jsonRquestModel.ChangeSettingsModelList.FirstOrDefault(b => b.BackupName.Equals(entry.Key)));
					}
					if (_jsonRquestModel.BackupDetailsModelList != null)
					{
						persistence.SaveDetailModel(_jsonRquestModel.BackupDetailsModelList.FirstOrDefault(b => b.BackupName.Equals(entry.Key)));
					}
					if (_jsonRquestModel.PeriodicBackupModelList != null)
					{
						persistence.SavePeriodicModel(_jsonRquestModel.PeriodicBackupModelList.FirstOrDefault(b => b.BackupName.Equals(entry.Key)));
					}
					service.CreateTaskScheduler(entry.Key, entry.Value);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.AddInfoIntoJson} {ex.Message} \n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Run manually task/tasks
		/// </summary>
		private void RunManuallyTasks()
		{
			try
			{
				var persistence = new Persistence();
				var service = new Service();
				var backupInfo = new Dictionary<string, bool>();
				var dictionaryBackup = new Dictionary<string, bool>();

				_jsonRquestModel = persistence.ReadFormInformation();

				if (dataGridView1.SelectedRows.Count > 0)
				{
					// Run selected backup tasks
					foreach (DataGridViewRow row in dataGridView1.SelectedRows)
					{
						dictionaryBackup = SetManuallyTasksInfo(persistence, service, row);
						foreach (var entry in dictionaryBackup)
						{
							backupInfo.Add(entry.Key, entry.Value);
						}
					}
					AddInfoIntoJson(persistence, service, backupInfo);
				}
				else
				{
					// Run all manually backup tasks
					foreach (DataGridViewRow row in dataGridView1.Rows)
					{
						dictionaryBackup = SetManuallyTasksInfo(persistence, service, row);
						foreach (var entry in dictionaryBackup)
						{
							backupInfo.Add(entry.Key, entry.Value);
						}
					}
					AddInfoIntoJson(persistence, service, backupInfo);
				}
				GetBackupTasks();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.RunManuallyTasks} {ex.Message} \n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Run disabled task/tasks
		/// </summary>
		private void RunDisabledTasks()
		{
			try
			{
				var backupInfo = new Dictionary<string, bool>();
				var persistence = new Persistence();
				var service = new Service();
				var taskNames = new List<string>();
				_jsonRquestModel = persistence.ReadFormInformation();

				using (var ts = new TaskService())
				{
					var tasks = ts.AllTasks.Where(t => t.State.Equals(TaskState.Disabled)).ToList();

					if (dataGridView1.SelectedRows.Count > 0)
					{
						// Run selected disabled tasks
						foreach (var task in tasks)
						{
							if (task.Name.Contains(Constants.TaskDetailValue))
							{
								var index = task.Name.IndexOf(" ") + 1;
								var taskName = task.Name.Substring(index);

								taskNames.Add(taskName);
							}
						}

						foreach (DataGridViewRow row in dataGridView1.SelectedRows)
						{
							var selectedTaskName = taskNames.FirstOrDefault(t => t.Equals(row.Cells[0].Value.ToString()));
							var backupModel = _jsonRquestModel.BackupModelList.FirstOrDefault(b => b.BackupName.Equals(selectedTaskName));
							if (backupModel != null)
							{
								backupInfo.Add(backupModel.BackupName, false);
								persistence.RemoveDataFromJson(backupModel.BackupName);
							}
						}
						AddInfoIntoJson(persistence, service, backupInfo);
					}
					else
					{
						// Run all disabled tasks
						foreach (var task in tasks)
						{
							if (task.Name.Contains(Constants.TaskDetailValue))
							{
								var index = task.Name.IndexOf(" ") + 1;
								var taskName = task.Name.Substring(index);

								var backupModel = _jsonRquestModel.BackupModelList.FirstOrDefault(b => b.BackupName.Equals(taskName));
								if (backupModel != null)
								{
									backupInfo.Add(backupModel.BackupName, false);

									persistence.RemoveDataFromJson(backupModel.BackupName);
								}
							}
						}
						AddInfoIntoJson(persistence, service, backupInfo);
					}
					GetBackupTasks();
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.RunDisabledTasks} {ex.Message} \n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Open Task Scheduler application from Windows
		/// </summary>
		private void OpenWindowsTaskScheduler()
		{
			Process.Start(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Administrative Tools\Task Scheduler");
		}

		/// <summary>
		/// Open Help file from SDL Community site
		/// </summary>
		private void OpenReadMe()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3134.sdl-tmbackup");
		}

		/// <summary>
		/// Create new task
		/// </summary>
		private void CreateNewTask()
		{
			Hide();

			var tmBackupForm = new TMBackupForm(true, string.Empty);
			tmBackupForm.ShowDialog();
		}
		#endregion

		#region Events	
		/// <summary>
		/// Open TMBackup form for the selected task on cell double click
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e">e</param>
		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (dataGridView1.Rows.Count > 0)
			{
				var cellValue = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

				Hide();

				var tmBackupForm = new TMBackupForm(false, cellValue);
				tmBackupForm.ShowDialog();
			}
		}

		/// <summary>
		/// Delete selected task/tasks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (dataGridView1.DataSource != null)
			{
				using (var ts = new TaskService())
				{
					foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
					{
						var task = ts.AllTasks.FirstOrDefault(t => t.Name.Contains(selectedRow.Cells[0].Value.ToString()));
						if (task != null)
						{
							var dialogResult = MessageBox.Show(Constants.DeleteInformativeMessage, string.Empty, MessageBoxButtons.YesNo);
							if (dialogResult == DialogResult.Yes)
							{
								ts.RootFolder.DeleteTask(task.Name);

								var index = task.Name.IndexOf(" ") + 1;
								var jsonTaskName = task.Name.Substring(index);

								var persistence = new Persistence();
								persistence.RemoveDataFromJson(jsonTaskName);
							}
						}
					}
					GetBackupTasks();
				}
			}
		}

		// Display the context menu with the 'Delete' option for the tasks
		private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				foreach (DataGridViewRow row in dataGridView1.SelectedRows)
				{
					row.Selected = true;
					dataGridView1.CurrentCell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
					contextMenuStrip1.Show(dataGridView1, e.Location);
					contextMenuStrip1.Show(Cursor.Position);
				}
			}
		}

		/// <summary>
		/// Run disabled task/tasks from the menu button
		/// </summary>
		private void btn_RunDisabledTasks_Click(object sender, EventArgs e)
		{
			RunDisabledTasks();
		}

		/// <summary>
		/// Create new tasks from the menu button
		/// </summary>		
		private void btn_CreateTask_Click(object sender, EventArgs e)
		{
			CreateNewTask();
		}

		/// <summary>
		/// Run manually task/tasks from the menu button
		/// </summary>
		private void btn_RunManuallyTasks_Click(object sender, EventArgs e)
		{
			RunManuallyTasks();
		}

		/// <summary>
		/// Refresh the current view from the menu button
		/// </summary>
		private void btn_RefreshView_Click(object sender, EventArgs e)
		{
			GetBackupTasks();
		}

		/// <summary>
		/// Open Windows Tasks Scheduler from the menu button
		/// </summary>
		private void btn_OpenWindowsTaskScheduler_Click(object sender, EventArgs e)
		{
			OpenWindowsTaskScheduler();
		}

		/// <summary>
		/// Open Help page from community.sdl.com web site from the menu button
		/// </summary>
		private void btn_Help_Click(object sender, EventArgs e)
		{
			OpenReadMe();
		}
		#endregion
	}
}