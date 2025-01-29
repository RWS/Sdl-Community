using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [RibbonGroup("PostEditCompareReportRibbonGroup", "Synchronization")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class PostEditCompareReportRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action(nameof(SyncReportProjectOff),
        typeof(ReportViewController),
        Icon = "StopSync", Name = "Sync Off",
        Description = "Stop synchronizing report with project")]
    [ActionLayout(typeof(PostEditCompareReportRibbonGroup), 10, DisplayType.Large)]
    public class SyncReportProjectOff : AbstractAction
    {
        public SyncReportProjectOff()
        {
            Enabled = false;
        }

        protected override void Execute()
        {
            SdlTradosStudio.Application.GetAction<SyncReportProjectOn>().Enabled = true;
            Enabled = false;
            Integration.ToggleSync(false);
        }
    }

    [Action(nameof(SyncReportProjectOn),
            typeof(ReportViewController),
        Icon = "StartSync", Name = "Sync On",
        Description = "Start synchronizing report with project")]
    [ActionLayout(typeof(PostEditCompareReportRibbonGroup), 10, DisplayType.Large)]
    public class SyncReportProjectOn : AbstractAction
    {
        protected override void Execute()
        {
            SdlTradosStudio.Application.GetAction<SyncReportProjectOff>().Enabled = true;
            Enabled = false;
            Integration.ToggleSync(true);
        }
    }
}