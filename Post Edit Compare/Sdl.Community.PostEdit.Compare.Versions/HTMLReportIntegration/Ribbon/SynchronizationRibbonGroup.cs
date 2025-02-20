using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [RibbonGroup(nameof(SynchronizationRibbonGroup), "Synchronization")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class SynchronizationRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action(nameof(SyncReportProjectOff),
        typeof(ReportViewController),
        Icon = "StopSync", Name = "Synchronization Off",
        Description = "Stop synchronizing report with project")]
    [ActionLayout(typeof(SynchronizationRibbonGroup), 10, DisplayType.Large)]
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
            Integration.ToggleReportProjectSync(false);
        }
    }

    [Action(nameof(SyncReportProjectOn),
            typeof(ReportViewController),
        Icon = "StartSync", Name = "Synchronization On",
        Description = "Start synchronizing report with project")]
    [ActionLayout(typeof(SynchronizationRibbonGroup), 10, DisplayType.Large)]
    public class SyncReportProjectOn : AbstractAction
    {
        public SyncReportProjectOn()
        {
            Enabled = false;
        }

        protected override void Execute()
        {
            SdlTradosStudio.Application.GetAction<SyncReportProjectOff>().Enabled = true;
            Enabled = false;
            Integration.ToggleReportProjectSync(true);
        }
    }
}