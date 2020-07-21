using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.Transcreate
{
	[RibbonGroup("TranscreateManager_SettingsGroup", Name = "TranscreateManager_SettingsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TranscreateManagerSettingsGroup : AbstractRibbonGroup
	{
	}

	[RibbonGroup("TranscreateFManager_ActionsGroup", Name = "TranscreateManager_ActionsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TranscreateManagerActionsGroup : AbstractRibbonGroup
	{
	}
}
