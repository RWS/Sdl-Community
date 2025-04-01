using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
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

    [Action(nameof(RefreshReportList),
        typeof(ReportViewController),
        Icon = "RefreshReportExplorer_Icon", Name = "Refresh Report List",
        Description = "Refresh Report List")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Large)]
    public class RefreshReportList : AbstractAction
    {
        protected override void Execute() => Integration.SetUpReportExplorer();
    }

    [Action(nameof(UndockReportViewer),
        typeof(ReportViewController),
        Icon = "Undock", Name = "Undock Report Viewer",
        Description = "Undock Report Viewer")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Large)]
    public class UndockReportViewer : AbstractAction
    {

        protected override void Execute()
        {
            Integration.UndockReportViewer();
            Enabled = false;
        }
    }

    [Action(nameof(AddNewReportFolder),
        typeof(ReportViewController),
        Icon = "Settings2", Name = "Report Locations",
        Description = "Report Locations")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class AddNewReportFolder : AbstractAction
    {
        protected override void Execute() => Integration.EditReportFolderList();
    }

    [Action(nameof(OpenReportBackupFolder),
        typeof(ReportViewController),
        Icon = "OpenReportFolder", Name = "Open Report Backup Folder",
        Description = "Open Report Backup Folder")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class OpenReportBackupFolder : AbstractAction
    {
        protected override void Execute() => Integration.OpenReportBackupFolder();
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
        Icon = "Export", Name = "Export Report",
        Description = "Export Report")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class ExportReport : AbstractAction
    {
        protected override void Execute() => Integration.ExportReport();
    }
}