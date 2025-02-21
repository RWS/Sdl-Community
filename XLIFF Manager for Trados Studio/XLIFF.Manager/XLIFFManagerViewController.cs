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
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Controls;
using Sdl.Community.XLIFF.Manager.CustomEventArgs;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.Community.XLIFF.Manager.Model.Tasks;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using AutomaticTask = Sdl.Community.XLIFF.Manager.Model.Tasks.AutomaticTask;
using ProjectFile = Sdl.Community.XLIFF.Manager.Model.ProjectFile;
using TaskFile = Sdl.Community.XLIFF.Manager.Model.Tasks.TaskFile;

namespace Sdl.Community.XLIFF.Manager
{
	[View(
		Id = "XLIFFManager_View",
		Name = "XLIFFManager_Name",
		Description = "XLIFFManager_Description",
		Icon = "Icon",
		AllowViewParts = true,
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class XLIFFManagerViewController : AbstractViewController
	{
		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;
		private readonly object _lockObject = new object();
		private CustomerProvider _customerProvider;
		private EditorController _editorController;
		private FilesController _filesController;
		private ImageService _imageService;
		private PathInfo _pathInfo;
		private ProjectFileActivityViewController _projectFileActivityViewController;
		private ProjectFilesViewControl _projectFilesViewControl;
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsController _projectsController;
		private ProjectSettingsService _projectSettingsService;
		private ProjectsNavigationViewControl _projectsNavigationViewControl;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;
		private ReportService _reportService;
		private bool _supressProjectControllerEvents;
		private List<Project> _xliffProjects;

		public bool IsInitialized { get; set; }

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

		public List<Project> GetProjects()
		{
			return _xliffProjects;
		}

		public List<ProjectFile> GetSelectedProjectFiles()
		{
			return _projectFilesViewModel.SelectedProjectFiles?.Cast<ProjectFile>().ToList();
		}

		public List<Project> GetSelectedProjects()
		{
			return _projectsNavigationViewModel.SelectedProjects?.Cast<Project>().ToList();
		}

		public void Initialize() => Initialize(null);

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
					var xliffProject = _xliffProjects.FirstOrDefault(a => a.Id == projectInfo.Id.ToString());
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
				_projectsNavigationViewModel.Projects = _xliffProjects;
			}
		}

		public void UpdateProjectData(WizardContext wizardContext, bool isBatchTask = false)
		{
			if (wizardContext == null || !wizardContext.Completed)
			{
				return;
			}

			var project = _xliffProjects.FirstOrDefault(a => a.Id == wizardContext.Project.Id);
			if (project == null)
			{
				_xliffProjects.Add(wizardContext.Project);

				foreach (var wcProjectFile in wizardContext.ProjectFiles)
				{
					ConvertToRelativePaths(wizardContext.Project, wcProjectFile);
				}

				project = wizardContext.Project;
			}
			else
			{
				foreach (var wcProjectFile in wizardContext.ProjectFiles)
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

						projectFile.XliffFilePath = wcProjectFile.XliffFilePath;
						projectFile.Location = wcProjectFile.Location;
						projectFile.Report = wcProjectFile.Report;
						projectFile.Status = wcProjectFile.Status;
						projectFile.Action = wcProjectFile.Action;
						projectFile.Date = wcProjectFile.Date;
						projectFile.ConfirmationStatistics = wcProjectFile.ConfirmationStatistics;
						projectFile.TranslationOriginStatistics = wcProjectFile.TranslationOriginStatistics;
						projectFile.ProjectFileActivities = wcProjectFile.ProjectFileActivities;
					}
				}
			}

			if (_projectFilesViewModel != null)
			{
				_projectFilesViewModel.ProjectFiles = project.ProjectFiles;
			}

