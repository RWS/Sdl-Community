using Sdl.Community.IATETerminologyProvider.Helpers;
using TradosStudio.API.UI;

namespace Sdl.Community.IATETerminologyProvider.Actions
{
	public class IATETerminologyProviderRibbonGroup: IRibbonGroup
    {
        public string Id => Constants.IATETerminologyProviderRibbonGroupId;

        public string DisplayName => PluginResources.IATETerminologyProviderRibbonGroup_Name;

        public string TargetSiteId => "EditorView";

        public TargetSiteType TargetSiteType => TargetSiteType.View;

        public RibbonTabs Locations => RibbonTabs.HomeTab;

	}
}