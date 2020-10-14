using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.Interfaces;
using Sdl.Community.Transcreate.LanguageMapping;
using Sdl.Community.Transcreate.LanguageMapping.Interfaces;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Transcreate.Actions
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
		private ILanguageProvider _languageProvider;
		private ProjectAutomationService _projectAutomationService;

		protected override void Execute()
		{
			if (!_controllers.TranscreateController.IsActive)
			{
				return;
			}

			var selectedProject = _controllers.TranscreateController.GetSelectedProjects()?.FirstOrDefault();
			if (selectedProject == null)
			{
				return;
			}

			var action = selectedProject.IsBackTranslationProject
				? Enumerators.Action.ImportBackTranslation
				: Enumerators.Action.Import;
			var workFlow = Enumerators.WorkFlow.External;

			var settings = GetSettings();
			var wizardService = new WizardService(action, workFlow, _pathInfo, _customerProvider,
				_imageService, _controllers, _segmentBuilder, settings, _dialogService, _languageProvider,
				_projectAutomationService);

			var taskContext = wizardService.ShowWizard(_controllers.TranscreateController, out var message);
			if (taskContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message, PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			_controllers.TranscreateController.UpdateProjectData(taskContext);
		}

		public void LaunchWizard()
		{
			Execute();
		}

		public override void Initialize()
		{
			_controllers = new Controllers();
			SetupTranscreateController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_dialogService = new DialogService();
			_segmentBuilder = new SegmentBuilder();
			_languageProvider = new LanguageProvider(_pathInfo);
			_projectAutomationService = new ProjectAutomationService(_imageService, _controllers.TranscreateController, _customerProvider);

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
