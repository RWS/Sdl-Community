using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Newtonsoft.Json;
using Reports.Viewer.Api;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Api.Providers;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.Controls;
using Trados.Transcreate.CustomEventArgs;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.FileTypeSupport.XLIFF.Writers;
using Trados.Transcreate.Interfaces;
using Trados.Transcreate.Model;
using Trados.Transcreate.Model.ProjectSettings;
using Trados.Transcreate.Service;
using Trados.Transcreate.View;
using Trados.Transcreate.ViewModel;
using Constants = Trados.Transcreate.Common.Constants;
using IProject = Trados.Transcreate.Interfaces.IProject;
using LanguageDirectionInfo = Trados.Transcreate.Model.LanguageDirectionInfo;
using PathInfo = Trados.Transcreate.Common.PathInfo;
using ProjectFile = Trados.Transcreate.Model.ProjectFile;

namespace Trados.Transcreate
{
	[View(
		Id = "TranscreateManager_View",
		Name = "TranscreateManager_Name",
		Description = "TranscreateManager_Description",
		Icon = "Icon",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class TranscreateViewController : AbstractViewController, ITranscreateController
	{
		private readonly object _lockObject = new object();
		private List<IProject> _transcreateProjects;
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;
		private ProjectFilesView _projectFilesView;
		private ProjectsNavigationView _projectsNavigationView;
		private ProjectFileActivityViewController _projectFileActivityViewController;
		private ProjectsController _projectsController;
		private EditorController _editorController;
		private FilesController _filesController;
		private ImageService _imageService;
		private PathInfo _pathInfo;
		private CustomerProvider _customerProvider;
		private ProjectSettingsService _projectSettingsService;
		private ProjectAutomationService _projectAutomationService;
		private SegmentBuilder _segmentBuilder;
		private ReportService _reportService;
		private StudioVersionService _studioVersionService;


		private Reports.Viewer.Api.Model.PathInfo _reviewerPathInfo;
		private TaskTemplateIdProvider _taskTemplateIdProvider;

		protected override void Initialize(IViewContext context)
		{
			ClientId = Guid.NewGuid().ToString();

			ActivationChanged += OnActivationChanged;

			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			_projectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;
			_editorController.Opened += EditorController_Opened;
			_editorController.Closed += EditorController_Closed;

			_reviewerPathInfo = new Reports.Viewer.Api.Model.PathInfo();
			_taskTemplateIdProvider = new TaskTemplateIdProvider();
			ReportsController = new ReportsController(
				_projectsController.CurrentProject ?? _projectsController.SelectedProjects.FirstOrDefault(),
				_reviewerPathInfo, _taskTemplateIdProvider);


			Controllers = new Controllers(_projectsController, _filesController, _editorController, this);

			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_customerProvider = new CustomerProvider();
			_projectSettingsService = new ProjectSettingsService();
			_segmentBuilder = new SegmentBuilder();
			_studioVersionService = new StudioVersionService();
			_projectAutomationService = new ProjectAutomationService(
				_imageService, this, _projectsController, _customerProvider, _studioVersionService);
			_reportService = new ReportService(_pathInfo, _projectAutomationService, _segmentBuilder);

			LoadProjects();
		}


		public Controllers Controllers { get; private set; }

		protected override IUIControl GetExplorerBarControl()
		{
			if (_projectsNavigationView != null)
			{
				return _projectsNavigationView;
			}

			_projectsNavigationViewModel = new ProjectsNavigationViewModel(_projectsController,
				_editorController, _projectAutomationService);

			_projectsNavigationViewModel.ProjectSelectionChanged += OnProjectSelectionChanged;

			_projectsNavigationView = new ProjectsNavigationView();
			_projectsNavigationView.DataContext = _projectsNavigationViewModel;

			return _projectsNavigationView;
		}

		protected override IUIControl GetContentControl()
		{
			if (_projectFilesView != null)
			{
				return _projectFilesView;
			}

			_projectFilesViewModel = new ProjectFilesViewModel();
			_projectFilesView = new ProjectFilesView { DataContext = _projectFilesViewModel };

			_projectsNavigationViewModel.ProjectFilesViewModel = _projectFilesViewModel;
			_projectFilesViewModel.ProjectFileSelectionChanged += ProjectFilesViewModel_ProjectFileSelectionChanged;

			UpdateProjectSelectionFromProjectsController();
			_projectsNavigationViewModel.Projects = _transcreateProjects;

			return _projectFilesView;
		}

		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;

		public EventHandler<ProjectFileSelectionChangedEventArgs> ProjectFileSelectionChanged;

		public bool IsActive { get; set; }

		public bool IgnoreProjectChanged { get; set; }

		public bool OverrideEditorWarningMessage { get; set; }

		public ReportsController ReportsController { get; private set; }

		public string ClientId { get; private set; }

		public void RefreshProjects(bool force)
		{
			if (_projectsNavigationViewModel == null)
			{
				return;
			}

			var refresh = false;
			foreach (var project in _projectsController.GetAllProjects())
			{
				var addedNewProject = AddNewProjectToContainer(project);
				if (addedNewProject)
				{
					refresh = true;
				}
				else
				{
					var projectInfo = project.GetProjectInfo();
					var transcreateProject = _transcreateProjects.FirstOrDefault(a => a.Id == projectInfo.Id.ToString());
					if (transcreateProject != null)
					{
						var updatedCustomer = UpdateCustomerInfo(project, transcreateProject);
						var addedNewFiles = AddNewProjectFiles(project, transcreateProject);

						if (updatedCustomer || addedNewFiles)
						{
							refresh = true;
						}
					}
				}

				var unloadedExistingProject = UnloadRemovedProjectsFromContainer();
				if (unloadedExistingProject)
				{
					refresh = true;
				}
			}

			if (refresh || force)
			{
				_projectsNavigationViewModel.Projects = _transcreateProjects;
			}
		}

		public List<IProject> GetProjects()
		{
			return _transcreateProjects;
		}

		public List<IProject> GetSelectedProjects()
		{
			if (_projectsNavigationViewModel == null)
			{
				return null;
			}

			if (_projectsNavigationViewModel?.SelectedProjects != null)
			{
				return _projectsNavigationViewModel.SelectedProjects?.Cast<IProject>().ToList();
			}

			return new List<IProject> { _projectsNavigationViewModel.SelectedProject };
		}

		public List<ProjectFile> GetSelectedProjectFiles()
		{
			if (_projectFilesViewModel == null)
			{
				return new List<ProjectFile>();
			}

			if (_projectFilesViewModel.SelectedProjectFiles != null)
			{
				return _projectFilesViewModel.SelectedProjectFiles.Cast<ProjectFile>().ToList();
			}

			return new List<ProjectFile> { _projectFilesViewModel.SelectedProjectFile };
		}

		public void UpdateProjectData(TaskContext taskContext)
		{
			if (taskContext == null || !taskContext.Completed)
			{
				return;
			}

			var fileBasedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == taskContext.Project.Id);
			if (fileBasedProject == null)
			{
				return;
			}

			if (taskContext.Project.Id != _projectsController.CurrentProject?.GetProjectInfo().Id.ToString())
			{
				lock (_lockObject)
				{
					_projectAutomationService.ActivateProject(fileBasedProject);
				}
			}

			var sourceLanguage = taskContext.Project.SourceLanguage.CultureInfo.Name;
			taskContext.Project.ProjectFiles.RemoveAll(a => string.Compare(a.TargetLanguage, sourceLanguage,
																 StringComparison.CurrentCultureIgnoreCase) == 0);
			var project = _transcreateProjects.FirstOrDefault(a => a.Id == taskContext.Project.Id);
			if (project == null)
			{
				_transcreateProjects.Add(taskContext.Project);

				foreach (var wcProjectFile in taskContext.ProjectFiles)
				{
					ConvertToRelativePaths(taskContext.Project, wcProjectFile);
				}

				project = taskContext.Project;
			}
			else
			{
				foreach (var wcProjectFile in taskContext.ProjectFiles)
				{
					var projectFile = project.ProjectFiles.FirstOrDefault(a => a.FileId == wcProjectFile.FileId);
					if (projectFile == null)
					{
						wcProjectFile.Project = project;
						ConvertToRelativePaths(project, wcProjectFile);
						project.ProjectFiles.Add(wcProjectFile);
					}
					else if (wcProjectFile.Selected)
					{
						foreach (var fileActivity in wcProjectFile.ProjectFileActivities)
						{
							fileActivity.ProjectFile = projectFile;
						}

						ConvertToRelativePaths(project, wcProjectFile);

						projectFile.ExternalFilePath = wcProjectFile.ExternalFilePath;
						projectFile.Location = wcProjectFile.Location;
						projectFile.Report = wcProjectFile.Report;
						projectFile.Status = wcProjectFile.Status;
						projectFile.Action = wcProjectFile.Action;
						projectFile.WorkFlow = wcProjectFile.WorkFlow;
						projectFile.Date = wcProjectFile.Date;
						projectFile.ConfirmationStatistics = wcProjectFile.ConfirmationStatistics;
						projectFile.TranslationOriginStatistics = wcProjectFile.TranslationOriginStatistics;
						projectFile.ProjectFileActivities = wcProjectFile.ProjectFileActivities;
					}
				}
			}

			var reports = CreateHtmlReports(taskContext, taskContext.FileBasedProject, project);
			if (reports.Count > 0)
			{
				ReportsController.AddReports(reports);
			}

			UpdateProjectSettingsBundle(project);
			if (_projectsNavigationViewModel != null)
			{
				_projectsNavigationViewModel.Projects = _transcreateProjects;
			}
		}

		public void UpdateBackTranslationProjectData(string parentProjectId, TaskContext taskContext)
		{
			if (taskContext == null || !taskContext.Completed)
			{
				return;
			}

			_projectsController.RefreshProjects();

			var parentProject = _transcreateProjects.FirstOrDefault(a => a.Id == parentProjectId);
			if (parentProject == null)
			{
				return;
			}

			var fileBasedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == taskContext.Project.Id);
			if (fileBasedProject == null)
			{
				return;
			}


			lock (_lockObject)
			{
				_projectAutomationService.RemoveLastReportOfType("Translate");
			}

			var sourceLanguage = taskContext.Project.SourceLanguage.CultureInfo.Name;
			taskContext.Project.ProjectFiles.RemoveAll(a => string.Compare(a.TargetLanguage, sourceLanguage,
				StringComparison.CurrentCultureIgnoreCase) == 0);

			var backTranslationProject = parentProject.BackTranslationProjects.FirstOrDefault(a => a.Id == taskContext.Project.Id) ??
										 parentProject.BackTranslationProjects.FirstOrDefault(a =>
											string.Compare(a.SourceLanguage?.CultureInfo.Name,
												taskContext.Project.SourceLanguage?.CultureInfo.Name,
												StringComparison.CurrentCultureIgnoreCase) == 0 &&
											string.Compare(a.TargetLanguages.FirstOrDefault()?.CultureInfo.Name,
												taskContext.Project.TargetLanguages.FirstOrDefault()?.CultureInfo.Name,
												StringComparison.CurrentCultureIgnoreCase) == 0);

			if (backTranslationProject != null)
			{
				((BackTranslationProject)taskContext.Project).IsUpdate = true;
				parentProject.BackTranslationProjects.Remove(backTranslationProject);
			}

			foreach (var wcProjectFile in taskContext.ProjectFiles)
			{
				ConvertToRelativePaths(taskContext.Project, wcProjectFile);
			}

			backTranslationProject = taskContext.Project as BackTranslationProject;
			if (backTranslationProject != null)
			{
				parentProject.BackTranslationProjects.Add(backTranslationProject);
			}

			var reports = CreateHtmlReports(taskContext, taskContext.FileBasedProject, backTranslationProject);
			if (reports.Count > 0)
			{
				ReportsController.AddReports(reports);
			}

			UpdateBackTranslationProjectSettingsBundle(parentProject);
			if (_projectsNavigationViewModel != null)
			{
				_projectsNavigationViewModel.Projects = _transcreateProjects;
			}
		}

		private List<Report> CreateHtmlReports(TaskContext taskContext, FileBasedProject studioParentProject, IProject project)
		{
			var reports = new List<Report>();
			var reportTemplate = GetReportTemplatePath("TranscreateReport.xsl");
			var projectInfo = studioParentProject.GetProjectInfo();

			var languageDirections = GetLanguageDirectionFiles(studioParentProject.FilePath, taskContext);

			var isUpdate = taskContext.Project is BackTranslationProject backTranslationProject && backTranslationProject.IsUpdate;
			var actionName = isUpdate ? "UpdateBackTranslation" : taskContext.Action.ToString();

			foreach (var languageDirection in languageDirections)
			{
				var reportName = string.Format("{0}_{1}_{2}_{3}.xml",
					actionName,
					taskContext.DateTimeStampToString,
					languageDirection.Key.SourceLanguageCode,
					languageDirection.Key.TargetLanguageCode);

				var projectsReportsFolder = Path.Combine(projectInfo.LocalProjectFolder, "Reports");
				if (!Directory.Exists(projectsReportsFolder))
				{
					Directory.CreateDirectory(projectsReportsFolder);
				}

				var reportFile = Path.Combine(taskContext.WorkingFolder, reportName);

				var projectReportsFilePath = Path.Combine(projectsReportsFolder, reportName);

				_reportService.CreateTaskReport(taskContext, reportFile, studioParentProject, languageDirection.Key.TargetLanguageCode);

				// Copy to project reports folder
				File.Copy(reportFile, projectReportsFilePath, true);

				var htmlReportFilePath = CreateHtmlReportFile(reportFile, reportTemplate);

				var report = new Report
				{
					Language = languageDirection.Key.TargetLanguageCode,
					Path = reportFile,
					XsltPath = reportTemplate,
					Date = DateTime.Now
				};
				AssingReportProperties(taskContext.Action, report, isUpdate);

				reports.Add(report);

				foreach (var projectFile in languageDirection.Value)
				{
					if (!projectFile.Selected || string.Compare(projectFile.TargetLanguage, report.Language,
						StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						continue;
					}

					UpdateTaskContextFiles(taskContext.ProjectFiles, taskContext.LocalProjectFolder, projectFile.FileId, htmlReportFilePath);
					UpdateTaskContextFiles(project.ProjectFiles, taskContext.LocalProjectFolder, projectFile.FileId, htmlReportFilePath);
				}
			}

			return reports;
		}

		private IProject GetBackTranslationProjectProject(string projectId, out IProject parentProject)
		{
			parentProject = null;
			var projects = GetProjects();
			foreach (var project in projects)
			{
				var backTranslationProject =
					project.BackTranslationProjects.FirstOrDefault(a => a.Id == projectId);
				if (backTranslationProject != null)
				{
					parentProject = project;
					return backTranslationProject;
				}
			}

			return null;
		}

		private List<SDLTranscreateProjectFile> GetSDLTranscreateProjectFiles(IProject project)
		{
			var projectFiles = new List<SDLTranscreateProjectFile>();
			foreach (var projectFile in project.ProjectFiles)
			{
				var fileActivities = new List<SDLTranscreateProjectFileActivity>();

				foreach (var fileActivity in projectFile.ProjectFileActivities)
				{
					var settingFileActivity = new SDLTranscreateProjectFileActivity
					{
						ProjectFileId = projectFile.FileId,
						ActivityId = fileActivity.ActivityId,
						Status = fileActivity.Status.ToString(),
						Action = fileActivity.Action.ToString(),
						WorkFlow = fileActivity.WorkFlow.ToString(),
						Path = GetRelativePath(project.Path, fileActivity.Path),
						Name = fileActivity.Name,
						Date = GetDateToString(fileActivity.Date),
						Report = GetRelativePath(project.Path, fileActivity.Report),
						ConfirmationStatistics = fileActivity.ConfirmationStatistics,
						TranslationOriginStatistics = fileActivity.TranslationOriginStatistics
					};

					fileActivities.Add(settingFileActivity);
				}

				var settingProjectFile = new SDLTranscreateProjectFile
				{
					Activities = fileActivities,
					Path = GetRelativePath(project.Path, projectFile.Path),
					Action = projectFile.Action.ToString(),
					WorkFlow = projectFile.WorkFlow.ToString(),
					Status = projectFile.Status.ToString(),
					FileId = projectFile.FileId,
					Name = projectFile.Name,
					Location = GetRelativePath(project.Path, projectFile.Location),
					Date = GetDateToString(projectFile.Date),
					FileType = projectFile.FileType,
					ExternalFilePath = GetRelativePath(project.Path, projectFile.ExternalFilePath),
					TargetLanguage = projectFile.TargetLanguage,
					Report = GetRelativePath(project.Path, projectFile.Report),
					ShortMessage = projectFile.ShortMessage,
					ConfirmationStatistics = projectFile.ConfirmationStatistics,
					TranslationOriginStatistics = projectFile.TranslationOriginStatistics
				};

				projectFiles.Add(settingProjectFile);
			}

			return projectFiles;
		}

		private string GetReportTemplatePath(string name)
		{
			var filePath = Path.Combine(_pathInfo.SettingsFolderPath, name);
			var resourceName = "Trados.Transcreate.Resources." + name;

			WriteResourceToFile(resourceName, filePath);

			return filePath;
		}

		private void WriteResourceToFile(string resourceName, string fullFilePath)
		{
			if (File.Exists(fullFilePath))
			{
				File.Delete(fullFilePath);
			}

			using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			{
				using (var file = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
				{
					resource?.CopyTo(file);
				}
			}
		}

		private void UpdateProjectSettingsBundle(IProject project)
		{
			if (project == null)
			{
				return;
			}

			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);

			if (selectedProject != null)
			{
				var settingsBundle = selectedProject.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();

				var projectFiles = GetSDLTranscreateProjectFiles(project);
				xliffManagerProject.ProjectFilesJson.Value = JsonConvert.SerializeObject(projectFiles);

				selectedProject.UpdateSettings(xliffManagerProject.SettingsBundle);
				selectedProject.Save();
			}
		}

		private void UpdateBackTranslationProjectSettingsBundle(IProject project)
		{
			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);

			if (selectedProject != null)
			{
				var settingsBundle = selectedProject.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<SDLTranscreateBackProjects>();

				var backProjects = new List<SDLTranscreateBackProject>();

				foreach (var backTranslationProject in project.BackTranslationProjects)
				{
					var backProject = new SDLTranscreateBackProject
					{
						Id = backTranslationProject.Id,
						Name = backTranslationProject.Name,
						SourceLanguage = backTranslationProject.SourceLanguage.CultureInfo.Name,
						TargetLanguages = backTranslationProject.TargetLanguages.Select(a => a.CultureInfo.Name).ToList(),
						Customer = backTranslationProject.Customer,
						Path = GetRelativePath(project.Path, backTranslationProject.Path),
						ProjectType = backTranslationProject.ProjectType,
						Created = GetDateToString(backTranslationProject.Created),
						DueDate = GetDateToString(backTranslationProject.DueDate),
						ProjectFiles = GetSDLTranscreateProjectFiles(backTranslationProject)
					};

					backProjects.Add(backProject);
				}

				xliffManagerProject.BackProjectsJson.Value = JsonConvert.SerializeObject(backProjects);

				selectedProject.UpdateSettings(xliffManagerProject.SettingsBundle);
				selectedProject.Save();
			}
		}

		private void LoadProjects()
		{
			_transcreateProjects = new List<IProject>();

			foreach (var project in _projectsController.GetAllProjects())
			{
				AddNewProjectToContainer(project);
			}
		}

		private bool AddNewProjectToContainer(FileBasedProject project)
		{
			if (project == null)
			{
				return false;
			}

			var projectInfo = GetNormalizedProjectOrigin(project);
			try
			{
				if (_transcreateProjects.FirstOrDefault(a => a.Id == projectInfo.Id.ToString()) != null)
				{
					// already present in the list
					return false;
				}

				if (IsBackTranslationProject(projectInfo.ProjectOrigin))
				{
					return false;
				}

				var settingsBundle = project.GetSettings();
				var sdlTranscreateProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();

				var sdlProjectFiles = DeserializeProjectFiles(sdlTranscreateProject.ProjectFilesJson.Value);
				if (sdlProjectFiles?.Count <= 0)
				{
					return false;
				}

				var xliffProject = new Project
				{
					Id = projectInfo.Id.ToString(),
					Name = projectInfo.Name,
					Customer = _customerProvider.GetProjectCustomer(project),
					Created = projectInfo.CreatedAt.ToUniversalTime(),
					DueDate = projectInfo.DueDate?.ToUniversalTime() ?? DateTime.MaxValue,
					Path = projectInfo.LocalProjectFolder,
					SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo),
					TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages),
					ProjectType = GetProjectType(project)
				};

				var projectFiles = GetProjectFiles(sdlProjectFiles, xliffProject);
				if (projectFiles?.Count > 0)
				{
					var sdlBackTranslationProjects = settingsBundle.GetSettingsGroup<SDLTranscreateBackProjects>();
					var sdlBackTranslationProject = SerializeBackProjects(sdlBackTranslationProjects.BackProjectsJson.Value);
					var backProjects = GetBackProjects(projectInfo.LocalProjectFolder, sdlBackTranslationProject);

					xliffProject.ProjectFiles.AddRange(projectFiles);

					if (backProjects?.Count > 0)
					{
						xliffProject.BackTranslationProjects.AddRange(backProjects);
					}

					_transcreateProjects.Add(xliffProject);

					return true;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(PluginResources.Warning_UnableToLoadProject, projectInfo.Name)
								+ Environment.NewLine + Environment.NewLine + ex.Message,
					PluginResources.Plugin_Name, MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}

			return false;
		}

