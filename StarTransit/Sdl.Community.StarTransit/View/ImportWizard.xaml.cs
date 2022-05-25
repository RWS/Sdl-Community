using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using Sdl.Community.StarTransit.Helpers;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.ViewModel;

namespace Sdl.Community.StarTransit.View
{
	/// <summary>
	/// Interaction logic for ImportWizard.xaml
	/// </summary>
	public partial class ImportWizard:IDisposable
	{
		private readonly WizardViewModel _model;
		private readonly IProjectsControllerService _projectControllerService;
		private readonly IDisposable _projectCreatedEvent;

		public ImportWizard(ObservableCollection<IProgressHeaderItem> pages,IEventAggregatorService eventAggregatorService, IProjectsControllerService projectsControllerService)
		{
			InitializeComponent();
			UpdatePageIndexes(pages);
			AddDataTemplates(this, pages);

			_model = new WizardViewModel(this, pages,eventAggregatorService);
			_model.SelectedPageChanged += Model_SelectedPageChanged;
			_model.RequestClose += ProjectWizardViewModel_RequestClose;
			_projectControllerService = projectsControllerService;
			_projectCreatedEvent=eventAggregatorService.Subscribe<ProjectCreated>(OnProjectCreated);

			DataContext = _model;
		}

		private void OnProjectCreated(ProjectCreated studioProject)
		{
			if (studioProject != null)
			{
				_projectControllerService.OpenProjectInFilesView(studioProject.CreatedProject);
			}
			Close();
		}

		private void Model_SelectedPageChanged(object sender, SelectedPageEventArgs e)
		{
			if (_model.CurrentPagePosition == e.PagePosition)
			{
				return;
			}

			if (_model.CurrentPage.IsLastPage && _model.CurrentPage.IsComplete)
			{
				return;
			}

			if (!_model.CanMoveToPage(e.PagePosition, out var message))
			{
				if (!string.IsNullOrEmpty(message))
				{
					MessageBox.Show(message, _model.WindowTitle, MessageBoxButton.OK, MessageBoxImage.Information);
				}

				return;
			}

			_model?.SetCurrentPage(e.PagePosition);
		}

		private static void UpdatePageIndexes(IReadOnlyList<IProgressHeaderItem> pages)
		{
			for (var i = 0; i < pages.Count; i++)
			{
				pages[i].PageIndex = i + 1;
				pages[i].TotalPages = pages.Count;
			}
		}

		public static void AddDataTemplates(FrameworkElement element, IEnumerable<IProgressHeaderItem> pages)
		{
			foreach (var page in pages)
			{
				AddDataTemplate(element, page.GetType(), ((WizardViewModelBase)page).View.GetType());
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
			var xaml = string.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

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

		private void ProjectWizardViewModel_RequestClose(object sender, EventArgs e)
		{
			Close();
		}

		public void Dispose()
		{
			_projectCreatedEvent?.Dispose();
			if (_model != null)
			{
				_model.RequestClose -= ProjectWizardViewModel_RequestClose;
				foreach (var page in _model.Pages)
				{
					page?.Dispose();
				}
			}
		}
	}
}
