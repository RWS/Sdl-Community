using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.Interfaces;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Interfaces;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Wizard.View;
using Sdl.Community.XLIFF.Manager.Wizard.View.Export;
using Sdl.Community.XLIFF.Manager.Wizard.View.Import;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Export;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Import;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using AnalysisBand = Sdl.Community.XLIFF.Manager.Model.AnalysisBand;
using ProjectFile = Sdl.Community.XLIFF.Manager.Model.ProjectFile;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class WizardService
	{
		private readonly Enumerators.Action _action;
		private readonly PathInfo _pathInfo;
		private readonly CustomerProvider _customerProvider;
		private readonly Controllers _controllers;
		private readonly ImageService _imageService;
		private readonly SegmentBuilder _segmentBuilder;
		private readonly Settings _settings;
		private readonly IDialogService _dialogService;
		private readonly ILanguageProvider _languageProvider;
		private WizardWindow _wizardWindow;
		private ObservableCollection<WizardPageViewModelBase> _pages;
		private WizardContext _wizardContext;
		private bool _isCancelled;
		
		public WizardService(Enumerators.Action action, PathInfo pathInfo, CustomerProvider customerProvider,
			ImageService imageService, Controllers controllers, SegmentBuilder segmentBuilder, Settings settings,
			IDialogService dialogService, ILanguageProvider languageProvider)
		{
			_action = action;
			_pathInfo = pathInfo;
			_customerProvider = customerProvider;
			_imageService = imageService;
			_controllers = controllers;
			_dialogService = dialogService;
			_segmentBuilder = segmentBuilder;
			_settings = settings;
			_languageProvider = languageProvider;
		}

		public WizardContext ShowWizard(AbstractController controller, out string message)
		{
			message = string.Empty;

			if (_action == Enumerators.Action.None)
			{
				message = PluginResources.WizardMessage_NoActionSelected;
				return null;
			}

			var success = CreateWizardContext(controller, out message);
			if (!success)
			{
				return null;
			}

			var documents = _controllers.EditorController.GetDocuments();
			if (documents.Any())
			{
				message = PluginResources.WizardMessage_CloseOpenDocumentsInTheEditor;
				return null;
			}


			_isCancelled = false;
			_wizardWindow = new WizardWindow();
			_wizardWindow.Loaded += WizardWindowLoaded;
			_wizardWindow.ShowDialog();

			if (!_isCancelled && _wizardContext.Completed)
			{
				if (_action == Enumerators.Action.Import)
				{
					_controllers.ProjectsController.RefreshProjects();
				}

				return _wizardContext;
			}

			return null;
		}

		private bool CreateWizardContext(AbstractController controller, out string message)
		{
			_wizardContext = new WizardContext(_action, _settings);

			message = string.Empty;

			if (controller is ProjectsController || controller is FilesController)
			{
				var selectedProject = _controllers.ProjectsController.SelectedProjects.FirstOrDefault()
									  ?? _controllers.ProjectsController.CurrentProject;

				_wizardContext.Owner = controller is ProjectsController
					? Enumerators.Controller.Projects
					: Enumerators.Controller.Files;

				// activate the selected project if diffrent to the current project
				if (_controllers.ProjectsController.CurrentProject?.GetProjectInfo().Id != selectedProject.GetProjectInfo().Id)
				{
					_controllers.ProjectsController.Open(selectedProject);
				}

				_wizardContext.AnalysisBands = GetAnalysisBands(selectedProject);

				var projectInfo = selectedProject.GetProjectInfo();
				var selectedFileIds = controller is FilesController
					? _controllers.FilesController.SelectedFiles.Select(a => a.Id.ToString()).ToList()
					: null;

				_wizardContext.LocalProjectFolder = projectInfo.LocalProjectFolder;
				_wizardContext.TransactionFolder = _wizardContext.GetDefaultTransactionPath();

				var projectModel = GetProjectModel(selectedProject, selectedFileIds);
				_wizardContext.Project = projectModel;
				_wizardContext.ProjectFiles = projectModel.ProjectFiles;
			}
			else if (controller is XLIFFManagerViewController)
			{
				_wizardContext.Owner = Enumerators.Controller.XliffManager;

				var selectedProjectFiles = _controllers.XliffManagerController.GetSelectedProjectFiles();
				var selectedProjects = GetSelectedProjects();
				var selectedFileIds = selectedProjectFiles?.Select(a => a.FileId.ToString()).ToList();

				if (selectedProjects.Count == 0)
				{
					message = PluginResources.WizardMessage_NoProjectSelected;
					return false;
				}

				if (selectedProjects.Count > 1)
				{
					message = PluginResources.WizardMessage_MultipleProjectsSelected;
					return false;
				}

				var selectedProject = _controllers.ProjectsController.GetProjects()
					.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == selectedProjects[0].Id);
				if (selectedProject == null)
				{
					message = PluginResources.WizardMessage_UnableToLocateSelectedProject;
					return false;
				}

				_wizardContext.AnalysisBands = GetAnalysisBands(selectedProject);

				var projectInfo = selectedProject.GetProjectInfo();
				_wizardContext.LocalProjectFolder = projectInfo.LocalProjectFolder;
				_wizardContext.TransactionFolder = _wizardContext.GetDefaultTransactionPath();

				var projectModel = GetProjectModel(selectedProject, selectedFileIds);
				_wizardContext.Project = projectModel;
				_wizardContext.ProjectFiles = projectModel.ProjectFiles;
			}

			return true;
		}

		private void WizardWindowLoaded(object sender, RoutedEventArgs e)
		{
			_wizardWindow.Loaded -= WizardWindowLoaded;

			_pages = CreatePages(_wizardContext);
			AddDataTemplates(_wizardWindow, _pages);

			var viewModel = new WizardViewModel(_wizardWindow, _pages, _wizardContext, _action);
			viewModel.RequestClose += ViewModel_RequestClose;
			viewModel.RequestCancel += ViewModel_RequestCancel;
			_wizardWindow.DataContext = viewModel;
		}

		private Project GetProjectModel(FileBasedProject selectedProject, IReadOnlyCollection<string> selectedFileIds)
		{
			if (selectedProject == null)
			{
				return null;
			}

			var projectInfo = selectedProject.GetProjectInfo();

			var project = new Project
			{
				Id = projectInfo.Id.ToString(),
				Name = projectInfo.Name,
				AbsoluteUri = projectInfo.Uri.AbsoluteUri,
				Customer = _customerProvider.GetProjectCustomer(selectedProject),
				Created = projectInfo.CreatedAt.ToUniversalTime(),
				DueDate = projectInfo.DueDate?.ToUniversalTime() ?? DateTime.MaxValue,
				Path = projectInfo.LocalProjectFolder,
				SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo),
				TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages),
				ProjectType = GetProjectType(selectedProject)
			};

			var existingProject = _controllers.XliffManagerController.GetProjects().FirstOrDefault(a => a.Id == projectInfo.Id.ToString());
			if (existingProject != null)
			{
				foreach (var projectFile in existingProject.ProjectFiles)
				{
					if (projectFile.Clone() is ProjectFile clonedProjectFile)
					{
						clonedProjectFile.Project = project;
						clonedProjectFile.Location = GeFullPath(project.Path, clonedProjectFile.Location);
						clonedProjectFile.Report = GeFullPath(project.Path, clonedProjectFile.Report);
						clonedProjectFile.XliffFilePath = GeFullPath(project.Path, clonedProjectFile.XliffFilePath);
						clonedProjectFile.Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.FileId.ToString());
						project.ProjectFiles.Add(clonedProjectFile);
					}
				}
			}
			else
			{
				project.ProjectFiles = GetProjectFiles(selectedProject, project, selectedFileIds);
			}

			return project;
		}

		private string GeFullPath(string projectPath, string path)
		{
			if (string.IsNullOrEmpty(path?.Trim('\\')))
			{
				return string.Empty;
			}

			return Path.Combine(projectPath.Trim('\\'), path.Trim('\\'));
		}

		private List<ProjectFile> GetProjectFiles(IProject project, Project projectModel, IReadOnlyCollection<string> selectedFileIds)
		{
			var projectInfo = project.GetProjectInfo();
			var projectFiles = new List<ProjectFile>();

			foreach (var targetLanguage in projectInfo.TargetLanguages)
			{
				var languageFiles = project.GetTargetLanguageFiles(targetLanguage);
				foreach (var projectFile in languageFiles)
				{
					if (projectFile.Role != FileRole.Translatable)
					{
						continue;
					}

					var projectFileModel = GetProjectFile(projectModel, projectFile, targetLanguage, selectedFileIds);
					projectFiles.Add(projectFileModel);
				}
			}

			return projectFiles;
		}

		private ProjectFile GetProjectFile(Project project, ProjectAutomation.Core.ProjectFile projectFile,
			Language targetLanguage, IReadOnlyCollection<string> selectedFileIds)
		{
			var projectFileModel = new ProjectFile
			{
				ProjectId = project.Id,
				FileId = projectFile.Id.ToString(),
				Name = projectFile.Name,
				Path = projectFile.Folder,
				Location = projectFile.LocalFilePath,
				Action = Enumerators.Action.None,
				Status = Enumerators.Status.Ready,
				Date = DateTime.MinValue,
				TargetLanguage = targetLanguage.CultureInfo.Name,
				Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.Id.ToString()),
				FileType = projectFile.FileTypeId,
				Project = project
			};

			return projectFileModel;
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

		private List<AnalysisBand> GetAnalysisBands(FileBasedProject project)
		{
			var regex = new Regex(@"(?<min>[\d]*)([^\d]*)(?<max>[\d]*)", RegexOptions.IgnoreCase);

			var analysisBands = new List<AnalysisBand>();
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				foreach (var analysisBand in internalDynamicProject.AnalysisBands)
				{
					Match match = regex.Match(analysisBand.ToString());
					if (match.Success)
					{
						var min = match.Groups["min"].Value;
						var max = match.Groups["max"].Value;
						analysisBands.Add(new AnalysisBand
						{
							MinimumMatchValue = Convert.ToInt32(min),
							MaximumMatchValue = Convert.ToInt32(max)
						});
					}
				}
			}

			return analysisBands;
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

		private List<Project> GetSelectedProjects()
		{
			return _controllers.XliffManagerController.GetSelectedProjects();
		}

		private ObservableCollection<WizardPageViewModelBase> CreatePages(WizardContext wizardContext)
		{
			var pages = new List<WizardPageViewModelBase>();

			if (_action == Enumerators.Action.Export)
			{
				pages.Add(new WizardPageExportFilesViewModel(_wizardWindow, new WizardPageExportFilesView(), wizardContext));
				pages.Add(new WizardPageExportOptionsViewModel(_wizardWindow, new WizardPageExportOptionsView(), wizardContext, _dialogService));
				pages.Add(new WizardPageExportSummaryViewModel(_wizardWindow, new WizardPageExportSummaryView(), wizardContext));
				pages.Add(new WizardPageExportPreparationViewModel(_wizardWindow, new WizardPageExportPreparationView(), wizardContext,
					_segmentBuilder, _pathInfo));
			}
			else if (_action == Enumerators.Action.Import)
			{
				pages.Add(new WizardPageImportFilesViewModel(_wizardWindow, new WizardPageImportFilesView(), wizardContext, _dialogService, _languageProvider));
				pages.Add(new WizardPageImportOptionsViewModel(_wizardWindow, new WizardPageImportOptionsView(), wizardContext));
				pages.Add(new WizardPageImportSummaryViewModel(_wizardWindow, new WizardPageImportSummaryView(), wizardContext));
				pages.Add(new WizardPageImportPreparationViewModel(_wizardWindow, new WizardPageImportPreparationView(), wizardContext,
					_segmentBuilder, _pathInfo));
			}

			UpdatePageIndexes(pages);

			return new ObservableCollection<WizardPageViewModelBase>(pages);
		}

		private void UpdatePageIndexes(IReadOnlyList<WizardPageViewModelBase> pages)
		{
			for (var i = 0; i < pages.Count; i++)
			{
				pages[i].PageIndex = i + 1;
				pages[i].TotalPages = pages.Count;
			}
		}

		private void AddDataTemplates(FrameworkElement element, IEnumerable<WizardPageViewModelBase> pages)
		{
			foreach (var page in pages)
			{
				AddDataTemplate(element, page.GetType(), page.View.GetType());
			}
		}

		private void AddDataTemplate(FrameworkElement element, Type viewModelType, Type viewType)
		{
			var template = CreateTemplate(viewModelType, viewType);
			var key = template.DataTemplateKey;
			element.Resources.Add(key, template);
		}

		private DataTemplate CreateTemplate(Type viewModelType, Type viewType)
		{
			const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
			var xaml = string.Format(xamlTemplate, viewModelType.Name, viewType.Name);

			var context = new ParserContext { XamlTypeMapper = new XamlTypeMapper(new string[0]) };

			context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
			context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

			context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
			context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
			context.XmlnsDictionary.Add("vm", "vm");
			context.XmlnsDictionary.Add("v", "v");

			var template = (DataTemplate)XamlReader.Parse(xaml, context);
			return template;
		}

		private void ViewModel_RequestCancel(object sender, EventArgs e)
		{
			_isCancelled = true;
			CloseWizard();
		}

		private void ViewModel_RequestClose(object sender, EventArgs e)
		{
			CloseWizard();
		}

		private void CloseWizard()
		{
			if (_wizardWindow.DataContext is WizardViewModel viewModel)
			{
				viewModel.RequestCancel -= ViewModel_RequestCancel;
				viewModel.RequestClose -= ViewModel_RequestClose;
				viewModel.Dispose();
			}

			_wizardWindow.Close();
		}
	}
}
