using System.Collections.Generic;
using Sdl.Community.Transcreate.Common;

namespace Sdl.Community.Transcreate.Model.Tasks
{
	public class AutomaticTask
	{
		public AutomaticTask(Enumerators.Action action)
		{
			var actionName = action.ToString();
			switch (action)
			{
				case Enumerators.Action.Export:
				case Enumerators.Action.ExportBackTranslation:
					actionName = "Export";
					break;
				case Enumerators.Action.Import:
				case Enumerators.Action.ImportBackTranslation:
					actionName = "Import";
					break;
			}

			Guid = System.Guid.NewGuid().ToString();
			Status = "Completed";
			PredecessorTaskGuid = "00000000-0000-0000-0000-000000000000";
			PercentComplete = "100";
			TemplateIds = new List<TemplateId> {new TemplateId {Value = "Transcreate.BatchTasks." + actionName } };
			TaskFiles = new List<TaskFile>();
			OutputFiles = new List<OutputFile>();
			Reports = new List<Report>();
		}

		public string Guid { get; set; }

		public string Status { get; set; }

		public string PredecessorTaskGuid { get; set; }

		public string CreatedAt { get; set; }

		public string StartedAt { get; set; }

		public string CompletedAt { get; set; }

		public string CreatedBy { get; set; }

		public string PercentComplete { get; set; }

		public List<TemplateId> TemplateIds { get; set; }

		public List<TaskFile> TaskFiles { get; set; }

		public List<OutputFile> OutputFiles { get; set; }

		public List<Report> Reports { get; set; }
	}
}