		private ProjectInfo GetNormalizedProjectOrigin(FileBasedProject project)
		{
			var projectInfo = project.GetProjectInfo();
			if (projectInfo.ProjectOrigin == "Transcreate Project")
			{
				// required to force studio to recognize a 'case' difference.
				projectInfo.ProjectOrigin = string.Empty;
				project.UpdateProject(projectInfo);

				projectInfo.ProjectOrigin = Constants.ProjectOrigin_TranscreateProject;
				project.UpdateProject(projectInfo);
				project.Save();
			}

			return projectInfo;
		}

		private List<ProjectFile> GetProjectFiles(IReadOnlyCollection<SDLTranscreateProjectFile> sdlProjectFiles, IProject project)
		{
			var projectFiles = new List<ProjectFile>();
			if (sdlProjectFiles?.Count > 0)
			{
				foreach (var sdlProjectFile in sdlProjectFiles)
				{
					var projectFileActivities = new List<ProjectFileActivity>();

					var projectFile = new ProjectFile
					{
						Path = sdlProjectFile.Path,
						Action = GetAction(sdlProjectFile.Action),
						WorkFlow = GetWorkFlow(sdlProjectFile.WorkFlow),
						Status = GetStatus(sdlProjectFile.Status),
						Date = GetDateTime(sdlProjectFile.Date),
						Report = GetRelativePath(project.Path, sdlProjectFile.Report),
						FileId = sdlProjectFile.FileId,
						FileType = sdlProjectFile.FileType,
						Location = GetRelativePath(project.Path, sdlProjectFile.Location),
						Name = sdlProjectFile.Name,
						ProjectId = project.Id,
						ProjectFileActivities = projectFileActivities,
						ExternalFilePath = GetRelativePath(project.Path, sdlProjectFile.ExternalFilePath),
						Project = project,
						TargetLanguage = sdlProjectFile.TargetLanguage,
						ConfirmationStatistics = sdlProjectFile.ConfirmationStatistics,
						TranslationOriginStatistics = sdlProjectFile.TranslationOriginStatistics
					};

					foreach (var fileActivity in sdlProjectFile.Activities)
					{
						var projectFileActivity = new ProjectFileActivity
						{
							ProjectFile = projectFile,
							Path = GetRelativePath(project.Path, fileActivity.Path),
							Action = GetAction(fileActivity.Action),
							WorkFlow = GetWorkFlow(fileActivity.WorkFlow),
							Status = GetStatus(fileActivity.Status),
							ActivityId = fileActivity.ActivityId,
							Date = GetDateTime(fileActivity.Date),
							Report = GetRelativePath(project.Path, fileActivity.Report),
							Name = fileActivity.Name,
							ProjectFileId = fileActivity.ProjectFileId,
							ConfirmationStatistics = fileActivity.ConfirmationStatistics,
							TranslationOriginStatistics = fileActivity.TranslationOriginStatistics
						};

						projectFileActivities.Add(projectFileActivity);
					}

					projectFiles.Add(projectFile);
				}
			}

			return projectFiles;
		}

