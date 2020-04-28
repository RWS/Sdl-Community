using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.RapidAddTerm
{
	[RibbonGroup("Sdl.Community.RapidAddTermRibbonGroup", Name = "Rapid Add Term", Description = "Rapid Add Term", ContextByType = typeof(EditorController))]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class RapidAddTermRibbonGroup : AbstractRibbonGroup
	{
	}
	[Action("Sdl.Community.RapidAddTerm.Action", Name = "RAT", Icon = "RAT1", Description = "Rapid Add Term")]
	[ActionLayout(typeof(RapidAddTermRibbonGroup), 250, DisplayType.Large)]
	public class RapidAddTermRibbonAction: AbstractAction
	{
		private readonly TermbaseService _termbaseService = new TermbaseService();
		protected override void Execute()
		{
			_termbaseService.AddNewTerm();
		}
	}
}
