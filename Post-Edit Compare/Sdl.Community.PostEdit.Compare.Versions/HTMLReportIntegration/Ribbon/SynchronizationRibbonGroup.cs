using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [RibbonGroup(nameof(SynchronizationRibbonGroup), "Synchronization")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class SynchronizationRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action(nameof(SyncReportProjectOn),
            typeof(ReportViewController),
        Icon = "Sync", Name = "Synchronize",
        Description = "Toggle synchronizing report with project")]
    [ActionLayout(typeof(SynchronizationRibbonGroup), 10, DisplayType.Large)]
    public class SyncReportProjectOn : AbstractAction
    {
        public SyncReportProjectOn()
        {
            Style = ActionStyle.ToggleButton;
        }

        protected override void Execute() => Integration.ToggleReportProjectSync(Checked);
    }
}