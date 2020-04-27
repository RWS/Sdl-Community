using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.XLIFF.Manager
{
	[RibbonGroup("XLIFFManagerSettingsGroup", "Configuration")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class XLIFFManagerSettingsGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("XLIFFManagerActionsGroup", "Actions")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class XLIFFManagerActionsGroup : AbstractRibbonGroup
	{
	}
}
