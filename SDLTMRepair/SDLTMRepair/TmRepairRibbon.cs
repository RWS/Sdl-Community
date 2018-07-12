using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.DefaultLocations;

namespace Sdl.Community.TMRepair
{
	[RibbonGroup("Sdl.Community.TMRepair", Name = "TMRepair")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	class TmRepairRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.TMRepair", Name = "TMRepair", Icon = "Repair", Description = "TMRepair")]
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
