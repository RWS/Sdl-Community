using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class TemplateService
	{
		private readonly ProjectsController _projectsController;

		public TemplateService()
		{
			var helpers = new Helpers();
			_projectsController = helpers.GetProjectsController();
		}

		public List<ProjectTemplateInfo> LoadProjectTemplates()
		{
			try
			{
				var templateList = _projectsController?.GetProjectTemplates().OrderBy(t => t.Name).ToList();
				return templateList;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"MedatataBuilder method: {ex.Message}\n {ex.StackTrace}");
			}
			return new List<ProjectTemplateInfo>();
		}
	}
}