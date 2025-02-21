using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Trados.Transcreate.Common;
using Trados.Transcreate.CustomEventArgs;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.Model;
using Trados.Transcreate.Service;
using Sdl.Versioning;

namespace Trados.Transcreate.Actions
{
	[Action("TranscreateManager_CreateReport_Action",
		Name = "TranscreateManager_CreateReport_Name",
		Description = "TranscreateManager_CreateReport_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "Report"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 2, DisplayType.Large)]
	public class CreateReport : AbstractViewControllerAction<TranscreateViewController>
	{
		private Settings _settings;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private SegmentBuilder _segmentBuilder;
		private ProjectAutomationService _projectAutomationService;
		private Controllers _controllers;
		private StudioVersionService _studioVersionService;

		protected override void Execute()
		{
			var selectedFiles = _controllers.TranscreateController.GetSelectedProjectFiles();
			Run(selectedFiles);
		}

		private void CreateReports(List<ProjectFile> selectedFiles)
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

			var reportService = new ReportService(_pathInfo, _projectAutomationService, _segmentBuilder);
			
			var reports = reportService.CreateFinalReport(project, studioProject, selectedFiles, out var workingPath);
			if (reports.Count > 0)
			{
				_controllers.TranscreateController.ReportsController.AddReports(reports);

				var dr = MessageBox.Show(PluginResources.Message_TranscreateReportsCreatedSuccessfully
				                         + Environment.NewLine + Environment.NewLine +
				                         PluginResources.Question_OpenFolderInExplorer,
					PluginResources.Plugin_Name, MessageBoxButtons.YesNo);

				if (dr == DialogResult.Yes)
				{
					if (Directory.Exists(workingPath))
					{
						System.Diagnostics.Process.Start("explorer.exe", workingPath);
					}
				}
			}
		}

		public void Run(List<ProjectFile> selectedFiles)
		{
			CreateReports(selectedFiles);
		}

		public bool IsEnabled()
		{
			return Enabled;
		}

		public override void Initialize()
		{
			Enabled = false;

			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_settings = GetSettings();
			_segmentBuilder = new SegmentBuilder();
			_controllers = SdlTradosStudio.Application.GetController<TranscreateViewController>().Controllers;
			_studioVersionService = new StudioVersionService();
			_projectAutomationService = new ProjectAutomationService(
				_imageService, _controllers.TranscreateController, _controllers.ProjectsController, _customerProvider, _studioVersionService);
			
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

		private void SetEnabled(Interfaces.IProject selectedProject)
		{
			if (selectedProject == null ||
				selectedProject is BackTranslationProject)
			{
				Enabled = false;
			}
			else
			{
				Enabled = true;
			}
		}
	}
}
