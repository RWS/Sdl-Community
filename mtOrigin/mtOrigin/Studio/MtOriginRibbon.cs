using System.Linq;
using System.Windows.Forms;
using Sdl.Community.mtOrigin.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.mtOrigin.Studio
{
	[Action("mtOriginRemoveNMT", Name = "NMT -> AT",Icon= "mtIco")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class NmtOriginRibbon : AbstractAction
	{
		protected override void Execute()
		{
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			if (projectsController != null && projectsController.SelectedProjects.Any())
			{
				var processFileService = new ProcessFileService();
				processFileService.RemoveTranslationOrigin(projectsController.SelectedProjects.ToList(), "mt");

				MessageBox.Show(@"Operation is completed");
			}
		}
	}

	[Action("mtOriginRemoveMT", Name = "AT -> NMT", Icon = "mtIco")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MtOriginRibbon : AbstractAction
	{
		protected override void Execute()
		{
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			if (projectsController != null && projectsController.SelectedProjects.Any())
			{
				var processFileService = new ProcessFileService();
				processFileService.RemoveTranslationOrigin(projectsController.SelectedProjects.ToList(), "nmt");
				projectsController.RefreshProjects();
				MessageBox.Show(@"Operation is completed");
			}
		}
	}
}
