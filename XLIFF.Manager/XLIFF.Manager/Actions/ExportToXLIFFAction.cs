using System.Linq;
using System.Windows;
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
		private XLIFFManagerViewController _xliffManagerController;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;

		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export, _pathInfo, _customerProvider,
				 _imageService, _xliffManagerController, _projectsController, _filesController);
			var wizardContext = wizardService.ShowWizard(Controller, out var message);

			if (wizardContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message);
				return;
			}

			_xliffManagerController.UpdateProjectData(wizardContext);
		}

		public override void Initialize()
		{
			_xliffManagerController = SdlTradosStudio.Application.GetController<XLIFFManagerViewController>();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService(_pathInfo);


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
		private XLIFFManagerViewController _xliffManagerController;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;

		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export, _pathInfo, _customerProvider,
				_imageService, _xliffManagerController, _projectsController, _filesController);
			var wizardContext = wizardService.ShowWizard(_filesController, out var message);
			if (wizardContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message);
				return;
			}

			_xliffManagerController.UpdateProjectData(wizardContext);
		}

		public override void Initialize()
		{
			_xliffManagerController = SdlTradosStudio.Application.GetController<XLIFFManagerViewController>();
			SetProjectsController();
			SetFilesController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService(_pathInfo);

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
			Enabled = _filesController.SelectedFiles.Any();
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
		private XLIFFManagerViewController _xliffManagerController;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;

		protected override void Execute()
		{
			var wizardService = new WizardService(Enumerators.Action.Export, _pathInfo, _customerProvider,
				_imageService, _xliffManagerController, _projectsController, _filesController);
			var wizardContext = wizardService.ShowWizard(_projectsController, out var message);
			if (wizardContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message);
				return;
			}

			_xliffManagerController.UpdateProjectData(wizardContext);
		}

		public override void Initialize()
		{
			_xliffManagerController = SdlTradosStudio.Application.GetController<XLIFFManagerViewController>();
			SetProjectsController();
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService(_pathInfo);
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
