using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;

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
							Interval = triggerInfo
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
	}
}