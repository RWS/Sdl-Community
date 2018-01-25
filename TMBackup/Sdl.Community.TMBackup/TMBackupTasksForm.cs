using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Sdl.Community.BackupService;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupTasksForm : Form
	{
		public TMBackupTasksForm()
		{
			InitializeComponent();
			GetBackupTasks();
		}

		public void GetBackupTasks()
		{
			using (var ts = new TaskService())
			{
				var tasks = new List<TaskDefinitionModel>();
				foreach (var task in ts.AllTasks)
				{
					if (task.Name.Contains(Constants.TaskDetailValue))
					{
						var triggerInfo = string.Empty;
						foreach (var trigger in task.Definition.Triggers)
						{
							triggerInfo = string.Format("Started at: '{0}'. After triggered, repeat every '{1}'", trigger.StartBoundary, trigger.Repetition.Interval);
						}

						var index = task.Name.IndexOf(" ") + 1;
						var taskName = task.Name.Substring(index);

						tasks.Add(new TaskDefinitionModel
						{
							TaskName = taskName,
							LastRun = task.LastRunTime,
							NextRun = task.NextRunTime,
							Interval = triggerInfo,
							Status = task.State.ToString()
						});
					}
				}
				dataGridView1.DataSource = tasks;
			}
		}

		private void createNewBackupAction_Click(object sender, EventArgs e)
		{
			Hide();

			var tmBackupForm = new TMBackupForm(true, string.Empty);
			tmBackupForm.ShowDialog();
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			var dataIndexNo = dataGridView1.Rows[e.RowIndex].Index.ToString();
			var cellValue = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

			Hide();

			var tmBackupForm = new TMBackupForm(false, cellValue);
			tmBackupForm.ShowDialog();
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
						var task = ts.AllTasks.Where(t => t.Name.Contains(selectedRow.Cells[0].Value.ToString())).FirstOrDefault();
						if (task != null)
						{
							ts.RootFolder.DeleteTask(task.Name);

							var index = task.Name.IndexOf(" ") + 1;
							var jsonTaskName = task.Name.Substring(index);

							Persistence persistence = new Persistence();
							persistence.RemoveDataFromJson(jsonTaskName);
						}
					}
					GetBackupTasks();
				}
			}
		}

		// Display the context menu with the 'Delete' option for the tasks
		private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
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

		private void btn_Refresh_Click(object sender, EventArgs e)
		{
			GetBackupTasks();
		}		
	}
}