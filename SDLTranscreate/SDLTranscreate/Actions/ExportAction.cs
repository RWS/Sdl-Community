using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.Interfaces;
using Sdl.Community.Transcreate.LanguageMapping;
using Sdl.Community.Transcreate.LanguageMapping.Interfaces;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

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
		private ILanguageProvider _languageProvider;
		private ProjectAutomationService _projectAutomationService;

		protected override void Execute()
		{

			//TODO Testing
			//var selectedFile = _controllers.TranscreateController.GetSelectedProjectFiles()[0];

			//var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			//var tokenVisitor = new TokenVisitor();
			//var settings = new GeneratorSettings();
			//var project = _controllers.ProjectsController.GetAllProjects().ToList()
			//	.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == selectedFile.ProjectId);
			//var analysisBands = _projectAutomationService.GetAnalysisBands(project);
			//var process = new Processor(fileTypeManager, tokenVisitor, settings, analysisBands);

			//var fullPath = Path.Combine(selectedFile.Project.Path, selectedFile.Location);

			//process.ExportFile(fullPath);




			var settings = GetSettings();

			var wizardService = new WizardService(Enumerators.Action.Export, _pathInfo, _customerProvider,
				_imageService, _controllers, _segmentBuilder, settings, _dialogService, _languageProvider,
				_projectAutomationService);

			var wizardContext = wizardService.ShowWizard(Controller, out var message);
			if (wizardContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message, PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			_controllers.TranscreateController.UpdateProjectData(wizardContext);
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
			//Enabled = e.SelectedProject != null;
		}
	}
}
