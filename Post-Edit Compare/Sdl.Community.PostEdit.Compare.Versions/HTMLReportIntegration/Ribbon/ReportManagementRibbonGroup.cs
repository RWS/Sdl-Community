using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Ribbon
{
    [Action(nameof(AddNewReportFolder),
        typeof(ReportViewController),
        Icon = "Settings2", Name = "Report Locations",
        Description = "Report Locations")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class AddNewReportFolder : AbstractAction
    {
        protected override void Execute() => Integration.EditReportFolderList();
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

    [Action(nameof(OpenReport),
        typeof(ReportViewController),
        Icon = "OpenReportFolder", Name = "Open Report Folder",
        Description = "Open Report Folder")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class OpenReport : AbstractAction
    {
        protected override void Execute() => Integration.OpenReportFolder();
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
}