		private List<BackTranslationProject> GetBackProjects(string parentProjectPath, IReadOnlyCollection<SDLTranscreateBackProject> backTranslationProjects)
		{
			var backProjects = new List<BackTranslationProject>();
			if (backTranslationProjects?.Count > 0)
			{
				foreach (var backTranslationProject in backTranslationProjects)
				{
					var backProject = new BackTranslationProject
					{
						Id = backTranslationProject.Id,
						Name = backTranslationProject.Name,
						SourceLanguage = GetLanguageInfo(backTranslationProject.SourceLanguage),
						TargetLanguages = GetLanguageInfos(backTranslationProject.TargetLanguages),
						Customer = backTranslationProject.Customer,
						Path = Path.Combine(parentProjectPath, backTranslationProject.Path),
						ProjectType = backTranslationProject.ProjectType,
						Created = GetDateTime(backTranslationProject.Created),
						DueDate = GetDateTime(backTranslationProject.DueDate),

					};

					backProject.ProjectFiles = GetProjectFiles(backTranslationProject.ProjectFiles, backProject);
					backProjects.Add(backProject);
				}
			}

			return backProjects;
		}

		private bool UpdateCustomerInfo(FileBasedProject project, IProject xliffProject)
		{
			var customer = _customerProvider.GetProjectCustomer(project);
			var customerName = customer?.Name ?? string.Empty;
			var customerEmail = customer?.Email ?? string.Empty;

			var xliffCustomerName = xliffProject.Customer?.Name?.Replace("[no client]", string.Empty) ?? string.Empty;
			var xliffCustomerEmail = xliffProject.Customer?.Email ?? string.Empty;

			if (customerName != xliffCustomerName || customerEmail != xliffCustomerEmail)
			{
				if (customer == null)
				{
					xliffProject.Customer = null;
				}
				else
				{
					xliffProject.Customer = new Customer
					{
						Id = customer.Id,
						Name = customer.Name,
						Email = customer.Email
					};
				}
				return true;
			}

			return false;
		}

