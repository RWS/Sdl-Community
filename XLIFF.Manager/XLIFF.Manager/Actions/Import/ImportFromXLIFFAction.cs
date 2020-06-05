using System.Windows;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.CustomEventArgs;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.Actions.Import
{
	[Action("XLIFFManager_ImportFromXLIFF_Action", typeof(XLIFFManagerViewController),
		Name = "XLIFFManager_ImportFromXLIFF_Name",
		Icon = "ImportFrom",
		Description = "XLIFFManager_ImportFromXLIFF_Description")]
	[ActionLayout(typeof(XLIFFManagerActionsGroup), 5, DisplayType.Large)]
	public class ImportFromXLIFFAction : AbstractViewControllerAction<XLIFFManagerViewController>
	{
		private ProjectsController _projectsController;
		private FilesController _filesController;
		private XLIFFManagerViewController _xliffManagerController;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private SegmentBuilder _segmentBuilder;
		private IDialogService _dialogService;

		protected override void Execute()
		{			
			var wizardService = new WizardService(Enumerators.Action.Import, _pathInfo, _customerProvider,
				_imageService, _xliffManagerController, _projectsController, _filesController,
				_segmentBuilder, _dialogService);
			var wizardContext = wizardService.ShowWizard(_xliffManagerController, out var message);
			if (wizardContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message);
				return;
			}

			_xliffManagerController.UpdateProjectData(wizardContext);
		}

		public override void Initialize()
		{
			SetupXliffManagerController();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_dialogService = new DialogService();
			_segmentBuilder = new SegmentBuilder();

			Enabled = true;
		}

		private void SetupXliffManagerController()
		{
			_xliffManagerController = SdlTradosStudio.Application.GetController<XLIFFManagerViewController>();
			_xliffManagerController.ProjectSelectionChanged += OnProjectSelectionChanged;
		}

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			Enabled = e.SelectedProject != null;
		}
	}
}
