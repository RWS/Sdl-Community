using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioMigrationUtility.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.StudioMigrationUtility
{
	[RibbonGroup("Sdl.Community.StudioMigrationUtility", Name = "Studio Migration Utility")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	class StudioMigrationUtilityRibbon : AbstractRibbonGroup
	{
	}


	[Action("Sdl.Community.StudioMigrationUtility", Name = "Studio Migration Utility", Icon = "migrate", Description = "Studio Migration Utility")]
	[ActionLayout(typeof(StudioMigrationUtilityRibbon), 20, DisplayType.Large)]
	class StudioMigrationUtilityViewPartAction : AbstractAction
	{
		protected override void Execute()
		{

			var migrateUtility = new MigrateUtility(new StudioVersionService());
			migrateUtility.ShowDialog();

		}
	}
	[Action("Sdl.Community.StudioMigrationUtilityHelp", Name = "Migration Utility help", Icon = "question", Description = "An wiki page will be opened in browser uith user documentation")]
	[ActionLayout(typeof(StudioMigrationUtilityRibbon), 250, DisplayType.Large)]
	public class AhkHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3265.studio-migration-utility");

		}
	}
}