		private void ConvertToRelativePaths(IProject project, ProjectFile wcProjectFile)
		{
			foreach (var fileActivity in wcProjectFile.ProjectFileActivities)
			{
				fileActivity.Path = GetRelativePath(project.Path, fileActivity.Path);
				fileActivity.Report = GetRelativePath(project.Path, fileActivity.Report);
			}

			wcProjectFile.ExternalFilePath = GetRelativePath(project.Path, wcProjectFile.ExternalFilePath);
			wcProjectFile.Location = GetRelativePath(project.Path, wcProjectFile.Location);
			wcProjectFile.Report = GetRelativePath(project.Path, wcProjectFile.Report);
		}

		private void AssingReportProperties(Enumerators.Action action, Report report, bool isUpdate)
		{
			switch (action)
			{
				case Enumerators.Action.Convert:
					report.Name = PluginResources.Report_Label_CreateTranscreateProject;
					report.Group = PluginResources.Report_Label_ProjectCreation;
					report.Description = PluginResources.Report_Label_CreatedTranscreateProject;
					break;
				case Enumerators.Action.CreateBackTranslation:
					if (isUpdate)
					{
						report.Name = PluginResources.Report_Label_UpdateBackTranslationProject;
						report.Group = PluginResources.Report_Label_ProjectCreation;
						report.Description = PluginResources.Report_Label_UpdatedBackTranslationProject;
					}
					else
					{
						report.Name = PluginResources.Report_Label_CreateBackTranslationProject;
						report.Group = PluginResources.Report_Label_ProjectCreation;
						report.Description = PluginResources.Report_Label_CreatedBackTranslationProject;
					}
					break;
				case Enumerators.Action.Export:
					report.Name = PluginResources.Report_Label_ExportTranslations;
					report.Group = PluginResources.Report_Label_Export;
					report.Description = PluginResources.Report_Label_ExportedForTranslation;
					break;
				case Enumerators.Action.Import:
					report.Name = PluginResources.Report_Label_ImportTranslations;
					report.Group = PluginResources.Report_Label_Import;
					report.Description = PluginResources.Report_Label_ImportedTranslations;
					break;
				case Enumerators.Action.ExportBackTranslation:
					report.Name = PluginResources.Report_Label_ExportBackTranslations;
					report.Group = PluginResources.Report_Label_Export;
					report.Description = PluginResources.Report_Label_ExportedForBackTranslation;
					break;
				case Enumerators.Action.ImportBackTranslation:
					report.Name = PluginResources.Report_Label_ImportBackTranslations;
					report.Group = PluginResources.Report_Label_Import;
					report.Description = PluginResources.Report_Label_ImportedBackTranslation;
					break;
			}
		}