			if (_projectsNavigationViewModel != null)
			{
				_projectsNavigationViewModel.Projects = new List<Project>();
				_projectsNavigationViewModel.Projects = _xliffProjects;
			}

			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == wizardContext.Project.Id);
			if (selectedProject == null)
			{
				return;
			}

			var automaticTask = CreateAutomaticTask(wizardContext, selectedProject, isBatchTask);
			CreateHtmlReport(wizardContext, selectedProject, automaticTask);
			if (!isBatchTask)
			{
				UpdateProjectReports(wizardContext, selectedProject, automaticTask);
			}

			UpdateProjectSettingsBundle(project);
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

		protected override IUIControl GetContentControl()
		{
			if (_projectFilesViewControl == null)
			{
				_projectFilesViewModel = new ProjectFilesViewModel(null);

				_projectFilesViewControl = new ProjectFilesViewControl(_projectFilesViewModel);
				_projectsNavigationViewModel.ProjectFilesViewModel = _projectFilesViewModel;
				_projectsNavigationViewModel.Projects = _xliffProjects;
			}

			return _projectFilesViewControl;
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

		protected override void Initialize(IViewContext context)
		{
			if (IsInitialized)
				return;

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

			LoadProjects();

			IsInitialized = true;
		}

		private static List<XliffManagerProjectFile> SerializeProjectFiles(string value)
		{
			try
			{
				var projectFiles =
					JsonConvert.DeserializeObject<List<XliffManagerProjectFile>>(value);
				return projectFiles;
			}
			catch
			{
				// catch all; ignore
			}

			return null;
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

		private bool AddNewProjectToContainer(FileBasedProject project)
		{
			if (project == null)
			{
				return false;
			}

			var projectInfo = project.GetProjectInfo();
			try
			{
				if (_xliffProjects.FirstOrDefault(a => a.Id == projectInfo.Id.ToString()) != null)
				{
					// allready present in the list
					return false;
				}

				var settingsBundle = project.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<XliffManagerProject>();

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
							Status = GetStatus(projectFile.Status),
							Date = GetDateTime(projectFile.Date),
							Report = GetRelativePath(xliffProject.Path, projectFile.Report),
							FileId = projectFile.FileId,
							FileType = projectFile.FileType,
							Location = GetRelativePath(xliffProject.Path, projectFile.Location),
							Name = projectFile.Name,
							ProjectId = projectInfo.Id.ToString(),
							ProjectFileActivities = xliffFileActivities,
							XliffFilePath = GetRelativePath(xliffProject.Path, projectFile.XliffFilePath),
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

					_xliffProjects.Add(xliffProject);

					return true;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Unable to load project: {0}", projectInfo.Name)
								+ Environment.NewLine + Environment.NewLine + ex.Message,
					PluginResources.Plugin_Name, MessageBoxButtons.OK,
					MessageBoxIcon.Information);
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

			wcProjectFile.XliffFilePath = GetRelativePath(project.Path, wcProjectFile.XliffFilePath);
			wcProjectFile.Location = GetRelativePath(project.Path, wcProjectFile.Location);
			wcProjectFile.Report = GetRelativePath(project.Path, wcProjectFile.Report);
		}

		private AutomaticTask CreateAutomaticTask(WizardContext wizardContext, FileBasedProject selectedProject, bool isBatchTask)
		{
			var projectInfo = selectedProject.GetProjectInfo();
			var automaticTask = new AutomaticTask(wizardContext.Action)
			{
				CreatedAt = GetDateToString(wizardContext.DateTimeStamp),
				StartedAt = GetDateToString(wizardContext.DateTimeStamp),
				CompletedAt = GetDateToString(wizardContext.DateTimeStamp),
				CreatedBy = Environment.UserDomainName + "\\" + Environment.UserName
			};

			foreach (var wcProjectFile in wizardContext.ProjectFiles)
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

			var languageDirections = GetLanguageDirectionFiles(selectedProject.FilePath, wizardContext);

			foreach (var languageDirection in languageDirections)
			{
				var actionName = wizardContext.Action == Enumerators.Action.Export
					? "Export To XLIFF"
					: "Import From XLIFF";

				var reportName = string.Format("{0}_{1}_{2}_{3}.xml",
					actionName.Replace(" ", ""),
					wizardContext.DateTimeStampToString,
					languageDirection.Key.SourceLanguageCode,
					languageDirection.Key.TargetLanguageCode);

				var projectsReportsFolder = Path.Combine(projectInfo.LocalProjectFolder, "Reports");
				if (!Directory.Exists(projectsReportsFolder))
				{
					Directory.CreateDirectory(projectsReportsFolder);
				}

				var reportFile = Path.Combine(wizardContext.WorkingFolder, reportName);

				var projectReportsFilePath = Path.Combine(projectsReportsFolder, reportName);
				var relativeProjectReportsFilePath = Path.Combine("Reports", reportName);

				_reportService.CreateReport(wizardContext, reportFile, selectedProject, languageDirection.Key.TargetLanguageCode);

				// Copy to project reports folder
				if (!isBatchTask)
				{
					File.Copy(reportFile, projectReportsFilePath, true);
				}

				var report = new Report(wizardContext.Action)
				{
					Name = actionName,
					Description = actionName,
					LanguageDirectionGuid = languageDirection.Key.Guid,
					PhysicalPath = relativeProjectReportsFilePath
				};

				automaticTask.Reports.Add(report);
			}

			return automaticTask;
		}

		private void CreateHtmlReport(WizardContext wizardContext, FileBasedProject selectedProject, AutomaticTask automaticTask)
		{
			var project = _xliffProjects.FirstOrDefault(a => a.Id == wizardContext.Project.Id);
			var languageDirections = _projectSettingsService.GetLanguageDirections(selectedProject.FilePath);
			foreach (var taskReport in automaticTask.Reports)
			{
				var languageDirection = languageDirections.FirstOrDefault(a =>
					string.Compare(taskReport.LanguageDirectionGuid, a.Guid, StringComparison.CurrentCultureIgnoreCase) == 0);

				var reportName = Path.GetFileName(taskReport.PhysicalPath);
				var reportFilePath = Path.Combine(wizardContext.WorkingFolder, reportName);
				var htmlReportFilePath = CreateHtmlReportFile(reportFilePath);

				foreach (var wcProjectFile in wizardContext.ProjectFiles)
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
		}

		private string CreateHtmlReportFile(string xmlReportFullPath)
		{
			var htmlReportFilePath = xmlReportFullPath + ".html";
			var xsltName = "Report.xsl";
			var xsltFilePath = Path.Combine(_pathInfo.SettingsFolderPath, xsltName);
			var xsltResourceFilePath = "Sdl.Community.XLIFF.Manager.BatchTasks.Report.xsl";

			WriteResourceToFile(xsltResourceFilePath, xsltFilePath);

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

		private void EditorController_Opened(object sender, DocumentEventArgs e)
		{
			if (e?.Document?.ActiveFile == null || e?.Document?.Project == null)
			{
				return;
			}

			var project = e.Document.Project;
			var projectInfo = project.GetProjectInfo();
			var projectId = projectInfo.Id.ToString();
			var documentId = e.Document.ActiveFile.Id.ToString();
			var language = e.Document.ActiveFile.Language;

			var xliffProject = _xliffProjects.FirstOrDefault(a => a.Id == projectId);
			var targetFile = xliffProject?.ProjectFiles.FirstOrDefault(a => a.FileId == documentId &&
											 a.TargetLanguage == language.CultureInfo.Name);

			if (targetFile != null && targetFile.Action == Enumerators.Action.Export)
			{
				var activityfile = targetFile.ProjectFileActivities.OrderByDescending(a => a.Date).FirstOrDefault(a => a.Action == Enumerators.Action.Export);

				var message1 = string.Format(PluginResources.Message_FileWasExportedOn, activityfile?.DateToString);
				var message2 = string.Format(PluginResources.Message_WarningTranslationsCanBeOverwrittenDuringImport, activityfile?.DateToString);

				MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2, PluginResources.XLIFFManager_Name, MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
			}
		}

		private Enumerators.Action GetAction(string value)
		{
			var successAction = Enum.TryParse<Enumerators.Action>(value, true, out var resultAction);
			var action = successAction ? resultAction : Enumerators.Action.None;
			return action;
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

		private Dictionary<LanguageDirectionInfo, List<ProjectFile>> GetLanguageDirectionFiles(string projectsFile, WizardContext wizardContext)
		{
			var languageDirections = new Dictionary<LanguageDirectionInfo, List<ProjectFile>>();
			foreach (var language in _projectSettingsService.GetLanguageDirections(projectsFile))
			{
				foreach (var projectFile in wizardContext.ProjectFiles)
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

		private LanguageInfo GetLanguageInfo(CultureInfo cultureInfo)
		{
			var languageInfo = new LanguageInfo
			{
				CultureInfo = cultureInfo,
				Image = _imageService.GetImage(cultureInfo.Name)
			};

			return languageInfo;
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

		private string GetRelativePath(string projectPath, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			return path.Replace(projectPath.Trim('\\') + '\\', string.Empty);
		}

		private List<Project> GetRemovedProjects()
		{
			var removedProjects = new List<Project>();
			var studioProjects = _projectsController.GetAllProjects().ToList();
			foreach (var project in _xliffProjects)
			{
				var studioProject = studioProjects.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);
				if (studioProject == null)
				{
					removedProjects.Add(project);
				}
			}

			return removedProjects;
		}

		private Enumerators.Status GetStatus(string value)
		{
			var successStatus = Enum.TryParse<Enumerators.Status>(value, true, out var resultStatus);
			var status = successStatus ? resultStatus : Enumerators.Status.None;
			return status;
		}

		private void LoadProjects()
		{
			_xliffProjects = new List<Project>();

			foreach (var project in _projectsController.GetAllProjects())
			{
				AddNewProjectToContainer(project);
			}
		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			if (e.Active)
			{
				SetProjectFileActivityViewController();
				_projectFilesViewModel.Refresh();
			}
		}

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			ProjectSelectionChanged?.Invoke(this, e);
		}

		private void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			if (_supressProjectControllerEvents)
			{
				return;
			}

			var updated = AddNewProjectToContainer(_projectsController?.CurrentProject);
			if (!updated)
			{
				updated = UnloadRemovedProjectsFromContainer();
			}

			if (updated && _projectsNavigationViewModel != null)
			{
				_projectsNavigationViewModel.Projects = new List<Project>();
				_projectsNavigationViewModel.Projects = _xliffProjects;
			}
		}

		private void SetProjectFileActivityViewController()
		{
			lock (_lockObject)
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
					_xliffProjects.Remove(project);
				}
			}

			return updated;
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

		private void UpdateProjectReports(WizardContext wizardContext, FileBasedProject project, AutomaticTask automaticTask)
		{
			if (project != null)
			{
				try
				{
					_supressProjectControllerEvents = true;

					_projectsController.Close(project);
					_projectSettingsService.UpdateAnalysisTaskReportInfo(project, automaticTask);
					_projectsController.Add(project.FilePath);

					switch (wizardContext.Owner)
					{
						case Enumerators.Controller.Files:
							_filesController.Activate();
							break;

						case Enumerators.Controller.Projects:
							_projectsController.Activate();
							break;
					}
				}
				finally
				{
					_supressProjectControllerEvents = false;
				}
			}
		}

		private void UpdateProjectSettingsBundle(Project project)
		{
			var selectedProject = _projectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);

			if (selectedProject != null)
			{
				var settingsBundle = selectedProject.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<XliffManagerProject>();

				var projectFiles = new List<XliffManagerProjectFile>();
				foreach (var projectFile in project.ProjectFiles)
				{
					var fileActivities = new List<XliffManagerProjectFileActivity>();

					foreach (var fileActivity in projectFile.ProjectFileActivities)
					{
						var settingFileActivity = new XliffManagerProjectFileActivity
						{
							ProjectFileId = projectFile.FileId,
							ActivityId = fileActivity.ActivityId,
							Status = fileActivity.Status.ToString(),
							Action = fileActivity.Action.ToString(),
							Path = GetRelativePath(project.Path, fileActivity.Path),
							Name = fileActivity.Name,
							Date = GetDateToString(fileActivity.Date),
							Report = GetRelativePath(project.Path, fileActivity.Report),
							ConfirmationStatistics = fileActivity.ConfirmationStatistics,
							TranslationOriginStatistics = fileActivity.TranslationOriginStatistics
						};

						fileActivities.Add(settingFileActivity);
					}

					var settingProjectFile = new XliffManagerProjectFile
					{
						Activities = fileActivities,
						Path = GetRelativePath(project.Path, projectFile.Path),
						Action = projectFile.Action.ToString(),
						Status = projectFile.Status.ToString(),
						FileId = projectFile.FileId,
						Name = projectFile.Name,
						Location = GetRelativePath(project.Path, projectFile.Location),
						Date = GetDateToString(projectFile.Date),
						FileType = projectFile.FileType,
						XliffFilePath = GetRelativePath(project.Path, projectFile.XliffFilePath),
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
	}
}