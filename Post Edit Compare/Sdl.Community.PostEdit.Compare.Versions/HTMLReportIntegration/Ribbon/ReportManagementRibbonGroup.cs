using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [RibbonGroup(nameof(ReportManagementRibbonGroup), "Report Management")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class ReportManagementRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action(nameof(SaveReport),
        typeof(ReportViewController),
        Icon = "SaveReport", Name = "Save Report",
        Description = "Save Report")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Large)]
    public class SaveReport : AbstractAction
    {
        protected override void Execute() => Integration.SaveReport();
    }

    [Action(nameof(RefreshReportList),
        typeof(ReportViewController),
        Icon = "RefreshReportList", Name = "Refresh Report List",
        Description = "Refresh Report List")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class RefreshReportList : AbstractAction
    {
        protected override void Execute() => Integration.RefreshReportList();
    }
    
    [Action(nameof(OpenReport),
        typeof(ReportViewController),
        Icon = "OpenReportFolder", Name = "Open Report Folder",
        Description = "Open Report Folder")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class OpenReport : AbstractAction
    {
        protected override void Execute() => Integration.OpenReportFolder();
    }
    
    [Action(nameof(ExportReport),
        typeof(ReportViewController),
        Icon = "ExportReport", Name = "Export Report",
        Description = "Export Report")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class ExportReport : AbstractAction
    {
        protected override void Execute() => Integration.ExportReport();
    }
}