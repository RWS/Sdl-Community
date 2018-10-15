using System;
using Sdl.Community.HunspellDictionaryManager.Helpers;
using Sdl.Community.HunspellDictionaryManager.Ui;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.HunspellDictionaryManager.Studio
{
	[RibbonGroup("Hunspell Dictionary Manager", Name = "Hunspell Dictionary Manager")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class HunspellDictionaryManagerRibbon : AbstractRibbonGroup
	{
		[Action("HunspellDictionaryManager.HunspellDictionaryManager", Name = "Hunspell Dictionary Manager", Icon = "h_icon", Description = "Hunspell Dictionary Manager")]
		[ActionLayout(typeof(HunspellDictionaryManagerRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
		public class HunspellDictionaryManagerAction : AbstractAction
		{
			protected override void Execute()
			{
				Utils.InitializeWpfApplicationSettings();

				var mainWindow = new MainWindow();
				mainWindow.Show();
			}
		}
	}
}