using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace ProjectController
{
	[ApplicationInitializer]
	public class ProjectControllerMethods : IApplicationInitializer
	{
		public void Execute()
		{
			//files controller
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();
			var activeProjectFromFiles = filesController.CurrentProject;
		}
	}


	[RibbonGroup("Test", Name = "Test project icon")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class ProjectTemplateRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TestProjIconAction", Icon = "", Name = "Test icon",
		Description = "")]
	[ActionLayout(typeof(ProjectTemplateRibbonGroup), 10, DisplayType.Large)]
	public class ProjectAction : AbstractAction
	{
		protected override void Execute()
		{

			var file = @"filepath";
			var converter = DefaultFileTypeManager.CreateInstance(true)
				.GetConverterToDefaultBilingual(file, file, null);
			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new FileProcessor()));
			converter.Parse();
			//projects controler
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var activeProject = projectsController?.CurrentProject;
		}
	}
}