		private void UpdateTaskContextFiles(IEnumerable<ProjectFile> projectFiles, string localProjectFolder, string fileId, string htmlReportFilePath)
		{
			var projectFile = projectFiles.FirstOrDefault(a => a.FileId == fileId);
			if (projectFile != null)
			{
				projectFile.Report = GetRelativePath(localProjectFolder, htmlReportFilePath);

				var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault();
				if (activityfile != null)
				{
					activityfile.Report = projectFile.Report;
				}
			}
		}

		private string CreateHtmlReportFile(string xmlReportFullPath, string xsltFilePath)
		{
			var htmlReportFilePath = xmlReportFullPath + ".html";

			var xsltSetting = new XsltSettings
			{
				EnableDocumentFunction = true,
				EnableScript = true
			};

			var myXPathDoc = new XPathDocument(xmlReportFullPath);

			var myXslTrans = new XslCompiledTransform();
			myXslTrans.Load(xsltFilePath, xsltSetting, null);

			var myWriter = new XmlTextWriter(htmlReportFilePath, Encoding.UTF8);

			myXslTrans.Transform(myXPathDoc, null, myWriter);

			myWriter.Flush();
			myWriter.Close();

			return htmlReportFilePath;
		}

		private Dictionary<LanguageDirectionInfo, List<ProjectFile>> GetLanguageDirectionFiles(string projectsFile, TaskContext taskContext)
		{
			var languageDirections = new Dictionary<LanguageDirectionInfo, List<ProjectFile>>();
			foreach (var language in _projectSettingsService.GetLanguageDirections(projectsFile))
			{
				foreach (var projectFile in taskContext.ProjectFiles)
				{
					if (!projectFile.Selected)
					{
						continue;
					}

					if (string.Compare(projectFile.TargetLanguage, language.TargetLanguageCode,
							StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						continue;
					}

					if (languageDirections.ContainsKey(language))
					{
						languageDirections[language].Add(projectFile);
					}
					else
					{
						languageDirections.Add(language, new List<ProjectFile> { projectFile });
					}
				}
			}

			return languageDirections;
		}

		private List<SDLTranscreateBackProject> SerializeBackProjects(string value)
		{
			try
			{
				var projects = JsonConvert.DeserializeObject<List<SDLTranscreateBackProject>>(value);
				return projects;
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private List<SDLTranscreateProjectFile> DeserializeProjectFiles(string value)
		{
			try
			{
				var projectFiles = JsonConvert.DeserializeObject<List<SDLTranscreateProjectFile>>(value);
				return projectFiles;
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private bool AddNewProjectFiles(FileBasedProject fileBasedProject, IProject project)
		{
			var addedNewFiles = false;
			var projectInfo = fileBasedProject.GetProjectInfo();

			foreach (var targetLanguage in projectInfo.TargetLanguages)
			{
				var projectFiles = fileBasedProject.GetTargetLanguageFiles(targetLanguage);
				foreach (var projectFile in projectFiles)
				{
					if (projectFile.Role != FileRole.Translatable)
					{
						continue;
					}

					var xliffFile = project.ProjectFiles.FirstOrDefault(a => a.FileId == projectFile.Id.ToString());
					if (xliffFile == null)
					{
						addedNewFiles = true;
						var file = new ProjectFile
						{
							ProjectId = projectInfo.Id.ToString(),
							FileId = projectFile.Id.ToString(),
							Name = projectFile.Name,
							Path = GetRelativePath(projectInfo.LocalProjectFolder, projectFile.Folder),
							Location = GetRelativePath(projectInfo.LocalProjectFolder, projectFile.LocalFilePath),
							Action = Enumerators.Action.None,
							WorkFlow = Enumerators.WorkFlow.None,
							Status = Enumerators.Status.Ready,
							Date = DateTime.MinValue,
							TargetLanguage = targetLanguage.CultureInfo.Name,
							Selected = false,
							FileType = projectFile.FileTypeId,
							Project = project
						};

						project.ProjectFiles.Add(file);
					}
				}
			}

			return addedNewFiles;
		}

		private string GetRelativePath(string projectPath, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			return path.Replace(projectPath.Trim('\\') + '\\', string.Empty);
		}

		private bool IsBackTranslationProject(string projectOrigin)
		{
			return string.Compare(projectOrigin, Constants.ProjectOrigin_BackTranslationProject,
				StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		private void EditorController_Opened(object sender, DocumentEventArgs e)
		{
			lock (_lockObject)
			{
				if (e.Document.Files?.Count() == 0 || e.Document?.Project == null)
				{
					return;
				}

				var project = e.Document.Project;
				var projectInfo = project.GetProjectInfo();
				var projectId = projectInfo.Id.ToString();

				IProject parentProject = null;
				var transcreateProject = IsBackTranslationProject(projectInfo.ProjectOrigin)
					? GetBackTranslationProjectProject(projectInfo.Id.ToString(), out parentProject)
					: GetProjects().FirstOrDefault(a => a.Id == projectId);

				foreach (var document in e.Document.Files)
				{
					var documentId = document?.Id.ToString();
					var language = document?.Language;

					var projectFile = transcreateProject?.ProjectFiles.FirstOrDefault(a => a.FileId == documentId &&
						a.TargetLanguage == language.CultureInfo.Name);

					if (projectFile != null)
					{
						if (!OverrideEditorWarningMessage)
						{
							if (projectFile.Action == Enumerators.Action.Export ||
								projectFile.Action == Enumerators.Action.ExportBackTranslation)
							{
								var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date)
									.FirstOrDefault(a => a.Action == Enumerators.Action.Export);

								var message1 = string.Format(PluginResources.Message_FileWasExportedOn,
									activityfile?.DateToString);
								var message2 =
									string.Format(PluginResources.Message_WarningTranslationsCanBeOverwrittenDuringImport,
										activityfile?.DateToString);

								MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2,
									PluginResources.TranscreateManager_Name, MessageBoxButtons.OK,
									MessageBoxIcon.Warning);
							}
						}

						OverrideEditorWarningMessage = false;

						var segmentBuilder = new SegmentBuilder();

						var action = transcreateProject is BackTranslationProject
							? Enumerators.Action.ExportBackTranslation
							: Enumerators.Action.Export;
						var workFlow = Enumerators.WorkFlow.Internal;
						var setttings = GetSettings();

						var taskContext = new TaskContext(action, workFlow, setttings);
						taskContext.AnalysisBands = _projectAutomationService.GetAnalysisBands(project);
						taskContext.ExportOptions.IncludeBackTranslations = true;
						taskContext.ExportOptions.IncludeTranslations = true;
						taskContext.ExportOptions.CopySourceToTarget = false;

						taskContext.LocalProjectFolder = projectInfo.LocalProjectFolder;
						taskContext.WorkflowFolder = taskContext.GetWorkflowPath();
						var workingProject =
							_projectAutomationService.GetProject(project, new List<string> { projectFile.FileId });

						taskContext.Project = workingProject;
						taskContext.FileBasedProject = project;
						taskContext.ProjectFiles = workingProject.ProjectFiles;

						var targetFile = taskContext.ProjectFiles.FirstOrDefault(a => a.FileId == projectFile.FileId);

						var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);
						var sdlxliffReader = new SdlxliffReader(segmentBuilder, taskContext.ExportOptions,
							taskContext.AnalysisBands);

						var languageFolder = GetLanguageFolder(taskContext, targetFile.TargetLanguage);

						var xliffFolder = GetXliffFolder(languageFolder, targetFile);
						var xliffFilePath = Path.Combine(xliffFolder, targetFile.Name + ".xliff");

						var inputPath = Path.Combine(taskContext.LocalProjectFolder, targetFile.Location);
						var data = sdlxliffReader.ReadFile(transcreateProject.Id, targetFile.FileId, inputPath,
							targetFile.TargetLanguage);
						var exported = xliffWriter.WriteFile(data, xliffFilePath, true);

						if (exported)
						{
							var tmpInputPath = inputPath + ".tmp.sdlxliff";
							if (File.Exists(tmpInputPath))
							{
								File.Delete(tmpInputPath);
							}

							File.Copy(inputPath, tmpInputPath);

							targetFile.XliffData = data;
							targetFile.Date = taskContext.DateTimeStamp;
							targetFile.Action = taskContext.Action;
							targetFile.WorkFlow = taskContext.WorkFlow;
							targetFile.Status = Enumerators.Status.Success;
							targetFile.ExternalFilePath = xliffFilePath;
							targetFile.ConfirmationStatistics = sdlxliffReader.ConfirmationStatistics;
							targetFile.TranslationOriginStatistics = sdlxliffReader.TranslationOriginStatistics;
							targetFile.Report = string.Empty;
						}

						var activityFile = new ProjectFileActivity
						{
							ProjectFileId = targetFile.FileId,
							ActivityId = Guid.NewGuid().ToString(),
							Action = taskContext.Action,
							WorkFlow = taskContext.WorkFlow,
							Status = exported ? Enumerators.Status.Success : Enumerators.Status.Error,
							Date = taskContext.DateTimeStamp,
							Name = Path.GetFileName(xliffFilePath),
							Path = Path.GetDirectoryName(xliffFilePath),
							Report = string.Empty,
							ProjectFile = targetFile,
							ConfirmationStatistics = sdlxliffReader.ConfirmationStatistics,
							TranslationOriginStatistics = sdlxliffReader.TranslationOriginStatistics
						};

						targetFile.ProjectFileActivities.Add(activityFile);
						taskContext.Completed = true;

						_projectAutomationService.ActivateProject(project);

						if (transcreateProject is BackTranslationProject)
						{
							UpdateBackTranslationProjectData(parentProject?.Id, taskContext);
						}
						else
						{
							UpdateProjectData(taskContext);
						}
					}
				}
			}
		}

		private void EditorController_Closed(object sender, DocumentEventArgs e)
		{
			lock (_lockObject)
			{
				if (e.Document.Files?.Count() == 0 || e.Document?.Project == null)
				{
					return;
				}

				var project = e.Document.Project;
				var projectInfo = project.GetProjectInfo();
				var projectId = projectInfo.Id.ToString();

				IProject parentProject = null;
				var transcreateProject = IsBackTranslationProject(projectInfo.ProjectOrigin)
					? GetBackTranslationProjectProject(projectInfo.Id.ToString(), out parentProject)
					: GetProjects().FirstOrDefault(a => a.Id == projectId);

				foreach (var document in e.Document.Files)
				{
					var documentId = document?.Id.ToString();
					var language = document?.Language;

					var projectFile = transcreateProject?.ProjectFiles.FirstOrDefault(a => a.FileId == documentId &&
						a.TargetLanguage == language.CultureInfo.Name);
					if (projectFile != null)
					{
						var segmentBuilder = new SegmentBuilder();

						var action = transcreateProject is BackTranslationProject
							? Enumerators.Action.ImportBackTranslation
							: Enumerators.Action.Import;
						var workFlow = Enumerators.WorkFlow.Internal;
						var setttings = GetSettings();

						var taskContext = new TaskContext(action, workFlow, setttings);
						taskContext.AnalysisBands = _projectAutomationService.GetAnalysisBands(project);
						taskContext.ExportOptions.IncludeBackTranslations = true;
						taskContext.ExportOptions.IncludeTranslations = true;
						taskContext.ExportOptions.CopySourceToTarget = false;
						taskContext.ImportOptions.StatusTranslationUpdatedId = string.Empty;
						taskContext.ImportOptions.StatusSegmentNotImportedId = string.Empty;
						taskContext.ImportOptions.StatusTranslationNotUpdatedId = string.Empty;
						taskContext.ImportOptions.OverwriteTranslations = true;
						taskContext.ImportOptions.OriginSystem = Constants.OriginSystem_TranscreateAutomation;

						taskContext.LocalProjectFolder = projectInfo.LocalProjectFolder;
						taskContext.WorkflowFolder = taskContext.GetWorkflowPath();
						var workingProject =
							_projectAutomationService.GetProject(project, new List<string> { projectFile.FileId });

						taskContext.Project = workingProject;
						taskContext.FileBasedProject = project;
						taskContext.ProjectFiles = workingProject.ProjectFiles;

						var targetFile = taskContext.ProjectFiles.FirstOrDefault(a => a.FileId == projectFile.FileId);

						var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);
						var sdlxliffReader = new SdlxliffReader(segmentBuilder, taskContext.ExportOptions,
							taskContext.AnalysisBands);
						var sdlxliffWriter = new SdlxliffWriter(segmentBuilder, taskContext.ImportOptions,
							taskContext.AnalysisBands);

						var languageFolder = GetLanguageFolder(taskContext, targetFile.TargetLanguage);

						var xliffFolder = GetXliffFolder(languageFolder, targetFile);
						var xliffFilePath = Path.Combine(xliffFolder, targetFile.Name + ".xliff");

						var inputPath = Path.Combine(taskContext.LocalProjectFolder, targetFile.Location);
						var data = sdlxliffReader.ReadFile(transcreateProject.Id, targetFile.FileId, inputPath,
							targetFile.TargetLanguage);

						var imported = false;
						var exported = xliffWriter.WriteFile(data, xliffFilePath, true);
						if (exported)
						{
							var tempFileName = Path.GetTempFileName();
							File.Move(tempFileName, tempFileName + ".sdlxliff");
							tempFileName = tempFileName + ".sdlxliff";

							var tmpInputFile = inputPath + ".tmp.sdlxliff";

							if (File.Exists(tmpInputFile))
							{
								imported = sdlxliffWriter.UpdateFile(data, tmpInputFile, tempFileName);

								if (File.Exists(tempFileName))
								{
									File.Delete(tempFileName);
								}

								if (File.Exists(tmpInputFile))
								{
									File.Delete(tmpInputFile);
								}

								if (imported)
								{
									targetFile.XliffData = data;
									targetFile.Date = taskContext.DateTimeStamp;
									targetFile.Action = taskContext.Action;
									targetFile.WorkFlow = taskContext.WorkFlow;
									targetFile.Status = Enumerators.Status.Success;
									targetFile.ExternalFilePath = xliffFilePath;
									targetFile.ConfirmationStatistics = sdlxliffWriter.ConfirmationStatistics;
									targetFile.TranslationOriginStatistics = sdlxliffWriter.TranslationOriginStatistics;
									targetFile.Report = string.Empty;
								}
							}
						}

						foreach (var projectFileActivity in targetFile.ProjectFileActivities)
						{
							projectFileActivity.ProjectFile = targetFile;
						}

						// monitor internal changes only when the document is updated in the editor.
						var projectTargetFile = transcreateProject.ProjectFiles.FirstOrDefault(a => a.FileId == targetFile.FileId);
						if (projectTargetFile == null)
						{
							continue;
						}

						var projectTargetFileActivity = projectTargetFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault();
						if (projectTargetFileActivity != null && (projectTargetFileActivity.Action == Enumerators.Action.Export ||
							projectTargetFileActivity.Action == Enumerators.Action.ExportBackTranslation))
						{

							if (sdlxliffWriter.ConfirmationStatistics?.WordCounts?.Processed?.Count <= 0)
							{
								RemovePreviousProjectFileActivity(projectTargetFile, projectTargetFileActivity);
								UpdateProjectSettingsBundle(transcreateProject is BackTranslationProject ? parentProject : transcreateProject);
								if (_projectsNavigationViewModel != null)
								{
									_projectsNavigationViewModel.Projects = _transcreateProjects;
								}
								continue;
							}

							var activityFile = new ProjectFileActivity
							{
								ProjectFileId = targetFile.FileId,
								ActivityId = Guid.NewGuid().ToString(),
								Action = taskContext.Action,
								WorkFlow = taskContext.WorkFlow,
								Status = imported ? Enumerators.Status.Success : Enumerators.Status.Error,
								Date = taskContext.DateTimeStamp,
								Name = Path.GetFileName(xliffFilePath),
								Path = Path.GetDirectoryName(xliffFilePath),
								Report = string.Empty,
								ProjectFile = targetFile,
								ConfirmationStatistics = sdlxliffWriter.ConfirmationStatistics,
								TranslationOriginStatistics = sdlxliffWriter.TranslationOriginStatistics
							};

							targetFile.ProjectFileActivities.Add(activityFile);
							taskContext.Completed = true;

							_projectAutomationService.ActivateProject(project);
							if (transcreateProject is BackTranslationProject)
							{
								UpdateBackTranslationProjectData(parentProject?.Id, taskContext);
							}
							else
							{
								UpdateProjectData(taskContext);
							}
						}
					}
				}
			}
		}

		private void RemovePreviousProjectFileActivity(ProjectFile projectTargetFile,
			ProjectFileActivity projectFileActivity)
		{
			if (projectFileActivity == null)
			{
				return;
			}

			projectTargetFile.ProjectFileActivities.Remove(projectFileActivity);

			var previousActivity = projectTargetFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault();
			if (previousActivity != null)
			{
				projectTargetFile.Action = previousActivity.Action;
				projectTargetFile.ConfirmationStatistics = previousActivity.ConfirmationStatistics;
				projectTargetFile.TranslationOriginStatistics = previousActivity.TranslationOriginStatistics;
				projectTargetFile.Date = previousActivity.Date;
				projectTargetFile.ExternalFilePath = Path.Combine(previousActivity.Path, previousActivity.Name);
				projectTargetFile.Report = previousActivity.Report;
			}
		}

		private static bool DocumentIsUpdated(IReadOnlyCollection<WordCount> currentWordCounts, IReadOnlyCollection<WordCount> updatedWordCounts)
		{
			if (currentWordCounts == null && updatedWordCounts == null)
			{
				return false;
			}

			if (currentWordCounts == null || updatedWordCounts == null)
			{
				return true;
			}

			if (currentWordCounts.Count != updatedWordCounts.Count)
			{
				return true;
			}

			foreach (var currentWordCount in currentWordCounts)
			{
				var updatedWordCount = updatedWordCounts.FirstOrDefault(x => x.Category == currentWordCount.Category);
				if (updatedWordCount == null)
				{
					return true;
				}

				if (updatedWordCount.Segments != currentWordCount.Segments ||
					updatedWordCount.Characters != currentWordCount.Characters ||
					updatedWordCount.Placeables != currentWordCount.Placeables ||
					updatedWordCount.Tags != currentWordCount.Tags ||
					updatedWordCount.Words != currentWordCount.Words)
				{
					return true;
				}
			}

			return false;
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

		private string GetLanguageFolder(TaskContext context, string name)
		{
			var languageFolder = context.GetLanguageFolder(name);
			if (!Directory.Exists(languageFolder))
			{
				Directory.CreateDirectory(languageFolder);
			}

			return languageFolder;
		}

		private static string GetXliffFolder(string languageFolder, ProjectFile targetFile)
		{
			var xliffFolder = Path.Combine(languageFolder, targetFile.Path.TrimStart('\\'));
			if (!Directory.Exists(xliffFolder))
			{
				Directory.CreateDirectory(xliffFolder);
			}

			return xliffFolder;
		}

		private DateTime GetDateTime(string value)
		{
			var format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
			var successDate = DateTime.TryParseExact(value, format,
				CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var resultDate);

			var date = successDate ? resultDate.ToUniversalTime() : DateTime.MinValue;
			return date;
		}

		private string GetDateToString(DateTime date)
		{
			var value = string.Empty;

			if (date != DateTime.MinValue || date != DateTime.MaxValue)
			{
				return date.ToUniversalTime()
					.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
			}

			return value;
		}

		private Enumerators.Status GetStatus(string value)
		{
			var success = Enum.TryParse<Enumerators.Status>(value, true, out var resultStatus);
			var status = success ? resultStatus : Enumerators.Status.None;
			return status;
		}

		private Enumerators.Action GetAction(string value)
		{
			var success = Enum.TryParse<Enumerators.Action>(value, true, out var resultAction);
			var action = success ? resultAction : Enumerators.Action.None;
			return action;
		}

		private Enumerators.WorkFlow GetWorkFlow(string value)
		{
			var success = Enum.TryParse<Enumerators.WorkFlow>(value, true, out var resultWorkFlow);
			var workFlow = success ? resultWorkFlow : Enumerators.WorkFlow.None;
			return workFlow;
		}

		private string GetProjectType(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				return internalDynamicProject.ProjectType.ToString();
			}

			return null;
		}

		private List<LanguageInfo> GetLanguageInfos(IEnumerable<Language> languages)
		{
			var targetLanguages = new List<LanguageInfo>();
			foreach (var targetLanguage in languages)
			{
				targetLanguages.Add(GetLanguageInfo(targetLanguage.CultureInfo));
			}

			return targetLanguages;
		}

		private LanguageInfo GetLanguageInfo(CultureInfo cultureInfo)
		{
			var languageInfo = new LanguageInfo
			{
				CultureInfo = cultureInfo,
				Image = _imageService.GetImage(cultureInfo.Name)
			};

			return languageInfo;
		}

		private LanguageInfo GetLanguageInfo(string language)
		{
			var ci = new CultureInfo(language);
			return GetLanguageInfo(ci);
		}

		private List<LanguageInfo> GetLanguageInfos(IEnumerable<string> languages)
		{
			var languageInfos = new List<LanguageInfo>();
			foreach (var language in languages)
			{
				languageInfos.Add(GetLanguageInfo(language));
			}

			return languageInfos;
		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			IsActive = e.Active;
			if (e.Active)
			{
				SetProjectFileActivityViewController();

				var currentProject = _projectsController.CurrentProject;
				var selectedProject = _projectsNavigationViewModel?.SelectedProject;

				if (selectedProject == null)
				{
					return;
				}

				if (currentProject.GetProjectInfo()?.Id.ToString() != selectedProject.Id)
				{
					var studioProject = GetStudioProject(selectedProject.Id);
					if (studioProject != null)
					{
						_projectsController.ActivateProject(studioProject);
						_projectsController.SelectedProjects = new List<FileBasedProject> { studioProject };
					}
				}
			}
		}

		private FileBasedProject GetStudioProject(string id)
		{
			foreach (var project in _projectsController.GetAllProjects())
			{
				if (project.GetProjectInfo()?.Id.ToString() == id)
				{
					return project;
				}
			}

			return null;
		}

		private void SetProjectFileActivityViewController()
		{
			if (_projectFileActivityViewController != null)
			{
				return;
			}

			try
			{
				_projectFileActivityViewController =
					SdlTradosStudio.Application.GetController<ProjectFileActivityViewController>();

				_projectFilesViewModel.ProjectFileActivityViewModel =
					new ProjectFileActivityViewModel(_projectFilesViewModel?.SelectedProjectFile?.ProjectFileActivities);

				_projectFileActivityViewController.ViewModel = _projectFilesViewModel.ProjectFileActivityViewModel;
			}
			catch
			{
				// catch all; unable to locate the controller
			}
		}

		private bool UnloadRemovedProjectsFromContainer()
		{
			var updated = false;
			var removedProjects = GetRemovedProjects();
			if (removedProjects.Count > 0)
			{
				updated = true;
				foreach (var project in removedProjects)
				{
					_transcreateProjects.Remove(project);
				}
			}

			return updated;
		}

		private List<IProject> GetRemovedProjects()
		{
			var removedProjects = new List<Interfaces.IProject>();
			var studioProjects = _projectsController.GetProjects().ToList();
			foreach (var project in _transcreateProjects)
			{
				var studioProject = studioProjects.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);
				if (studioProject == null)
				{
					removedProjects.Add(project);
				}
			}

			return removedProjects;
		}

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			ProjectSelectionChanged?.Invoke(this, e);

			if (e.SelectedProject != null && _projectAutomationService != null)
			{
				lock (_lockObject)
				{
					if (_projectsController.CurrentProject?.GetProjectInfo().Id.ToString() != e.SelectedProject.Id)
					{
						var fileBasedProject = _projectsController.GetProjects()
							.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == e.SelectedProject.Id);

						if (fileBasedProject != null)
						{
							try
							{
								lock (_lockObject)
								{
									_projectAutomationService.ActivateProject(fileBasedProject);
								}

								_projectsController.SelectedProjects = new[] { fileBasedProject };
							}
							catch
							{
								//ignore
							}
						}
					}
				}
			}
		}

