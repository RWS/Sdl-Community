using System.Collections.Generic;
using Sdl.Community.Transcreate.Common;

namespace Sdl.Community.Transcreate.Model
{
	public class TaskResult
	{
		public TaskResult(Enumerators.Action action, Enumerators.WorkFlow workFlow)
		{
			Action = action;
			WorkFlow = workFlow;
		}
		
		public Enumerators.Action Action { get; }

		public Enumerators.WorkFlow WorkFlow { get; }

		public bool Completed { get; set; }

		private List<TaskContext> TaskContexts { get; set; }
	}
}
