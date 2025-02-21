using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Legit
{
	[RibbonGroup("Sdl.Community.Legit", Name = "Trados LegIt!")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	class LegitRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.Legit", Name = "Trados LegIt!", Icon = "LegIt", Description = "Trados LegIt!")]
	[ActionLayout(typeof(LegitRibbon), 20, DisplayType.Large)]
	class LegitViewPartAction : AbstractAction
	{
		protected override void Execute()
		{
			var main = new MainForm();
			main.ShowDialog();
		}
	}

	[Action("Sdl.Community.Legit.Help", Name = "Help", Icon = "help_icon", Description = "Help")]
	[ActionLayout(typeof(LegitRibbon), 20, DisplayType.Large)]
	class HelpLinkAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://appstore.rws.com/Plugin/57?tab=documentation");
		}
	}
}