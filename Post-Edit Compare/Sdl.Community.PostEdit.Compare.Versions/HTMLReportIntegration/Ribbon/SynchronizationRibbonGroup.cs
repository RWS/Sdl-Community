using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using System;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [Action(nameof(SyncReportProjectOn),
            typeof(ReportViewController),
        Icon = "Sync", Name = "Synchronize",
        Description = "Toggle synchronizing report with project")]
    [ActionLayout(typeof(SynchronizationRibbonGroup), 10, DisplayType.Large)]
    public class SyncReportProjectOn : AbstractAction
    {
        public SyncReportProjectOn() => Style = ActionStyle.ToggleButton;

        protected override void Execute() => Integration.ToggleReportProjectSync(Checked);
    }
}