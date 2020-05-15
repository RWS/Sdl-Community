using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Sdl.Community.XLIFF.Manager.Wizard.View;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class WizardService
	{
		public static void ShowWizard()
		{
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();
			
			var translatableFilesCount = filesController.SelectedFiles.Count(projectFile => projectFile.Role == FileRole.Translatable);
			if (translatableFilesCount == 0)
			{
				MessageBox.Show("No translatable files selected!", PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var isMissingProjectFiles = IsMissingProjectFiles(filesController);
			if (isMissingProjectFiles)
			{
				MessageBox.Show("Some of the selected files could not be found locally. Connect to the server to get the latest version of these files.", PluginResources.Plugin_Name, MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var wizardView = new WizardView();
			var viewModel = new WizardViewModel(wizardView);
			
			wizardView.DataContext = viewModel;
			wizardView.ShowDialog();
		}


		public static ObservableCollection<WizardPageViewModelBase> CreatePages()
		{
			var pages = new ObservableCollection<WizardPageViewModelBase>
			{
				new WizardPageFilesViewModel( new WizardPageFilesView()),
				new WizardPageOptionsViewModel(new WizardPageOptionsView()),
				new WizardPageSummaryViewModel(new WizardPageSummaryView()),
				new WizardPagePreparationViewModel(new WizardPagePreparationView())				
			};

			UpdatePageIndexes(pages);

			return pages;
		}

		private static bool IsMissingProjectFiles(FilesController filesController)
		{
			var currentProject = filesController.CurrentProject;
			var projectInfo = currentProject.GetProjectInfo();
			var isGroupshareProject = !string.IsNullOrEmpty(projectInfo.ServerUri?.AbsoluteUri)
			                          && projectInfo.PublicationStatus == PublicationStatus.Published;

			return isGroupshareProject
			       && filesController.SelectedFiles.Where(projectFile => projectFile.Role == FileRole.Translatable)
				       .Any(file => file.LocalFileState == LocalFileState.Missing);
		}

		private static void UpdatePageIndexes(IReadOnlyList<WizardPageViewModelBase> pages)
		{
			for (var i = 0; i < pages.Count; i++)
			{
				pages[i].PageIndex = i + 1;
				pages[i].TotalPages = pages.Count;
			}
		}

		public static void AddDataTemplates(FrameworkElement element, IEnumerable<WizardPageViewModelBase> pages)
		{
			foreach (var page in pages)
			{
				AddDataTemplate(element, page.GetType(), page.View.GetType());
			}
		}

		private static void AddDataTemplate(FrameworkElement element, Type viewModelType, Type viewType)
		{
			var template = CreateTemplate(viewModelType, viewType);
			var key = template.DataTemplateKey;
			element.Resources.Add(key, template);
		}

		private static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
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
	}
}
