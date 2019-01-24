using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.UI;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ApplyTMTemplate
{
	[Action("ApplyTMTemplateAction", Icon = "ATTA", Name = "Apply TM Template", Description = "Applies settings from a TM template to a TM")]
	//[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
	[ActionLayout(typeof(ApplyTMTemplateRibbonGroup), 10, DisplayType.Large)]
	public class ApplyTMTemplateAction : AbstractAction
	{
		protected override void Execute()
		{
			var tmPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				@"Studio 2019\Translation Memories");

			var tmCollection = new ObservableCollection<TranslationMemory>();

			var dialog = new System.Windows.Forms.FolderBrowserDialog();

			//if (!Directory.Exists(tmPath))
			//{
			//	dialog.Description = "Please select your TM folder";
			//	dialog.ShowDialog();
			//	tmPath = dialog.SelectedPath;
			//}

			//foreach (var file in Directory.GetFiles(tmPath))
			//{
			//	tmCollection.Add(new TranslationMemory(new FileBasedTranslationMemory(file)));
			//}

			//var translationMemoryViewModel = new TranslationMemoryViewModel(tmCollection);

			var mainWindowViewModel = new MainWindowViewModel();

			var mainWindow = new MainWindow
			{
				DataContext = mainWindowViewModel
			};

			mainWindow.Show();
		}

		
	}
}
