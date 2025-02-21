using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model.Tasks
{
	public class Report
	{
		private Enumerators.Action _action;

		public Report(Enumerators.Action action)
		{
			_action = action;

			Guid = System.Guid.NewGuid().ToString();
			TaskTemplateId = "XLIFF.Manager.BatchTasks."+ action;
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
