using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [RibbonGroup(nameof(UIRibbonGroup), "UI")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class UIRibbonGroup : AbstractRibbonGroup
    {
    }

    [RibbonGroup(nameof(ReportManagementRibbonGroup), "Report Management")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class ReportManagementRibbonGroup : AbstractRibbonGroup
    {
    }

    [RibbonGroup(nameof(SynchronizationRibbonGroup), "Synchronization")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class SynchronizationRibbonGroup : AbstractRibbonGroup
    {
    }

}