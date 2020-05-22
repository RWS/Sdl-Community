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
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Size = System.Drawing.Size;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class WizardService
	{
		private readonly Enumerators.Action _action;
		private readonly CustomerProvider _customerProvider;
		private readonly ProjectsController _projectsController;
		private readonly FilesController _filesController;
		private readonly XLIFFManagerViewController _xliffManagerController;
		private readonly ImageService _imageService;
		private WizardWindow _wizardWindow;
		private ObservableCollection<WizardPageViewModelBase> _pages;
		private WizardContextModel _wizardContextModel;

		public WizardService(Enumerators.Action action, CustomerProvider customerProvider, ImageService imageService, XLIFFManagerViewController xliffManagerController, ProjectsController projectsController, FilesController filesController)
		{
			_action = action;
			_customerProvider = customerProvider;
			_imageService = imageService;
			_xliffManagerController = xliffManagerController;
			_projectsController = projectsController;
			_filesController = filesController;
		}

		public bool ShowWizard(AbstractController owner, out string message)
		{
			message = string.Empty;

			if (_action == Enumerators.Action.None)
			{
				message = "no action selected";
				return false;
			}

			var projectFileModels = new List<ProjectFileModel>();
			
			_wizardContextModel = new WizardContextModel();
			if (owner is ProjectsController || owner is FilesController)
			{
				var selectedProject = _projectsController.CurrentProject;
				var projectInfo = selectedProject.GetProjectInfo();

				var projectModel = new ProjectModel();
				projectModel.Id = projectInfo.Id.ToString();
				projectModel.Name = projectInfo.Name;
				projectModel.AbsoluteUri = projectInfo.Uri.AbsoluteUri;
				projectModel.Customer = _customerProvider.GetProjectCustomer(selectedProject);
				projectModel.Created = projectInfo.CreatedAt;
				projectModel.DueDate = projectInfo.DueDate ?? DateTime.MaxValue;
				projectModel.Path = projectInfo.LocalProjectFolder;
				projectModel.SourceLanguage = GetLanguageInfo(projectInfo.SourceLanguage.CultureInfo);
				projectModel.TargetLanguages = GetLanguageInfos(projectInfo.TargetLanguages);
				projectModel.ProjectType = GetProjectType(selectedProject);
				projectModel.ProjectFileModels = GetProjectFileModels(selectedProject, projectModel);

				projectFileModels = projectModel.ProjectFileModels;
			}
			else if (owner is XLIFFManagerViewController)
			{
				projectFileModels = _xliffManagerController.GetSelectedProjectFiles();
				var selectedProjects = GetSelectedProjects(projectFileModels);

				if (selectedProjects.Count > 1)
				{
					message = "Multiple projects selected!";
					return false;
				}
			}

			_wizardContextModel.ProjectFileModels = projectFileModels;
			_wizardWindow = new WizardWindow();
			_wizardWindow.Loaded += WizardWindowLoaded;

			var result = _wizardWindow.ShowDialog();

			return result ?? false;
		}

		private void WizardWindowLoaded(object sender, RoutedEventArgs e)
		{
			_pages = CreatePages(_wizardContextModel);
			AddDataTemplates(_wizardWindow, _pages);

			var viewModel = new WizardViewModel(_wizardWindow, _pages, _wizardContextModel, _action);
			viewModel.RequestClose += ViewModel_RequestClose;
			_wizardWindow.DataContext = viewModel;
		}

		private List<ProjectFileModel> GetProjectFileModels(FileBasedProject project, ProjectModel projectModel)
		{
			var projectInfo = project.GetProjectInfo();
			var selectedFiles = _filesController.SelectedFiles.ToList();

			var projectFileModels = new List<ProjectFileModel>();

			foreach (var targetLanguage in projectInfo.TargetLanguages)
			{
				var projectFiles = project.GetTargetLanguageFiles(targetLanguage);
				foreach (var projectFile in projectFiles)
				{
					if (projectFile.Role == FileRole.Translatable)
					{
						var projectFileModel = new ProjectFileModel(projectModel);
						projectFileModel.Id = projectFile.Id.ToString();
						projectFileModel.Name = projectFile.Name;
						projectFileModel.Path = projectFile.Folder;
						projectFileModel.Action = Enumerators.Action.Export;
						projectFileModel.Status = Enumerators.Status.Ready;
						projectFileModel.Date = DateTime.Now;
						projectFileModel.TargetLanguage = GetLanguageInfo(targetLanguage.CultureInfo);
						projectFileModel.Selected = selectedFiles.Any(a => a.Id == projectFile.Id);
						projectFileModel.XliffFilePath = string.Empty;
						projectFileModel.FileType = projectFile.FileTypeId;

						projectFileModels.Add(projectFileModel);
					}
				}
			}

			projectFileModels[0].Status = Enumerators.Status.Warning;
			projectFileModels[0].ShortMessage = "File was exported on date: {datetime}";

			return projectFileModels;
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

		public string GetProjectType(FileBasedProject project)
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

		private List<ProjectModel> GetSelectedProjects(IEnumerable<ProjectFileModel> selectedFiles)
		{
			var selectedProjects = new List<ProjectModel>();
			foreach (var selectedFile in selectedFiles)
			{
				if (!selectedProjects.Contains(selectedFile.ProjectModel))
				{
					selectedProjects.Add(selectedFile.ProjectModel);
				}
			}

			return selectedProjects;
		}



		private ObservableCollection<WizardPageViewModelBase> CreatePages(WizardContextModel transactionModel)
		{
			//TODO setup the Import action pages

			var pages = new ObservableCollection<WizardPageViewModelBase>
			{
				new WizardPageExportFilesViewModel(_wizardWindow, new WizardPageExportFilesView(), transactionModel),
				new WizardPageExportOptionsViewModel(_wizardWindow, new WizardPageExportOptionsView(), transactionModel),
				new WizardPageSummaryViewModel(_wizardWindow, new WizardPageSummaryView(), transactionModel),
				new WizardPagePreparationViewModel(_wizardWindow, new WizardPagePreparationView(), transactionModel)
			};

			UpdatePageIndexes(pages);

			return pages;
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

		private void ViewModel_RequestClose(object sender, System.EventArgs e)
		{
			if (_wizardWindow.DataContext is WizardViewModel viewModel)
			{
				viewModel.RequestClose -= ViewModel_RequestClose;
				viewModel.Dispose();
			}

			_wizardWindow.Close();
		}
	}
}
