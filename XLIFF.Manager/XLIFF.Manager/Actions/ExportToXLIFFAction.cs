using System.Linq;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.XLIFF.Manager.Actions
{

	[Action("XLIFFManager_ExportToXLIFF_Action", typeof(XLIFFManagerViewController),
		Name = "XLIFFManager_ExportToXLIFF_Name",
		Icon = "ExportTo",
		Description = "XLIFFManager_ExportToXLIFF_Description")]
	[ActionLayout(typeof(XLIFFManagerActionsGroup), 6, DisplayType.Large)]
	public class ExportToXLIFFAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		private ProjectsController _projectsController;
		private FilesController _filesController;

		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export, _projectsController, _filesController);
			wizardService.ShowWizard();
		}		

		public override void Initialize()
		{
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			Enabled = true;
		}			
	}

	[Action("XLIFFManager_FilesContextMenu_ExportToXLIFF_Action", typeof(FilesController),
		Name = "XLIFFManager_ContextMenu_ExportToXLIFF_Name",
		Icon = "ExportTo",
		Description = "XLIFFManager_ContextMenu_ExportToXLIFF_Description")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 8, DisplayType.Default, "", true)]
	public class XLIFFManagerFilesContextMenuExportToXLIFFAction : AbstractAction
	{
		private ProjectsController _projectsController;
		private FilesController _filesController;

		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export, _projectsController, _filesController);
			wizardService.ShowWizard();
		}

		public override void Initialize()
		{
			SetProjectsController();
			SetFilesController();
			SetEnabled();
		}

		private void SetProjectsController()
		{
			if (_projectsController != null)
			{
				_projectsController.SelectedProjectsChanged -= ProjectsController_SelectedProjectsChanged;
			}

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			if (_projectsController != null)
			{
				_projectsController.SelectedProjectsChanged += ProjectsController_SelectedProjectsChanged;
			}
		}

		private void SetFilesController()
		{
			if (_filesController != null)
			{
				_filesController.SelectedFilesChanged -= FilesController_SelectedFilesChanged;
			}

			_filesController = SdlTradosStudio.Application.GetController<FilesController>();

			if (_filesController != null)
			{
				_filesController.SelectedFilesChanged += FilesController_SelectedFilesChanged;
			}
		}

		private void FilesController_SelectedFilesChanged(object sender, System.EventArgs e)
		{
			SetEnabled();
		}

		private void ProjectsController_SelectedProjectsChanged(object sender, System.EventArgs e)
		{
			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = _projectsController.SelectedProjects.Count() == 1 &&
			          _filesController.SelectedFiles.Any();
		}
	}

	[Action("XLIFFManager_ProjectsContextMenu_ExportToXLIFF_Action", typeof(ProjectsController),
		Name = "XLIFFManager_ContextMenu_ExportToXLIFF_Name",
		Icon = "ExportTo",
		Description = "XLIFFManager_ContextMenu_ExportToXLIFF_Description")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 8, DisplayType.Default, "", true)]
	public class XLIFFManagerProjectsContextMenuExportToXLIFFAction : AbstractAction
	{
		private ProjectsController _projectsController;
		private FilesController _filesController;

		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export, _projectsController, _filesController);
			wizardService.ShowWizard();
		}

		public override void Initialize()
		{
			SetProjectsController();
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			SetEnabled();
		}

		private void SetProjectsController()
		{
			if (_projectsController != null)
			{
				_projectsController.SelectedProjectsChanged -= ProjectsController_SelectedProjectsChanged;
			}

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			if (_projectsController != null)
			{
				_projectsController.SelectedProjectsChanged += ProjectsController_SelectedProjectsChanged;
			}
		}

		private void ProjectsController_SelectedProjectsChanged(object sender, System.EventArgs e)
		{
			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = _projectsController.SelectedProjects.Count() == 1;
		}
	}
}
