using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Controls;
using Sdl.Community.XLIFF.Manager.CustomEventArgs;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

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
		private readonly object _lockObject = new object();
		private List<Project> _xliffProjects;
		private ProjectFilesViewModel _projectFilesViewModel;
		private ProjectsNavigationViewModel _projectsNavigationViewModel;
		private ProjectFilesViewControl _projectFilesViewControl;
		private ProjectsNavigationViewControl _projectsNavigationViewControl;
		private ProjectFileActivityViewController _projectFileActivityViewController;
		private IStudioEventAggregator _eventAggregator;
		private ProjectsController _projectsController;
		private EditorController _editorController;
		private ImageService _imageService;
		private PathInfo _pathInfo;
		private CustomerProvider _customerProvider;

		protected override void Initialize(IViewContext context)
		{
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_customerProvider = new CustomerProvider();

			ActivationChanged += OnActivationChanged;

			_eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			_eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>()?.Subscribe(OnStudioWindowCreatedNotificationEvent);

			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_editorController.Opened += EditorController_Opened;

			LoadProjects();
		}

		protected override Control GetExplorerBarControl()
		{
			if (_projectsNavigationViewControl == null)
			{
				_projectsNavigationViewModel = new ProjectsNavigationViewModel(new List<Project>(), _projectsController);
				_projectsNavigationViewModel.ProjectSelectionChanged += OnProjectSelectionChanged;

				_projectsNavigationViewControl = new ProjectsNavigationViewControl(_projectsNavigationViewModel);
			}

			return _projectsNavigationViewControl;
		}

		protected override Control GetContentControl()
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

		public EventHandler<ProjectSelectionChangedEventArgs> ProjectSelectionChanged;

		public List<Project> GetProjects()
		{
			return _xliffProjects;
		}

		public List<Project> GetSelectedProjects()
		{
			return _projectsNavigationViewModel.SelectedProjects?.Cast<Project>().ToList();
		}

		public List<ProjectFile> GetSelectedProjectFiles()
		{
			return _projectFilesViewModel.SelectedProjectFiles?.Cast<ProjectFile>().ToList();
		}

		public void UpdateProjectData(WizardContext wizardContext)
		{
			if (wizardContext == null || !wizardContext.Completed)
			{
				return;
			}

			var project = _xliffProjects.FirstOrDefault(a => a.Id == wizardContext.Project.Id);
			if (project == null)
			{
				_xliffProjects.Add(wizardContext.Project);
				project = wizardContext.Project;
			}
			else
			{
				foreach (var wcProjectFile in wizardContext.ProjectFiles)
				{
					var projectFile = project.ProjectFiles.FirstOrDefault(a => a.FileId == wcProjectFile.FileId);
					if (projectFile == null)
					{
						project.ProjectFiles.Add(wcProjectFile);
					}
					else
					{
						projectFile.Status = wcProjectFile.Status;
						projectFile.Action = wcProjectFile.Action;
						projectFile.Date = wcProjectFile.Date;
						projectFile.XliffFilePath = wcProjectFile.XliffFilePath;
						//projectFile.Details = wcProjectFile.Details;

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

			UpdateProjectSettingsBundle(project);
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
							Path = fileActivity.Path,
							Name = fileActivity.Name,
							Date = GetDateToString(fileActivity.Date),
							Details = fileActivity.Details
						};

						fileActivities.Add(settingFileActivity);
					}

					var settingProjectFile = new XliffManagerProjectFile
					{
						Activities = fileActivities,
						Path = projectFile.Path,
						Action = projectFile.Action.ToString(),
						Status = projectFile.Status.ToString(),
						FileId = projectFile.FileId,
						Name = projectFile.Name,
						Location = projectFile.Location,
						Date = GetDateToString(projectFile.Date),
						FileType = projectFile.FileType,
						XliffFilePath = projectFile.XliffFilePath,
						TargetLanguage = projectFile.TargetLanguage.CultureInfo.Name,
						Details = projectFile.Details,
						ShortMessage = projectFile.ShortMessage
					};

					projectFiles.Add(settingProjectFile);
				}

				xliffManagerProject.ProjectFiles.Value = projectFiles;

				selectedProject.UpdateSettings(xliffManagerProject.SettingsBundle);

				selectedProject.Save();
			}
		}

		private void LoadProjects()
		{
			_xliffProjects = new List<Project>();

			foreach (var project in _projectsController.GetAllProjects())
			{
				var projectInfo = project.GetProjectInfo();
				var settingsBundle = project.GetSettings();
				var xliffManagerProject = settingsBundle.GetSettingsGroup<XliffManagerProject>();

				var projectFiles = xliffManagerProject.ProjectFiles.Value;

				if (projectFiles?.Count > 0)
				{
					var xliffProjectFiles = new List<ProjectFile>();

					var xliffProject = new Project
					{
						Id = projectInfo.Id.ToString(),
						Name = projectInfo.Name,
						AbsoluteUri = projectInfo.Uri.AbsoluteUri,
						Customer = _customerProvider.GetProjectCustomer(project),
						Created = projectInfo.CreatedAt,
						DueDate = projectInfo.DueDate ?? DateTime.MaxValue,
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
							Details = projectFile.Details,
							FileId = projectFile.FileId,
							FileType = projectFile.FileType,
							Location = projectFile.Location,
							Name = projectFile.Name,
							ProjectId = projectInfo.Id.ToString(),
							ProjectFileActivities = xliffFileActivities,
							XliffFilePath = projectFile.XliffFilePath,
							Project = xliffProject,
							TargetLanguage = GetLanguageInfo(projectFile.TargetLanguage)
						};

						foreach (var fileActivity in projectFile.Activities)
						{
							var xliffFileActivity = new ProjectFileActivity
							{
								ProjectFile = xliffProjectFile,
								Path = fileActivity.Path,
								Action = GetAction(fileActivity.Action),
								Status = GetStatus(fileActivity.Status),
								ActivityId = fileActivity.ActivityId,
								Date = GetDateTime(fileActivity.Date),
								Details = fileActivity.Details,
								Name = fileActivity.Name,
								ProjectFileId = fileActivity.ProjectFileId
							};

							xliffFileActivities.Add(xliffFileActivity);
						}

						xliffProject.ProjectFiles.Add(xliffProjectFile);
					}

					_xliffProjects.Add(xliffProject);
				}
			}
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
											 a.TargetLanguage.CultureInfo.Name == language.CultureInfo.Name);

			if (targetFile != null && targetFile.Action == Enumerators.Action.Export)
			{
				var activityfile = targetFile.ProjectFileActivities.LastOrDefault(a => a.Action == Enumerators.Action.Export);

				var message1 = string.Format(PluginResources.Message_FileWasExportedOn, activityfile?.DateToString);
				var message2 = string.Format(PluginResources.Message_WarningTranslationsCanBeOverwrittenDuringImport, activityfile?.DateToString);

				MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2, PluginResources.XLIFFManager_Name, MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
			}
		}

		private LanguageInfo GetLanguageInfo(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			return new LanguageInfo
			{
				CultureInfo = new CultureInfo(value),
				Image = _imageService.GetImage(value)
			};
		}

		private DateTime GetDateTime(string value)
		{
			var format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
			var successDate = DateTime.TryParseExact(value, format,
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var resultDate);

			var date = successDate ? resultDate : DateTime.MinValue;
			return date;
		}

		private Enumerators.Status GetStatus(string value)
		{
			var successStatus = Enum.TryParse<Enumerators.Status>(value, true, out var resultStatus);
			var status = successStatus ? resultStatus : Enumerators.Status.None;
			return status;
		}

		private Enumerators.Action GetAction(string value)
		{
			var successAction = Enum.TryParse<Enumerators.Action>(value, true, out var resultAction);
			var action = successAction ? resultAction : Enumerators.Action.None;
			return action;
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

		private void OnProjectSelectionChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			ProjectSelectionChanged?.Invoke(this, e);
		}

		private void OnStudioWindowCreatedNotificationEvent(StudioWindowCreatedNotificationEvent e)
		{

		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			if (e.Active)
			{
				SetProjectFileActivityViewController();
				_projectFilesViewModel.Refresh();
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

		public override void Dispose()
		{
			_projectFilesViewModel?.Dispose();

			if (_editorController != null)
			{
				_editorController.Opened -= EditorController_Opened;
			}

			_projectsNavigationViewModel?.Dispose();


			base.Dispose();
		}
	}
}