		private void ProjectFilesViewModel_ProjectFileSelectionChanged(object sender, ProjectFileSelectionChangedEventArgs e)
		{
			ProjectFileSelectionChanged?.Invoke(sender, e);
		}

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			if (IgnoreProjectChanged)
			{
				return;
			}

			ReportsController = new ReportsController(_projectsController.CurrentProject, _reviewerPathInfo, _taskTemplateIdProvider);

			var updated = AddNewProjectToContainer(_projectsController?.CurrentProject);
			if (!updated)
			{
				updated = UnloadRemovedProjectsFromContainer();
			}

			var updatedProjectSelection = UpdateProjectSelectionFromProjectsController();
			if (_projectsNavigationViewModel == null)
			{
				return;
			}

			if (updated || updatedProjectSelection)
			{
				_projectsNavigationViewModel.Projects = _transcreateProjects;
				if (_projectsController?.CurrentProject == null)
				{
					return;
				}

				lock (_lockObject)
				{
					var projectInfo = GetNormalizedProjectOrigin(_projectsController?.CurrentProject);
					var iconPath = _projectAutomationService.IsBackTranslationProject(projectInfo?.ProjectOrigin)
						? _projectAutomationService.GetBackTranslationIconPath(_pathInfo)
						: _projectAutomationService.GetTranscreateIconPath(_pathInfo);

					_projectAutomationService?.UpdateProjectIcon(_projectsController?.CurrentProject, iconPath);
				}
			}
		}

		private bool UpdateProjectSelectionFromProjectsController()
		{
			if (_projectsController?.CurrentProject != null)
			{
				var selectedProject = GetSelectedProjects()?.FirstOrDefault();

				var projectInfo = _projectsController.CurrentProject.GetProjectInfo();
				var projectId = projectInfo.Id.ToString();
				IProject parentProject = null;
				var transcreateProject = IsBackTranslationProject(projectInfo.ProjectOrigin)
					? GetBackTranslationProjectProject(projectId, out parentProject)
					: GetProjects().FirstOrDefault(a => a.Id == projectId);

				if (transcreateProject != null)
				{
					if (selectedProject?.Id != transcreateProject.Id)
					{
						if (selectedProject != null)
						{
							selectedProject.IsSelected = false;
						}

						if (parentProject != null)
						{
							parentProject.IsExpanded = true;
						}

						transcreateProject.IsSelected = true;

						return true;
					}
				}
			}

			return false;
		}

		public override void Dispose()
		{
			_projectFilesViewModel?.Dispose();
			if (_projectsController != null)
			{
				_projectsController.CurrentProjectChanged -= ProjectsController_CurrentProjectChanged;
			}
			if (_editorController != null)
			{
				_editorController.Opened -= EditorController_Opened;
			}

			_projectsNavigationViewModel?.Dispose();


			base.Dispose();
		}
	}
}
