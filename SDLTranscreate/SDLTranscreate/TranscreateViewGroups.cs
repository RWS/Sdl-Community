using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Trados.Transcreate
{
	[RibbonGroup("TranscreateManager_SettingsGroup", Name = "TranscreateManager_SettingsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TranscreateManagerSettingsGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TranscreateManager_ActionsGroup", Name = "TranscreateManager_ActionsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TranscreateManagerActionsGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TranscreateManager_OpenGroup", Name = "TranscreateManager_OpenGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TranscreateManagerOpenGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TranscreateManager_ViewGroup", Name = "TranscreateManager_ViewGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TranscreateManagerViewGroup : AbstractRibbonGroup
	{
	}
}
