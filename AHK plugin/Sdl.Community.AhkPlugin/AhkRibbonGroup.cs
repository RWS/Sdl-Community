using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.DataBase;
using Sdl.Community.AhkPlugin.Repository.Raven;
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
		/// <summary>
		/// Create raven context when Studio starts, in this way when we click on rebbon AHK window appears immediately
		/// </summary>
		public void Execute()
		{
			RavenContext.Current.CreateSession();
			var masterScriptDb = new MasterScriptDb();
			var masterScript = masterScriptDb.GetMasterScript().Result;
			if (masterScript == null)
			{
				var master = new MasterScript
				{
					ScriptId = Guid.NewGuid().ToString(),
					Name = "AhkMasterScript.ahk",
					Location = GetDefaultPath()
				};
				//write empty ahk script on disk
				ProcessScript.ExportScript(Path.Combine(master.Location,master.Name),new List<Script>());
				//add master to db
				masterScriptDb.CreateMasterScript(master);
			}
		}

		private string GetDefaultPath()
		{
			var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"SDL Community", "AhkMasterScript");
			if (!Directory.Exists(defaultPath))
			{
				Directory.CreateDirectory(defaultPath);
			}
			return defaultPath;
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
			var scriptDb = new ScriptDb();
			var list = scriptDb.GetAllScripts().Result;
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
