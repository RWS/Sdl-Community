using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using Sdl.Community.XLIFF.Manager.Common;
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
using ProjectFile = Sdl.Community.XLIFF.Manager.Model.ProjectFile;
using Size = System.Drawing.Size;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class WizardService
	{
		private readonly Enumerators.Action _action;
		private readonly PathInfo _pathInfo;
		private readonly CustomerProvider _customerProvider;
		private readonly ProjectsController _projectsController;
		private readonly FilesController _filesController;
		private readonly XLIFFManagerViewController _xliffManagerController;
		private readonly ImageService _imageService;
		private WizardWindow _wizardWindow;
		private ObservableCollection<WizardPageViewModelBase> _pages;
		private WizardContext _wizardContext;
		private bool _isCancelled;

		public WizardService(Enumerators.Action action, PathInfo pathInfo, CustomerProvider customerProvider,
			ImageService imageService, XLIFFManagerViewController xliffManagerController,
			ProjectsController projectsController, FilesController filesController)
		{
			_action = action;
			_pathInfo = pathInfo;
			_customerProvider = customerProvider;
			_imageService = imageService;
			_xliffManagerController = xliffManagerController;
			_projectsController = projectsController;
			_filesController = filesController;
		}

		public WizardContext ShowWizard(AbstractController controller, out string message)
		{
			message = string.Empty;

			if (_action == Enumerators.Action.None)
			{
				message = "no action selected";
				return null;
			}

			var success = CreateWizardContext(controller, out message);
			if (!success)
			{
				return null;
			}

			_isCancelled = false;
			_wizardWindow = new WizardWindow();
			_wizardWindow.Loaded += WizardWindowLoaded;
			_wizardWindow.ShowDialog();

			return (!_isCancelled && _wizardContext.Completed) ? _wizardContext : null;
		}

		private bool CreateWizardContext(AbstractController controller, out string message)
		{
			_wizardContext = new WizardContext();
			message = string.Empty;

			if (controller is ProjectsController || controller is FilesController)
			{
				var selectedProject = _projectsController.CurrentProject;
				var projectInfo = selectedProject.GetProjectInfo();
				var selectedFileIds = controller is FilesController
					? _filesController.SelectedFiles.Select(a => a.Id.ToString()).ToList()
					: null;

				_wizardContext.ProjectFolder = projectInfo.LocalProjectFolder;
				_wizardContext.Support = Enumerators.XLIFFSupport.xliff12polyglot;
				_wizardContext.OutputFolder = _pathInfo.GetProjectOutputPath(projectInfo);

				var projectModel = GetProjectModel(selectedProject, selectedFileIds);
				_wizardContext.Project = projectModel;
				_wizardContext.ProjectFiles = projectModel.ProjectFiles;
			}
			else if (controller is XLIFFManagerViewController)
			{
				var selectedProjectFiles = _xliffManagerController.GetSelectedProjectFiles();
				var selectedProjects = GetSelectedProjects(selectedProjectFiles);
				var selectedFileIds = selectedProjectFiles?.Select(a => a.Id.ToString()).ToList();

				if (selectedProjects.Count == 0)
				{
					message = "No project selected!";
					return false;
				}

				if (selectedProjects.Count > 1)
				{
					message = "Multiple projects selected!";
					return false;
				}

				var selectedProject = _projectsController.GetProjects()
					.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == selectedProjects[0].Id);
				if (selectedProject == null)
				{
					message = "Unable to locate the selected project!";
					return false;
				}

				var projectInfo = selectedProject.GetProjectInfo();

				_wizardContext.ProjectFolder = projectInfo.LocalProjectFolder;
				_wizardContext.Support = Enumerators.XLIFFSupport.xliff12polyglot;
				_wizardContext.OutputFolder = _pathInfo.GetProjectOutputPath(projectInfo);

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

			var projectModel = new Project
			{
				Id = projectInfo.Id.ToString(),
				Name = projectInfo.Name,
				AbsoluteUri = projectInfo.Uri.AbsoluteUri,
				Customer = _customerProvider.GetProjectCustomer(selectedProject),
				Created = projectInfo.CreatedAt,
				DueDate = projectInfo.DueDate ?? DateTime.MaxValue,
				Path = projectInfo.LocalProjectFolder,
				SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo),
				TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages),
				ProjectType = GetProjectType(selectedProject)
			};

			var existingProject = _xliffManagerController.GetProjects().FirstOrDefault(a => a.Id == projectInfo.Id.ToString());

			if (existingProject != null)
			{
				projectModel.ProjectFiles = existingProject.ProjectFiles;
				foreach (var projectFile in projectModel.ProjectFiles)
				{
					projectFile.Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.Id.ToString());
				}
			}
			else
			{
				projectModel.ProjectFiles = GetProjectFiles(selectedProject, projectModel, selectedFileIds);
			}
				
			return projectModel;
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

					var projectFileModel = GetProjectFileModel(projectModel, projectFile, targetLanguage, selectedFileIds);
					projectFiles.Add(projectFileModel);
				}
			}

			return projectFiles;
		}

		private ProjectFile GetProjectFileModel(Project project, ProjectAutomation.Core.ProjectFile projectFile,
			Language targetLanguage, IReadOnlyCollection<string> selectedFileIds)
		{
			var projectFileModel = new ProjectFile
			{
				ProjectId = project.Id,
				Id = projectFile.Id.ToString(),
				Name = projectFile.Name,
				Path = projectFile.Folder,
				Location = projectFile.LocalFilePath,
				Action = Enumerators.Action.None,
				Status = Enumerators.Status.Ready,
				Date = DateTime.MinValue,
				TargetLanguage = GetLanguageInfo(targetLanguage.CultureInfo),
				Selected = selectedFileIds != null && selectedFileIds.Any(a => a == projectFile.Id.ToString()),
				FileType = projectFile.FileTypeId,
				ProjectModel = project
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

		private LanguageInfo GetLanguageInfo(CultureInfo cultureInfo)
		{
			var sourceLanguageInfo = new LanguageInfo
			{
				CultureInfo = cultureInfo,
				ImageName = cultureInfo.Name + ".ico"
			};
			sourceLanguageInfo.Image = _imageService.GetImage(sourceLanguageInfo.ImageName, new Size(24, 24));

			return sourceLanguageInfo;
		}

		private List<Project> GetSelectedProjects(IEnumerable<ProjectFile> selectedFiles)
		{
			var selectedProjects = new List<Project>();

			if (selectedFiles == null)
			{
				return selectedProjects;
			}

			foreach (var selectedFile in selectedFiles)
			{
				if (!selectedProjects.Contains(selectedFile.ProjectModel))
				{
					selectedProjects.Add(selectedFile.ProjectModel);
				}
			}

			return selectedProjects;
		}

		private ObservableCollection<WizardPageViewModelBase> CreatePages(WizardContext wizardContext)
		{
			var pages = new List<WizardPageViewModelBase>();

			if (_action == Enumerators.Action.Export)
			{
				pages.Add(new WizardPageExportFilesViewModel(_wizardWindow, new WizardPageExportFilesView(), wizardContext));
				pages.Add(new WizardPageExportOptionsViewModel(_wizardWindow, new WizardPageExportOptionsView(), wizardContext));
				pages.Add(new WizardPageExportSummaryViewModel(_wizardWindow, new WizardPageExportSummaryView(), wizardContext));
				pages.Add(new WizardPageExportPreparationViewModel(_wizardWindow, new WizardPageExportPreparationView(), wizardContext));
			}
			else if (_action == Enumerators.Action.Import)
			{
				pages.Add(new WizardPageImportFilesViewModel(_wizardWindow, new WizardPageImportFilesView(), wizardContext));
				// TODO: (Andrea)
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

		private void ViewModel_RequestCancel(object sender, System.EventArgs e)
		{
			_isCancelled = true;
			CloseWizard();
		}

		private void ViewModel_RequestClose(object sender, System.EventArgs e)
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
