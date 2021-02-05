using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.Interfaces;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using MessageBox = System.Windows.MessageBox;

namespace Sdl.Community.Transcreate.Actions
{
	[Action("TranscreateManager_Export_Action",
		Name = "TranscreateManager_Export_Name",
		Description = "TranscreateManager_Export_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "Export"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 5, DisplayType.Large)]
	public class ExportAction : AbstractViewControllerAction<TranscreateViewController>
	{
		private Controllers _controllers;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private IDialogService _dialogService;
		private SegmentBuilder _segmentBuilder;
		private ProjectAutomationService _projectAutomationService;

		protected override void Execute()
		{
			var selectedProject = _controllers.TranscreateController.GetSelectedProjects()?.FirstOrDefault();
			if (selectedProject == null)
			{
				return;
			}

			var action = selectedProject is BackTranslationProject
				? Enumerators.Action.ExportBackTranslation
				: Enumerators.Action.Export;

			var workFlow = Enumerators.WorkFlow.External;

			var settings = GetSettings();
			var wizardService = new WizardService(action, workFlow, _pathInfo, _customerProvider,
				_imageService, _controllers, _segmentBuilder, settings, _dialogService,
				_projectAutomationService);

			var taskContext = wizardService.ShowWizard(Controller, out var message);
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
			_controllers = SdlTradosStudio.Application.GetController<TranscreateViewController>().Controllers;
			SetupTranscreateController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_dialogService = new DialogService();
			_segmentBuilder = new SegmentBuilder();
			_projectAutomationService = new ProjectAutomationService(_imageService, _controllers.TranscreateController,
				_controllers.ProjectsController, _customerProvider);

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
