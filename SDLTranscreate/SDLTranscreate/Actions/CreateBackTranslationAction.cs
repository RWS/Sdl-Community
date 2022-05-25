using System;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using NLog;
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
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;

namespace Trados.Transcreate.Actions
{
	[Action("TranscreateManager_CreateBackTranslationProject_Action",
		Name = "TranscreateManager_CreateBackTranslationProject_Name",
		Description = "TranscreateManager_CreateBackTranslationProject_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "back_translation_small"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 3, DisplayType.Large)]
	public class CreateBackTranslationAction : AbstractViewControllerAction<TranscreateViewController>
	{
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private IDialogService _dialogService;
		private SegmentBuilder _segmentBuilder;
		private ProjectAutomationService _projectAutomationService;
		private ProjectSettingsService _projectSettingsService;
		private Controllers _controllers;
		private StudioVersionService _studioVersionService;

		protected override void Execute()
		{
			var projects = _controllers.TranscreateController.GetSelectedProjects();
			if (projects?.Count != 1)
			{
				Enabled = false;
				return;
			}

			var project = projects[0];
			if (project is BackTranslationProject)
			{
				return;
			}
			
			var studioProject = _controllers.ProjectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);
			if (studioProject == null)
			{
				return;
			}

			var documents = _controllers.EditorController.GetDocuments()?.ToList();
			if (documents != null && documents.Count > 0)
			{
				var documentProjectIds = documents.Select(a => a.Project.GetProjectInfo().Id.ToString()).Distinct();
				if (documentProjectIds.Any(a => a == project.Id))
				{
					MessageBox.Show(PluginResources.Wanring_Message_CloseAllProjectDocumentBeforeProceeding, PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
			}

			var action = Enumerators.Action.CreateBackTranslation;
			var workFlow = Enumerators.WorkFlow.Internal;

			var settings = GetSettings();
			settings.ExportOptions.IncludeBackTranslations = true;
			settings.ExportOptions.IncludeTranslations = true;
			settings.ExportOptions.CopySourceToTarget = false;

			settings.ImportOptions.OverwriteTranslations = true;
			settings.ImportOptions.OriginSystem = Constants.OriginSystem_TranscreateAutomation;
			settings.ImportOptions.StatusTranslationUpdatedId = string.Empty;

			var wizardService = new WizardService(action, workFlow, _pathInfo, _customerProvider,
				_imageService, _controllers, _segmentBuilder, settings, _dialogService,
				_projectAutomationService, _projectSettingsService);

			try
			{
				_controllers.TranscreateController.IgnoreProjectChanged = true;

				var taskContext = wizardService.ShowWizard(Controller, out var message);
				if (taskContext == null && !string.IsNullOrEmpty(message))
				{
					LogManager.GetCurrentClassLogger().Warn(message);
					MessageBox.Show(message,
						PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Error(ex);
				MessageBox.Show(ex.Message,
					PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
			}
			finally
			{
				_controllers.TranscreateController.IgnoreProjectChanged = false;
			}
		}

		public void Run()
		{
			Execute();
		}

		public override void Initialize()
		{
			Enabled = false;

			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_dialogService = new DialogService();
			_imageService = new ImageService();
			_segmentBuilder = new SegmentBuilder();
			_controllers = SdlTradosStudio.Application.GetController<TranscreateViewController>().Controllers;
			_studioVersionService = new StudioVersionService();
			_projectAutomationService = new ProjectAutomationService(
				_imageService, _controllers.TranscreateController, _controllers.ProjectsController, _customerProvider, _studioVersionService);
			
			_projectSettingsService = new ProjectSettingsService();
			
			_controllers.TranscreateController.ProjectSelectionChanged += ProjectsController_SelectedProjectsChanged;

			var projects = _controllers?.TranscreateController?.GetSelectedProjects();
			SetEnabled(projects?[0]);
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

		private void ProjectsController_SelectedProjectsChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			SetEnabled(e.SelectedProject);
		}

		private void SetEnabled(IProject selectedProject)
		{
			Enabled = selectedProject is Project && !(selectedProject is BackTranslationProject);
		}
	}
}
