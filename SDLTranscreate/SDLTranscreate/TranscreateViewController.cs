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
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.Controls;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Writers;
using Sdl.Community.Transcreate.Interfaces;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.Community.Transcreate.Model.Tasks;
using Sdl.Community.Transcreate.Service;
using Sdl.Community.Transcreate.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.Reports.Viewer.API;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using AutomaticTask = Sdl.Community.Transcreate.Model.Tasks.AutomaticTask;
using ProjectFile = Sdl.Community.Transcreate.Model.ProjectFile;
using TaskFile = Sdl.Community.Transcreate.Model.Tasks.TaskFile;

namespace Sdl.Community.Transcreate
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
		private List<Project> _transcreateProjects;
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;
		private ProjectFilesViewControl _projectFilesViewControl;
		private ProjectsNavigationViewControl _projectsNavigationViewControl;
		private ProjectFileActivityViewController _projectFileActivityViewController;
		//private ProjectPropertiesViewController _projectPropertiesViewController; 
		private ProjectsController _projectsController;
		private EditorController _editorController;
		private FilesController _filesController;
		private ImageService _imageService;
		private PathInfo _pathInfo;
		private CustomerProvider _customerProvider;
		private ProjectSettingsService _projectSettingsService;
		private ReportService _reportService;

		protected override void Initialize(IViewContext context)
		{
			ClientId = Guid.NewGuid().ToString();

			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_customerProvider = new CustomerProvider();
			_projectSettingsService = new ProjectSettingsService();
			_reportService = new ReportService();

			ActivationChanged += OnActivationChanged;

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;

			_filesController = SdlTradosStudio.Application.GetController<FilesController>();
			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_editorController.Opened += EditorController_Opened;
			_editorController.Closed += EditorController_Closed;

			ReportsController = ReportsController.Instance;

			LoadProjects();
		}

		protected override IUIControl GetExplorerBarControl()
		{
			if (_projectsNavigationViewControl == null)
			{
				_projectsNavigationViewModel = new ProjectsNavigationViewModel(new List<Project>(), _projectsController);
				_projectsNavigationViewModel.ProjectSelectionChanged += OnProjectSelectionChanged;

				_projectsNavigationViewControl = new ProjectsNavigationViewControl(_projectsNavigationViewModel);
			}

			return _projectsNavigationViewControl;
		}

		protected override IUIControl GetContentControl()
		{
			if (_projectFilesViewControl == null)
			{
				_projectFilesViewModel = new ProjectFilesViewModel(null);
				_projectFilesViewControl = new ProjectFilesViewControl(_projectFilesViewModel);
				_projectsNavigationViewModel.ProjectFilesViewModel = _projectFilesViewModel;

				_projectFilesViewModel.ProjectFileSelectionChanged += ProjectFilesViewModel_ProjectFileSelectionChanged;

				if (_transcreateProjects.Count > 0)
				{
					_transcreateProjects[0].IsSelected = true;
				}
				_projectsNavigationViewModel.Projects = _transcreateProjects;

			}

			return _projectFilesViewControl;
		}

		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;

		public EventHandler<ProjectFileSelectionChangedEventArgs> ProjectFileSelectionChanged;

		public bool IsActive { get; set; }

		public bool OverrideEditorWarningMessage { get; set; }

		public void RefreshProjects()
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
					var xliffProject = _transcreateProjects.FirstOrDefault(a => a.Id == projectInfo.Id.ToString());
					if (xliffProject != null)
					{
						var updatedCustomer = UpdateCustomerInfo(project, xliffProject);
						var addedNewFiles = AddNewProjectFiles(project, xliffProject);

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

			if (refresh)
			{
				_projectsNavigationViewModel.Projects = new List<Project>();
				_projectsNavigationViewModel.Projects = _transcreateProjects;
			}
		}

		public List<Project> GetProjects()
		{
			return _transcreateProjects;
		}

		public List<Project> GetSelectedProjects()
		{
			if (_projectsNavigationViewModel.SelectedProjects != null)
			{
				return _projectsNavigationViewModel.SelectedProjects?.Cast<Project>().ToList();
			}

			return new List<Project> { _projectsNavigationViewModel.SelectedProject };
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


		public void UpdateProjectData(TaskContext taskContext, bool isBatchTask = false)
		{
			if (taskContext == null || !taskContext.Completed)
			{
				return;
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

			if (_projectFilesViewModel != null)
			{
				_projectFilesViewModel.ProjectFiles = new List<ProjectFile>();
				_projectFilesViewModel.ProjectFiles = project.ProjectFiles;
			}

			if (_projectsNavigationViewModel != null)
			{
				_projectsNavigationViewModel.Projects = new List<Project>();
				_projectsNavigationViewModel.Projects = _transcreateProjects;
			}

			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == taskContext.Project.Id);
			if (selectedProject == null)
			{
				return;
			}

			CreateReports(taskContext, selectedProject);

			UpdateProjectSettingsBundle(project);
		}

		internal ReportsController ReportsController { get; private set; }

		internal string ClientId { get; private set; }

		private void CreateReports(TaskContext taskContext, FileBasedProject selectedProject)
		{
			var automaticTask = CreateAutomaticTask(taskContext, selectedProject);
			var reports = CreateHtmlReports(taskContext, selectedProject, automaticTask);

			// add to reports controller
			ReportsController.AddReports(ClientId, reports);
		}

		private bool UpdateCustomerInfo(FileBasedProject project, Project xliffProject)
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

		private void ConvertToRelativePaths(Project project, ProjectFile wcProjectFile)
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

		private List<Reports.Viewer.API.Model.Report> CreateHtmlReports(TaskContext taskContext, FileBasedProject selectedProject, AutomaticTask automaticTask)
		{
			var reports = new List<Reports.Viewer.API.Model.Report>();

			var project = _transcreateProjects.FirstOrDefault(a => a.Id == taskContext.Project.Id);
			var languageDirections = _projectSettingsService.GetLanguageDirections(selectedProject.FilePath);
			var reportTemplate = GetReportTemplatePath();

			foreach (var taskReport in automaticTask.Reports)
			{
				var languageDirection = languageDirections.FirstOrDefault(a =>
					string.Compare(taskReport.LanguageDirectionGuid, a.Guid, StringComparison.CurrentCultureIgnoreCase) == 0);

				var reportName = Path.GetFileName(taskReport.PhysicalPath);
				var reportFilePath = Path.Combine(taskContext.WorkingFolder, reportName);

				var htmlReportFilePath = CreateHtmlReportFile(reportFilePath, reportTemplate);

				var name = string.Empty;
				var groupName = string.Empty;
				var description = string.Empty;
				switch (taskContext.Action)
				{
					case Enumerators.Action.Convert:
						name = "Create Transcreate Project";
						groupName = PluginResources.ReportsGroup_ProjectCreation;
						description = "Created transcreate project";
						break;
					case Enumerators.Action.CreateBackTranslation:
						name = "Create Back-Translation Project";
						groupName = PluginResources.ReportsGroup_ProjectCreation;
						description = "Created back-translation project";
						break;
					case Enumerators.Action.Export:
						name = "Export Translations";
						groupName = "Export";
						description = "Exported for translation";
						break;
					case Enumerators.Action.Import:
						name = "Import Translations";
						groupName = "Import";
						description = "Imported translations";
						break;
					case Enumerators.Action.ExportBackTranslation:
						name = "Export Back-Translations";
						groupName = "Export";
						description = "Exported for back-translation";
						break;
					case Enumerators.Action.ImportBackTranslation:
						name = "Import Back-Translations";
						groupName = "Import";
						description = "Imported back-translation";
						break;
				}

				var report =
					new Reports.Viewer.API.Model.Report
					{
						Path = reportFilePath,
						XsltPath = reportTemplate,
						Group = groupName,
						Date = DateTime.Now,
						Name = name,
						Language = languageDirection?.TargetLanguageCode,
						Description = description
					};

				reports.Add(report);

				foreach (var wcProjectFile in taskContext.ProjectFiles)
				{
					if (!wcProjectFile.Selected || string.Compare(wcProjectFile.TargetLanguage, languageDirection?.TargetLanguageCode,
							StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						continue;
					}

					var projectFile = project?.ProjectFiles.FirstOrDefault(a => a.FileId == wcProjectFile.FileId);
					if (projectFile != null)
					{
						projectFile.Report = GetRelativePath(project.Path, htmlReportFilePath);

						var activityfile = projectFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault();
						if (activityfile != null)
						{
							activityfile.Report = projectFile.Report;
						}
					}
				}
			}

			return reports;
		}

		private AutomaticTask CreateAutomaticTask(TaskContext taskContext, FileBasedProject selectedProject)
		{
			var projectInfo = selectedProject.GetProjectInfo();
			var automaticTask = new AutomaticTask(taskContext.Action)
			{
				CreatedAt = GetDateToString(taskContext.DateTimeStamp),
				StartedAt = GetDateToString(taskContext.DateTimeStamp),
				CompletedAt = GetDateToString(taskContext.DateTimeStamp),
				CreatedBy = Environment.UserDomainName + "\\" + Environment.UserName
			};

			foreach (var wcProjectFile in taskContext.ProjectFiles)
			{
				if (wcProjectFile.Selected)
				{
					var taskFile = new TaskFile
					{
						LanguageFileGuid = wcProjectFile.FileId
					};
					automaticTask.TaskFiles.Add(taskFile);

					var outputFile = new OutputFile
					{
						LanguageFileGuid = wcProjectFile.FileId
					};
					automaticTask.OutputFiles.Add(outputFile);
				}
			}

			var languageDirections = GetLanguageDirectionFiles(selectedProject.FilePath, taskContext);

			foreach (var languageDirection in languageDirections)
			{
				var reportName = string.Format("{0}_{1}_{2}_{3}.xml",
					taskContext.Action.ToString().Replace(" ", ""),
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
				var relativeProjectReportsFilePath = Path.Combine("Reports", reportName);

				_reportService.CreateReport(taskContext, reportFile, selectedProject, languageDirection.Key.TargetLanguageCode);

				// Copy to project reports folder
				File.Copy(reportFile, projectReportsFilePath, true);

				var report = new Report(taskContext.Action)
				{
					Name = taskContext.Action.ToString(),
					Description = taskContext.Action.ToString(),
					LanguageDirectionGuid = languageDirection.Key.Guid,
					PhysicalPath = relativeProjectReportsFilePath
				};

				automaticTask.Reports.Add(report);
			}

			return automaticTask;
		}

		public string GetReportTemplatePath()
		{
			var name = "TranscreateReport.xsl";
			var filePath = Path.Combine(_pathInfo.SettingsFolderPath, name);
			var resourceName = "Sdl.Community.Transcreate.BatchTasks.TranscreateReport.xsl";

			WriteResourceToFile(resourceName, filePath);

			return filePath;
		}

		public void WriteResourceToFile(string resourceName, string fullFilePath)
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

		private void UpdateProjectSettingsBundle(Project project)
		{
			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);

			if (selectedProject != null)
			{
				var settingsBundle = selectedProject.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();

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


				xliffManagerProject.ProjectFilesJson.Value = JsonConvert.SerializeObject(projectFiles);

				selectedProject.UpdateSettings(xliffManagerProject.SettingsBundle);

				selectedProject.Save();
			}
		}

		private void LoadProjects()
		{
			_transcreateProjects = new List<Project>();

			foreach (var project in _projectsController.GetAllProjects())
			{
				AddNewProjectToContainer(project);
			}
		}

		private static List<SDLTranscreateProjectFile> SerializeProjectFiles(string value)
		{
			try
			{
				var projectFiles =
					JsonConvert.DeserializeObject<List<SDLTranscreateProjectFile>>(value);
				return projectFiles;
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private string GetRelativePath(string projectPath, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			return path.Replace(projectPath.Trim('\\') + '\\', string.Empty);
		}

		private void EditorController_Opened(object sender, DocumentEventArgs e)
		{
			if (e.Document.Files?.Count() == 0 || e.Document?.Project == null)
			{
				return;
			}

			var project = e.Document.Project;
			var projectInfo = project.GetProjectInfo();
			var projectId = projectInfo.Id.ToString();
			var document = e.Document.Files?.FirstOrDefault();
			var documentId = document?.Id.ToString();
			var language = document?.Language;

			var transcreateProject = _transcreateProjects.FirstOrDefault(a => a.Id == projectId);
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
				var projectAutomationService = new ProjectAutomationService(_imageService, this, _customerProvider);

				var action = transcreateProject.IsBackTranslationProject
					? Enumerators.Action.ExportBackTranslation
					: Enumerators.Action.Export;
				var workFlow = Enumerators.WorkFlow.Internal;
				var setttings = GetSettings();

				var taskContext = new TaskContext(action, workFlow, setttings);
				taskContext.AnalysisBands = projectAutomationService.GetAnalysisBands(project);
				taskContext.ExportOptions.IncludeBackTranslations = true;
				taskContext.ExportOptions.IncludeTranslations = true;
				taskContext.ExportOptions.CopySourceToTarget = false;

				taskContext.LocalProjectFolder = projectInfo.LocalProjectFolder;
				taskContext.WorkflowFolder = taskContext.GetWorkflowPath();
				var workingProject = projectAutomationService.GetProject(project, new List<string> { projectFile.FileId });

				taskContext.Project = workingProject;
				taskContext.ProjectFiles = workingProject.ProjectFiles;

				var targetFile = taskContext.ProjectFiles.FirstOrDefault(a => a.FileId == projectFile.FileId);

				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);
				var sdlxliffReader = new SdlxliffReader(segmentBuilder, taskContext.ExportOptions, taskContext.AnalysisBands);

				var languageFolder = GetLanguageFolder(taskContext, targetFile.TargetLanguage);

				var xliffFolder = GetXliffFolder(languageFolder, targetFile);
				var xliffFilePath = Path.Combine(xliffFolder, targetFile.Name + ".xliff");

				var inputPath = Path.Combine(taskContext.LocalProjectFolder, targetFile.Location);
				var data = sdlxliffReader.ReadFile(transcreateProject.Id, inputPath, targetFile.TargetLanguage);
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

				UpdateProjectData(taskContext);
			}
		}

		private void EditorController_Closed(object sender, DocumentEventArgs e)
		{
			if (e.Document.Files?.Count() == 0 || e.Document?.Project == null)
			{
				return;
			}

			var project = e.Document.Project;
			var projectInfo = project.GetProjectInfo();
			var projectId = projectInfo.Id.ToString();
			var document = e.Document.Files?.FirstOrDefault();
			var documentId = document?.Id.ToString();
			var language = document?.Language;

			var transcreateProject = _transcreateProjects.FirstOrDefault(a => a.Id == projectId);
			var projectFile = transcreateProject?.ProjectFiles.FirstOrDefault(a => a.FileId == documentId &&
																			a.TargetLanguage == language.CultureInfo.Name);
			if (projectFile != null)
			{
				var segmentBuilder = new SegmentBuilder();
				var projectAutomationService = new ProjectAutomationService(_imageService, this, _customerProvider);

				var action = transcreateProject.IsBackTranslationProject
					? Enumerators.Action.ImportBackTranslation
					: Enumerators.Action.Import;
				var workFlow = Enumerators.WorkFlow.Internal;
				var setttings = GetSettings();

				var taskContext = new TaskContext(action, workFlow, setttings);
				taskContext.AnalysisBands = projectAutomationService.GetAnalysisBands(project);
				taskContext.ExportOptions.IncludeBackTranslations = true;
				taskContext.ExportOptions.IncludeTranslations = true;
				taskContext.ExportOptions.CopySourceToTarget = false;
				taskContext.ImportOptions.StatusTranslationUpdatedId = string.Empty;
				taskContext.ImportOptions.StatusSegmentNotImportedId = string.Empty;
				taskContext.ImportOptions.StatusTranslationNotUpdatedId = string.Empty;
				taskContext.ImportOptions.OverwriteTranslations = true;
				taskContext.ImportOptions.OriginSystem = "Transcreate Automation";

				taskContext.LocalProjectFolder = projectInfo.LocalProjectFolder;
				taskContext.WorkflowFolder = taskContext.GetWorkflowPath();
				var workingProject = projectAutomationService.GetProject(project, new List<string> { projectFile.FileId });

				taskContext.Project = workingProject;
				taskContext.ProjectFiles = workingProject.ProjectFiles;

				var targetFile = taskContext.ProjectFiles.FirstOrDefault(a => a.FileId == projectFile.FileId);

				var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);

				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);
				var sdlxliffReader = new SdlxliffReader(segmentBuilder, taskContext.ExportOptions, taskContext.AnalysisBands);
				var sdlxliffWriter = new SdlxliffWriter(fileTypeManager, segmentBuilder, taskContext.ImportOptions, taskContext.AnalysisBands);

				var languageFolder = GetLanguageFolder(taskContext, targetFile.TargetLanguage);

				var xliffFolder = GetXliffFolder(languageFolder, targetFile);
				var xliffFilePath = Path.Combine(xliffFolder, targetFile.Name + ".xliff");

				var inputPath = Path.Combine(taskContext.LocalProjectFolder, targetFile.Location);
				var data = sdlxliffReader.ReadFile(transcreateProject.Id, inputPath, targetFile.TargetLanguage);

				var exported = xliffWriter.WriteFile(data, xliffFilePath, true);

				var tempFileName = Path.GetTempFileName();
				var tmpInputFile = inputPath + ".tmp.sdlxliff";
				var imported = sdlxliffWriter.UpdateFile(data, tmpInputFile, tempFileName);

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
					ConfirmationStatistics = sdlxliffWriter.ConfirmationStatistics,
					TranslationOriginStatistics = sdlxliffWriter.TranslationOriginStatistics
				};

				targetFile.ProjectFileActivities.Add(activityFile);
				taskContext.Completed = true;

				UpdateProjectData(taskContext);
			}
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

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			ProjectSelectionChanged?.Invoke(this, e);
		}

		private void ProjectFilesViewModel_ProjectFileSelectionChanged(object sender, ProjectFileSelectionChangedEventArgs e)
		{
			ProjectFileSelectionChanged?.Invoke(sender, e);
		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			IsActive = e.Active;

			if (e.Active)
			{
				//SetProjectPropertiesViewController();
				SetProjectFileActivityViewController();
				_projectFilesViewModel.Refresh();
			}
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

		//private void SetProjectPropertiesViewController()
		//{
		//	if (_projectPropertiesViewController != null)
		//	{
		//		return;
		//	}

		//	try
		//	{
		//		_projectPropertiesViewController =
		//			SdlTradosStudio.Application.GetController<ProjectPropertiesViewController>();

		//		var viewModel = _projectsNavigationViewModel.ProjectPropertiesViewModel =
		//			new ProjectPropertiesViewModel(_projectsNavigationViewModel.SelectedProject);

		//		_projectPropertiesViewController.ViewModel = viewModel;
		//	}
		//	catch
		//	{
		//		// catch all; unable to locate the controller
		//	}
		//}

		private bool AddNewProjectToContainer(FileBasedProject project)
		{
			if (project == null)
			{
				return false;
			}

			var projectInfo = project.GetProjectInfo();
			try
			{
				if (_transcreateProjects.FirstOrDefault(a => a.Id == projectInfo.Id.ToString()) != null)
				{
					// allready present in the list
					return false;
				}

				var settingsBundle = project.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();

				var projectFiles = SerializeProjectFiles(xliffManagerProject.ProjectFilesJson.Value);
				if (projectFiles?.Count > 0)
				{
					var xliffProjectFiles = new List<ProjectFile>();

					var xliffProject = new Project
					{
						Id = projectInfo.Id.ToString(),
						Name = projectInfo.Name,
						AbsoluteUri = projectInfo.Uri.AbsoluteUri,
						Customer = _customerProvider.GetProjectCustomer(project),
						Created = projectInfo.CreatedAt.ToUniversalTime(),
						DueDate = projectInfo.DueDate?.ToUniversalTime() ?? DateTime.MaxValue,
						Path = projectInfo.LocalProjectFolder,
						SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo),
						TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages),
						ProjectType = GetProjectType(project),
						ProjectFiles = xliffProjectFiles
					};

					foreach (var projectFile in projectFiles)
					{
						var xliffFileActivities = new List<ProjectFileActivity>();

						var xliffProjectFile = new ProjectFile
						{
							Path = projectFile.Path,
							Action = GetAction(projectFile.Action),
							WorkFlow = GetWorkFlow(projectFile.WorkFlow),
							Status = GetStatus(projectFile.Status),
							Date = GetDateTime(projectFile.Date),
							Report = GetRelativePath(xliffProject.Path, projectFile.Report),
							FileId = projectFile.FileId,
							FileType = projectFile.FileType,
							Location = GetRelativePath(xliffProject.Path, projectFile.Location),
							Name = projectFile.Name,
							ProjectId = projectInfo.Id.ToString(),
							ProjectFileActivities = xliffFileActivities,
							ExternalFilePath = GetRelativePath(xliffProject.Path, projectFile.ExternalFilePath),
							Project = xliffProject,
							TargetLanguage = projectFile.TargetLanguage,
							ConfirmationStatistics = projectFile.ConfirmationStatistics,
							TranslationOriginStatistics = projectFile.TranslationOriginStatistics
						};

						foreach (var fileActivity in projectFile.Activities)
						{
							var xliffFileActivity = new ProjectFileActivity
							{
								ProjectFile = xliffProjectFile,
								Path = GetRelativePath(xliffProject.Path, fileActivity.Path),
								Action = GetAction(fileActivity.Action),
								WorkFlow = GetWorkFlow(fileActivity.WorkFlow),
								Status = GetStatus(fileActivity.Status),
								ActivityId = fileActivity.ActivityId,
								Date = GetDateTime(fileActivity.Date),
								Report = GetRelativePath(xliffProject.Path, fileActivity.Report),
								Name = fileActivity.Name,
								ProjectFileId = fileActivity.ProjectFileId,
								ConfirmationStatistics = fileActivity.ConfirmationStatistics,
								TranslationOriginStatistics = fileActivity.TranslationOriginStatistics
							};

							xliffFileActivities.Add(xliffFileActivity);
						}

						xliffProject.ProjectFiles.Add(xliffProjectFile);
					}

					var addedNewFiles = AddNewProjectFiles(project, xliffProject);
					if (addedNewFiles)
					{
						UpdateProjectSettingsBundle(xliffProject);
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

		private bool AddNewProjectFiles(FileBasedProject project, Project xliffProject)
		{
			var addedNewFiles = false;
			var projectInfo = project.GetProjectInfo();

			foreach (var targetLanguage in projectInfo.TargetLanguages)
			{
				var projectFiles = project.GetTargetLanguageFiles(targetLanguage);
				foreach (var projectFile in projectFiles)
				{
					if (projectFile.Role != FileRole.Translatable)
					{
						continue;
					}

					var xliffFile = xliffProject.ProjectFiles.FirstOrDefault(a => a.FileId == projectFile.Id.ToString());
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
							Project = xliffProject
						};

						xliffProject.ProjectFiles.Add(file);
					}
				}
			}

			return addedNewFiles;
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

		private List<Project> GetRemovedProjects()
		{
			var removedProjects = new List<Project>();
			var studioProjects = _projectsController.GetAllProjects().ToList();
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

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			var updated = AddNewProjectToContainer(_projectsController?.CurrentProject);
			if (!updated)
			{
				updated = UnloadRemovedProjectsFromContainer();
			}

			if (updated && _projectsNavigationViewModel != null)
			{
				_projectsNavigationViewModel.Projects = new List<Project>();
				_projectsNavigationViewModel.Projects = _transcreateProjects;
			}
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
