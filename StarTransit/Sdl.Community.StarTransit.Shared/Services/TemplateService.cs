using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class TemplateService : AbstractViewControllerAction<ProjectsController>
	{
		public static readonly Log Log = Log.Instance;

		protected override void Execute()
		{

		}

		public List<ProjectTemplateInfo> LoadProjectTemplates()
		{
			try
			{
				var controller = Controller;
				var templateList = controller.GetProjectTemplates().OrderBy(t => t.Name).ToList();

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