using Sdl.Community.Utilities.TMTool.Task;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Utilities.TMTool
{
	public class TasksManager
	{
		List<ITask> _tasks;

		/// <summary>
		/// initializes new tasks manager for list of ITask objects
		/// </summary>
		/// <param name="tasks">list of ITask objects</param>
		public TasksManager(List<ITask> tasks)
		{
			_tasks = tasks;
		}

		/// <summary>
		/// task that is currently selected by user
		/// </summary>
		public ITask SelectedTask
		{
			get;
			private set;
		}

		/// <summary>
		/// gets dictionary of all tasks file types
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetSupportedFileTypes()
		{
			List<string> valuesAdded = new List<string>();

			Dictionary<string, string> tasksFTypes = new Dictionary<string, string>();
			foreach (ITask task in _tasks)
				foreach (KeyValuePair<string, string> fType in task.SupportedFileTypes)
				{
					if (tasksFTypes.ContainsKey(fType.Key))
					{
						if (!valuesAdded.Contains(fType.Value))
						{
							tasksFTypes[fType.Key] = string.Format("{0}; {1}", tasksFTypes[fType.Key], fType.Value);
							valuesAdded.Add(fType.Value);
						}
					}
					else
					{
						tasksFTypes.Add(fType.Key, fType.Value);
						valuesAdded.Add(fType.Value);
					}
				}
			// 02.24.2011 fix
			// tasksFTypes = tasksFTypes.Union(task.SupportedFileTypes).Distinct().ToDictionary(pair => pair.Key, pair => pair.Value);

			return tasksFTypes;
		}
		/// <summary>
		/// gets dictionary of pairs: task id - task name
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetTaskNames()
		{
			Dictionary<string, string> tasksNames = new Dictionary<string, string>();
			foreach (ITask task in _tasks)
				if (!tasksNames.ContainsKey(task.TaskID.ToString()))
					tasksNames.Add(task.TaskID.ToString(), task.TaskName);

			return tasksNames;
		}

		/// <summary>
		/// get maximum control MinWidth property (among all the tasks)
		/// </summary>
		/// <returns></returns>
		public int GetMinWidth()
		{
			return _tasks
				.Where(t => t.Control != null && t.Control.UControl != null)
				.Max(w => w.Control.UControl.MinimumSize.Width);
		}
		/// <summary>
		/// get maximum control MinHeight property (among all the tasks)
		/// </summary>
		/// <returns></returns>
		public int GetMinHeight()
		{
			return _tasks
				.Where(t => t.Control != null && t.Control.UControl != null)
				.Max(w => w.Control.UControl.MinimumSize.Height);
		}

		/// <summary>
		/// sets new SelectedTask by task ID
		/// </summary>
		/// <param name="taskGuid">task unique identifier</param>
		public void SelectTask(string taskGuid)
		{
			SelectedTask = _tasks.Find(t => t.TaskID.ToString() == taskGuid);
		}
		/// <summary>
		/// gets IControl property of selected task
		/// </summary>
		/// <returns></returns>
		public IControl GetSelectedTaskControl()
		{
			if (SelectedTask != null)
				return SelectedTask.Control;

			return null;
		}
		/// <summary>
		/// gets ISettings property of selected task
		/// </summary>
		/// <returns></returns>
		public ISettings GetSelectedTaskSettings()
		{
			IControl currControl = GetSelectedTaskControl();
			if (currControl != null)
			{
				return currControl.Options;
			}

			return null;
		}
	}
}