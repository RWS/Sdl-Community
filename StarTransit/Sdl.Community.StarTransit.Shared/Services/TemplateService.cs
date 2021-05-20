//using System.Collections.Generic;
//using System.Linq;
//using Sdl.Community.StarTransit.Shared.Utils;
//using Sdl.ProjectAutomation.Core;
//using Sdl.TranslationStudioAutomation.IntegrationApi;

////TODO:Remove this for the final implementation
//namespace Sdl.Community.StarTransit.Shared.Services
//{
//	public class TemplateService
//	{
//		private readonly ProjectsController _projectsController;

//		public TemplateService()
//		{
//			var helpers = new Helpers();
//			_projectsController = helpers.GetProjectsController();
//		}

//		public List<ProjectTemplateInfo> LoadProjectTemplates()
//		{
//			var templateList = _projectsController?.GetProjectTemplates()?.OrderBy(t => t.Name).ToList();
//			return templateList ?? new List<ProjectTemplateInfo>();
//		}
//	}
//}