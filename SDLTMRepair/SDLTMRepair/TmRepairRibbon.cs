using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.DefaultLocations;

namespace Sdl.Community.TMRepair
{
	[RibbonGroup("Sdl.Community.TMRepair", Name = "Trados Translation Memory Management Utility")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	class TmRepairRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.TMRepair", Name = "Trados Translation Memory Management Utility", Icon = "Repair", Description = "Trados Translation Memory Management Utility")]
	[ActionLayout(typeof(TmRepairRibbon), 20, DisplayType.Large)]
	class TmRepairViewPartAction : AbstractAction
	{
		protected override void Execute()
		{
			var tmRepair = new SDLTMRepair();
			tmRepair.ShowDialog();
		}
	}
}
