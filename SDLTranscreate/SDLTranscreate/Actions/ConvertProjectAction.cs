using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.Interfaces;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using MessageBox = System.Windows.MessageBox;

namespace Sdl.Community.Transcreate.Actions
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

		protected override void Execute()
		{
			// set the default settings for creating the xliff from the sdlxliff
			// these should not be taken from the users settings
			var settings = GetSettings();
			settings.ExportOptions.CopySourceToTarget = false;
			settings.ExportOptions.IncludeTranslations = true;
			settings.ExportOptions.ExcludeFilterIds = new List<string>();

			settings.ImportOptions.StatusTranslationUpdatedId = "Translated";
			settings.ImportOptions.StatusSegmentNotImportedId = string.Empty;
			settings.ImportOptions.StatusTranslationNotUpdatedId = string.Empty;
			settings.ImportOptions.OverwriteTranslations = true;

			var action = Enumerators.Action.Convert;
			var workFlow = Enumerators.WorkFlow.Internal;

			var wizardService = new WizardService(action, workFlow, _pathInfo, _customerProvider,
				_imageService, _controllers, _segmentBuilder, settings, _dialogService, 
				_projectAutomationService);

			var taskContext = wizardService.ShowWizard(_controllers.ProjectsController, out var message);
			if (taskContext == null && !string.IsNullOrEmpty(message))
			{
				MessageBox.Show(message, PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			_controllers.TranscreateController.UpdateProjectData(taskContext);
		}
	

		public override void Initialize()
		{
			Enabled = false;


			_controllers = SdlTradosStudio.Application.GetController<TranscreateViewController>().Controllers;
			SetProjectsController();
			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_dialogService = new DialogService();
			_segmentBuilder = new SegmentBuilder();
			_projectAutomationService = new ProjectAutomationService(_imageService, _controllers.TranscreateController, 
				_controllers.ProjectsController, _customerProvider);

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
			if (_controllers.ProjectsController != null)
			{
				_controllers.ProjectsController.SelectedProjectsChanged += ProjectsController_SelectedProjectsChanged;
			}
		}

		private void ProjectsController_SelectedProjectsChanged(object sender, System.EventArgs e)
		{
			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = _controllers.ProjectsController.SelectedProjects.Count() == 1;
		}
	}
}
