using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Wizard.View;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class WizardService
	{
		private readonly Enumerators.Action _action;

		private WizardWindow _wizardWindow;
		private ObservableCollection<WizardPageViewModelBase> _pages;		

		public WizardService(Enumerators.Action action)
		{
			_action = action;
		}

		public void ShowWizard()
		{			
			if (_action == Enumerators.Action.None)
			{
				return;
			}

			_pages = CreatePages();

			_wizardWindow = new WizardWindow();							
			_wizardWindow.Loaded += WizardWindowLoaded;			
			_wizardWindow.ShowDialog();
		}
		
		private void WizardWindowLoaded(object sender, RoutedEventArgs e)
		{
			AddDataTemplates(_wizardWindow, _pages);

			var viewModel = new WizardViewModel(_wizardWindow, _action, _pages);
			viewModel.RequestClose += ViewModel_RequestClose;
			_wizardWindow.DataContext = viewModel;
		}		

		private ObservableCollection<WizardPageViewModelBase> CreatePages()
		{
			//TODO setup the Import action pages

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
