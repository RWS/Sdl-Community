using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Ui;
using Sdl.Community.AhkPlugin.ViewModels;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AhkPlugin
{
	[ApplicationInitializer]
	public class InitializePlugin : IApplicationInitializer
	{
		public void Execute()
		{
			// Open database (or create if not exits)
			var defaultFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"SDL Community", "AhkMasterScript");
			if (!Directory.Exists(defaultFolderPath))
			{
				Directory.CreateDirectory(defaultFolderPath);
			}
			var defaultDbPath = Path.Combine(defaultFolderPath, "Ahk.db");

			using (var db = new LiteDatabase(defaultDbPath))
			{
				var masterScriptCollection = db.GetCollection<MasterScript>("masterScript");
				var masterScript = masterScriptCollection.FindOne(m => m.Name.Contains("AhkMasterScript.ahk"));
				if (masterScript != null) return;

				//create master script if does not exist
				var master = new MasterScript
				{
					Id = 1,
					Name = "AhkMasterScript.ahk",
					Location = defaultFolderPath,
					Scripts = new List<Script>()
				};
				masterScriptCollection.Insert(master);
			}
		}
	}

	[RibbonGroup("Sdl.Community.AhkRibbonGroup", Name = "Ahk", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class AhkRibbonGroup: AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.AhkAction", Name = "AutoHotKey scripts", Icon = "icon", Description = "Manage HotKey Scripts")]
	[ActionLayout(typeof(AhkRibbonGroup), 250, DisplayType.Large)]
	public class AhkAction : AbstractAction
	{
		protected override void Execute()
		{
			var mainVindowViewModel = new MainWindowViewModel();
			var mainWindow = new MainWindow
			{
				DataContext = mainVindowViewModel
			};
			mainWindow.ShowDialog();
		}
	}
	[Action("Sdl.Community.HelpAhkAction", Name = "AutoHotKey scripts help", Icon = "question", Description = "An wiki page will be opened in browser uith user documentation")]
	[ActionLayout(typeof(AhkRibbonGroup), 250, DisplayType.Large)]
	public class AhkHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3171.ahk-plugin");

		}
	}

}
