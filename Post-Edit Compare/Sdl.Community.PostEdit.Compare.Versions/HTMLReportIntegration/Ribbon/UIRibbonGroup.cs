using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [Action(nameof(RefreshReportList),
        typeof(ReportViewController),
        Icon = "RefreshReportExplorer_Icon", Name = "Refresh Report List",
        Description = "Refresh Report List")]
    [ActionLayout(typeof(UIRibbonGroup), 10, DisplayType.Large)]
    public class RefreshReportList : AbstractAction
    {
        protected override void Execute() => Integration.SetUpReportExplorer();
    }

    [Action(nameof(UndockReportViewer),
        typeof(ReportViewController),
        Icon = "Undock", Name = "Undock Report Viewer",
        Description = "Undock Report Viewer")]
    [ActionLayout(typeof(UIRibbonGroup), 1, DisplayType.Large)]
    public class UndockReportViewer : AbstractAction
    {
        protected override void Execute()
        {
            Integration.UndockReportViewer();
            Enabled = false;
        }
    }
}