using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Reports.Viewer.API.Example
{
	
	[RibbonGroup("ReportsViewerAPIExample_ActionsGroup", Name = "ReportsViewerAPIExample_ActionsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class ReportsViewerAPIExampleActionsGroup : AbstractRibbonGroup
	{
	}	
}
