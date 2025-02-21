using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Reports.Viewer.Plus
{
	[RibbonGroup("ReportsViewer_SettingsGroup", Name = "ReportsViewer_SettingsGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class ReportsViewerSettingsGroups : AbstractRibbonGroup
	{
	}

	[RibbonGroup("ReportsViewer_ReportGroup", Name = "ReportsViewer_ReportGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class ReportsViewerReportGroups : AbstractRibbonGroup
	{
	}

	[RibbonGroup("ReportsViewer_ViewGroup", Name = "ReportsViewer_ViewGroup_Name")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class ReportsViewerViewGroups : AbstractRibbonGroup
	{
	}
}
