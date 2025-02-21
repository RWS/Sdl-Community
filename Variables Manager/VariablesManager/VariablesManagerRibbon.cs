using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace VariablesManager
{
	[RibbonGroup("Sdl.Community.VariablesManager", Name = "Variables Manager")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	class VariablesManagerRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.VariablesManager", Name = "Variables Manager", Icon = "variables_manager___128", Description = "Variables Manager")]
	[ActionLayout(typeof(VariablesManagerRibbon), 20, DisplayType.Large)]
	class VariablesManagerViewPartAction : AbstractAction
	{
		protected override void Execute()
		{
			var main = new Form1();
			main.ShowDialog();
		}
	}
}
