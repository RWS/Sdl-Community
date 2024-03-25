using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;
using Trados.Transcreate.Service;
using MessageBox = System.Windows.MessageBox;

namespace Trados.Transcreate.Actions
{
	[Action("TranscreateManager_ConvertProject_Action",
		Name = "TranscreateManager_ConvertProject_Name",
		Description = "TranscreateManager_ConvertProject_Description",
		ContextByType = typeof(ProjectsController),
		Icon = "Icon"
		)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 5, DisplayType.Default, "", true)]
	public class ConvertProjectAction : AbstractAction
	{
		private Controllers _controllers;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private IDialogService _dialogService;
		private SegmentBuilder _segmentBuilder;
		private ProjectAutomationService _projectAutomationService;
		private ProjectSettingsService _projectSettingsService;
		private StudioVersionService _studioVersionService;

		public Controllers Controllers
		{
			get
			{

				if (_controllers == null)
				{ 
					var fileController = SdlTradosStudio.Application.GetController<FilesController>();
					var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
					var editorController = SdlTradosStudio.Application.GetController<EditorController>();
					var transcreateViewController = SdlTradosStudio.Application.GetController<TranscreateViewController>();
					_controllers = new Controllers(projectsController, fileController, editorController, transcreateViewController);

					SetProjectsController();
				}


				return _controllers;
			}
		}

		protected override void Execute()
		{
			var selectedProject = Controllers.ProjectsController.SelectedProjects.FirstOrDefault();
			if (selectedProject == null)
			{
				return;
			}

			var documents = Controllers.EditorController.GetDocuments()?.ToList();
			if (documents != null && documents.Count > 0)
			{
				var documentProjectIds = documents.Select(a => a.Project.GetProjectInfo().Id.ToString()).Distinct();
				if (documentProjectIds.Any(a => a == selectedProject.GetProjectInfo().Id.ToString()))
				{
					MessageBox.Show(PluginResources.Wanring_Message_CloseAllProjectDocumentBeforeProceeding, PluginResources.TranscreateManager_Name, MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
			}

			// set the default settings for creating the xliff from the sdlxliff
			// these should not be taken from the users settings
			var settings = GetSettings();
			settings.ExportOptions.CopySourceToTarget = false;
			settings.ExportOptions.IncludeTranslations = true;
			settings.ExportOptions.ExcludeFilterIds = new List<string>();

			settings.ImportOptions.StatusTranslationUpdatedId = string.Empty;
			settings.ImportOptions.StatusSegmentNotImportedId = string.Empty;
			settings.ImportOptions.StatusTranslationNotUpdatedId = string.Empty;
			settings.ImportOptions.OverwriteTranslations = true;

			var action = Enumerators.Action.Convert;
			var workFlow = Enumerators.WorkFlow.Internal;


			var newProjectLocalFolder = selectedProject.GetProjectInfo().LocalProjectFolder + "-T";
			if (Directory.Exists(newProjectLocalFolder))
			{
				MessageBox.Show(PluginResources.Warning_Message_ProjectFolderAlreadyExists + Environment.NewLine + Environment.NewLine + newProjectLocalFolder,
					PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			if (selectedProject.GetProjectInfo().ProjectOrigin == Constants.ProjectOrigin_TranscreateProject)
			{
				MessageBox.Show(PluginResources.Warning_Message_ProjectAlreadyTranscreateProject,
					PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var wizardService = new WizardService(action, workFlow, _pathInfo, _customerProvider,
				_imageService, Controllers, _segmentBuilder, settings, _dialogService,
				_projectAutomationService, _projectSettingsService);

			var taskContext = wizardService.ShowWizard(Controllers.ProjectsController, out var message);
			if (taskContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			Controllers.TranscreateController.UpdateProjectData(taskContext);
		}


		public override void Initialize()
		{
			Enabled = false;

			//_controllers = SdlTradosStudio.Application.GetController<TranscreateViewController>().Controllers;
			//SetProjectsController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_dialogService = new DialogService();
			_segmentBuilder = new SegmentBuilder();
			_studioVersionService = new StudioVersionService();
			_projectAutomationService = new ProjectAutomationService(
				_imageService, Controllers.TranscreateController, Controllers.ProjectsController, _customerProvider, _studioVersionService);
			_projectSettingsService = new ProjectSettingsService();

			SetEnabled();
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

		private void SetProjectsController()
		{
			if (Controllers.ProjectsController != null)
			{
				Controllers.ProjectsController.SelectedProjectsChanged += ProjectsController_SelectedProjectsChanged;
			}
		}

		private void ProjectsController_SelectedProjectsChanged(object sender, System.EventArgs e)
		{
			SetEnabled();
		}

		private void SetEnabled()
		{
			if (Controllers.ProjectsController.SelectedProjects.Count() != 1)
			{
				Enabled = false;
				return;
			}

			var selectedProject = Controllers.ProjectsController.SelectedProjects.FirstOrDefault();

			Enabled = selectedProject?.GetProjectInfo().ProjectOrigin != Constants.ProjectOrigin_TranscreateProject;
		}
	}
}
