using System.Collections.Generic;
using System.Linq;

namespace Reports.Viewer.Api.Providers
{
	public class TaskTemplateIdProvider
	{
		private readonly Dictionary<string, string> _templates;

		public TaskTemplateIdProvider()
		{
			_templates = new Dictionary<string, string>
			{
				{"analyse", "Sdl.ProjectApi.AutomaticTasks.Analysis"},
				{"translate", "Sdl.ProjectApi.AutomaticTasks.Translate"},
				{"wordcount", "Sdl.ProjectApi.AutomaticTasks.WordCount"},
				{"Verify Files", "Sdl.ProjectApi.AutomaticTasks.Verification"},
				{"translation quality assessment", "Sdl.ProjectApi.AutomaticTasks.Feedback"},
				{"translationcount", "Sdl.ProjectApi.AutomaticTasks.TranslationCount"},
				{"PerfectMatch", "Sdl.ProjectApi.AutomaticTasks.PerfectMatch"},
				{"WIPReport", "WIP Report"}
			};
		}

		public void AddTemplateId(string name, string templateId)
		{
			if (!string.IsNullOrEmpty(name) && !_templates.Keys.Contains(name) &&
			    !TaskTemplateIdExists(templateId))
			{
				_templates.Add(name, templateId);
			}
		}

		public string GetTaskTemplateId(string name)
		{
			return _templates.ContainsKey(name) ? _templates[name] : null;
		}

		public bool TaskTemplateIdExists(string templateId)
		{
			return !string.IsNullOrEmpty(templateId) && _templates.Values.Contains(templateId);
		}
	}
}
