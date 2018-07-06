using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using  Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.BetaAPIs.UI
{
	[RibbonGroup("Sdl.Community.StudioBetaAPIs", Name = "Studio Beta APIs")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class BetaApisRibbon: AbstractRibbonGroup
	{
	}
	[Action("Sdl.Community.StudioBetaAPIs", Icon = "", Name = "Available Beta APIs", Description = "Enable or disable Studio Beta APIs")]
	[ActionLayout(typeof(BetaApisRibbon), 20, DisplayType.Normal)]
	public class BetaApiViewPartAction : AbstractAction
	{
		protected override void Execute()
		{
			var window = new StudioBetaAPIsMainWindow();
			window.ShowDialog();
			
		}
	}
}
