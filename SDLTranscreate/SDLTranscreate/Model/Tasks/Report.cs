using Sdl.Community.Transcreate.Common;

namespace Sdl.Community.Transcreate.Model.Tasks
{
	public class Report
	{
		public Report(Enumerators.Action action)
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
			TaskTemplateId = "Transcreate.BatchTasks." + actionName;
		}

		public string Guid { get; set; }

		//Pre-translate Files Report
		public string Name { get; set; }

		//Results of applying the translation memories to the files.
		public string Description { get; set; }
		
		public string TaskTemplateId { get; set; }

		public string LanguageDirectionGuid { get; set; }

		//Reports\Pre-translate Files en-US_de-de.xml
		public string PhysicalPath { get; set; }
	}
}
