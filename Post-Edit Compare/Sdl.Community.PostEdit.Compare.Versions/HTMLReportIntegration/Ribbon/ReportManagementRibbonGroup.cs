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

    [Action(nameof(RefreshReportList),
        typeof(ReportViewController),
        Icon = "RefreshReportList", Name = "Refresh Report List",
        Description = "Refresh Report List")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Large)]
    public class RefreshReportList : AbstractAction
    {
        protected override void Execute() => Integration.SetUpReportExplorer();
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
    
    [Action(nameof(AddNewReportFolder),
        typeof(ReportViewController),
        Icon = "AddReportFolder", Name = "Add/Remove Report Folders",
        Description = "Add/Remove Report Folders")]
    [ActionLayout(typeof(ReportManagementRibbonGroup), 10, DisplayType.Default)]
    public class AddNewReportFolder : AbstractAction
    {
        protected override void Execute() => Integration.EditReportFolderList();
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