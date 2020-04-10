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
		private ProjectsController _projectsController;

		public static readonly Log Log = Log.Instance;

		public TemplateService()
		{
			_projectsController = Extensions.GetProjectsController();
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