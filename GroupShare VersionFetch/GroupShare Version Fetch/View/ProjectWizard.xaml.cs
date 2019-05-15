using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using Sdl.Community.GSVersionFetch.ViewModel;

namespace Sdl.Community.GSVersionFetch.View
{
	/// <summary>
	/// Interaction logic for ProjectWizard.xaml
	/// </summary>
	public partial class ProjectWizard : IDisposable
	{
		private readonly ProjectWizardViewModel _model;

		public ProjectWizard(ObservableCollection<ProjectWizardViewModelBase> pages)
		{
			InitializeComponent();

			UpdatePageIndexes(pages);
			AddDataTemplates(this, pages);

			_model = new ProjectWizardViewModel(this, pages);
			_model.RequestClose += ProjectWizardViewModel_RequestClose;

			DataContext = _model;
		}

		private static void UpdatePageIndexes(IReadOnlyList<ProjectWizardViewModelBase> pages)
		{
			for (var i = 0; i < pages.Count; i++)
			{
				pages[i].PageIndex = i + 1;
				pages[i].TotalPages = pages.Count;
			}
		}

		public static void AddDataTemplates(FrameworkElement element, IEnumerable<ProjectWizardViewModelBase> pages)
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
			if (_model != null)
			{
				_model.RequestClose -= ProjectWizardViewModel_RequestClose;
			}
		}
	}
}
