using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.XLIFF.Manager
{
	[RibbonGroup("XLIFFManager_SettingsGroup", Name = "XLIFFManager_SettingsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class XLIFFManagerSettingsGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("XLIFFManager_ActionsGroup", Name = "XLIFFManager_ActionsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class XLIFFManagerActionsGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("XLIFFManager_ViewGroup", Name = "XLIFFManager_ViewGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class XLIFFManagerViewGroup : AbstractRibbonGroup
	{
	}
}
