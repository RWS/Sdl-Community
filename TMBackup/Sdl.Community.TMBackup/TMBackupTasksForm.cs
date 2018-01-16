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
			using (TaskService ts = new TaskService())
			{
				List<TaskDefinitionModel> tasks = new List<TaskDefinitionModel>();
				foreach (var task in ts.AllTasks)
				{
					if (task.Name.Contains("DailyScheduler"))
					{
						var triggerInfo = string.Empty;
						foreach (var trigger in task.Definition.Triggers)
						{
							triggerInfo = string.Format("Started at: '{0}'. After triggered, repeat every '{1}'", trigger.StartBoundary, trigger.Repetition.Interval);
						}

						tasks.Add(new TaskDefinitionModel
						{
							TaskName = task.Name,
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
			TMBackupForm tmBackupForm = new TMBackupForm(true);
			tmBackupForm.ShowDialog();
		}
	}
}
