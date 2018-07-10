using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Legit
{
	[RibbonGroup("Sdl.Community.Legit", Name = "LegIt!")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	class LegitRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.Legit", Name = "LegIt!", Icon = "LegIt", Description = "LegIt!")]
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
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3281.legit");
		}
	}
}