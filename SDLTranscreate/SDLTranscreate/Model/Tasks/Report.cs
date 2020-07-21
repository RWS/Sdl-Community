using Sdl.Community.Transcreate.Common;

namespace Sdl.Community.Transcreate.Model.Tasks
{
	public class Report
	{
		private Enumerators.Action _action;

		public Report(Enumerators.Action action)
		{
			_action = action;

			Guid = System.Guid.NewGuid().ToString();
			TaskTemplateId = "Transcreate.BatchTasks." + action;
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
