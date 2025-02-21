using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.CustomEventArgs;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;
using Trados.Transcreate.Service;
using MessageBox = System.Windows.MessageBox;

namespace Trados.Transcreate.Actions
{
	[Action("TranscreateManager_Import_Action",
		Name = "TranscreateManager_Import_Name",
		Description = "TranscreateManager_Import_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "Import"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 4, DisplayType.Large)]
	public class ImportAction : AbstractViewControllerAction<TranscreateViewController>
	{
		private Controllers _controllers;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private SegmentBuilder _segmentBuilder;
		private IDialogService _dialogService;
		private ProjectAutomationService _projectAutomationService;
		private ProjectSettingsService _projectSettingsService;
		private StudioVersionService _studioVersionService;

		protected override void Execute()
		{
			var selectedProject = _controllers.TranscreateController.GetSelectedProjects()?.FirstOrDefault();
			if (selectedProject == null)
			{
				return;
			}

			var documents = _controllers.EditorController.GetDocuments()?.ToList();
			if (documents != null && documents.Count > 0)
			{
				var documentProjectIds = documents.Select(a => a.Project.GetProjectInfo().Id.ToString()).Distinct();
				if (documentProjectIds.Any(a => a == selectedProject.Id))
				{
					MessageBox.Show(PluginResources.Wanring_Message_CloseAllProjectDocumentBeforeProceeding, PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
			}

			var action = selectedProject is BackTranslationProject
				? Enumerators.Action.ImportBackTranslation
				: Enumerators.Action.Import;
			var workFlow = Enumerators.WorkFlow.External;

			var settings = GetSettings();
			var wizardService = new WizardService(action, workFlow, _pathInfo, _customerProvider,
				_imageService, _controllers, _segmentBuilder, settings, _dialogService,
				_projectAutomationService, _projectSettingsService);

			var taskContext = wizardService.ShowWizard(_controllers.TranscreateController, out var message);
			if (taskContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message, PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			if (selectedProject is BackTranslationProject)
			{
				var projects = _controllers.TranscreateController.GetProjects();
				var parentProject = projects.FirstOrDefault(project => project.BackTranslationProjects.Exists(a => a.Id == selectedProject.Id));

				_controllers.TranscreateController.UpdateBackTranslationProjectData(parentProject?.Id, taskContext);
			}
			else
			{
				_controllers.TranscreateController.UpdateProjectData(taskContext);
			}
		}

		public void LaunchWizard()
		{
			Execute();
		}

		public override void Initialize()
		{
            ApplicationInstance.InitializeTranscreateViewController();

            _controllers = SdlTradosStudio.Application.GetController<TranscreateViewController>().Controllers;
			SetupTranscreateController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_dialogService = new DialogService();
			_segmentBuilder = new SegmentBuilder();
			_studioVersionService = new StudioVersionService();
			_projectAutomationService = new ProjectAutomationService(
				_imageService, _controllers.TranscreateController, _controllers.ProjectsController, _customerProvider, _studioVersionService);
			_projectSettingsService = new ProjectSettingsService();

			Enabled = false;
		}

		private Settings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		private void SetupTranscreateController()
		{
			_controllers.TranscreateController.ProjectSelectionChanged += OnProjectSelectionChanged;
		}

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			Enabled = e.SelectedProject != null;
		}
	}
}
