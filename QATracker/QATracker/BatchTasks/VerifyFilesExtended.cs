using QATracker.Components.Report_Extender;
using QATracker.Components.SegmentMetadata_Provider;
using QATracker.Components.SettingsProvider;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using System.IO;
using System.Linq;

namespace QATracker.BatchTasks;

[AutomaticTask(nameof(VerifyFilesExtended),
    "BatchTaskName",
    "BatchTaskName",
    GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
[RequiresSettings(typeof(VerifyFilesExtendedSettings), typeof(VerifyFilesExtendedSettingsPage))]
public class VerifyFilesExtended : AbstractFileContentProcessingAutomaticTask
{
    private ReportExtender ReportExtender { get; } = new();
    private SegmentMetadataProvider SegmentMetadataProvider { get; set; } = new();
    private VerifyFilesExtendedSettings Settings { get; set; }
    private VerificationSettingsDataProvider VerificationSettingsDataProvider { get; set; } = new();

    public override void TaskComplete()
    {
        var xmlString = GetOriginalVerificationReport();
        var extendedReport = ReportExtender.CreateReport(xmlString);

        AddActiveQaProviders(extendedReport);
        AddMetadataToSegments(extendedReport);
        ApplySettings(extendedReport);

        var extendedReportXmlString = extendedReport.GetExtendedReportXmlString();
        CreateReport(extendedReportXmlString);
    }

    private void ApplySettings(IExtendedReport extendedReport)
    {
        var statuses = Settings.ReportStatuses;
        extendedReport.FilterMessages(statuses);
    }

    protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
    { }

    protected override void OnInitializeTask()
    {
        Settings = GetSetting<VerifyFilesExtendedSettings>();
    }

    private void AddActiveQaProviders(IExtendedReport extendedReport)
    {
        var activeQaProvidersXmlString = VerificationSettingsDataProvider.GetVerificationSettings(Project);
        extendedReport.AddActiveQaProviders(activeQaProvidersXmlString);
    }

    private void AddMetadataToSegments(IExtendedReport extendedReport)
    {
        var languageFiles = Project.GetTargetLanguageFiles();

        foreach (var languageFile in languageFiles)
        {
            var statuses = SegmentMetadataProvider.GetAllSegmentStatuses(Project, languageFile.Id);
            extendedReport.AddStatuses(statuses, languageFile.Id);
        }
    }

    private void CreateReport(string extendedReportString)
    {
        var noLanguages = TaskFiles.Select(f => f.Language).Distinct().Count();
        if (noLanguages == 1)
            CreateReport("Verify Files Extended", "Verify Files Extended", extendedReportString, TaskFiles.First().GetLanguageDirection());
        else
            CreateReport("Verify Files Extended", "Verify Files Extended", extendedReportString);
    }

    private string GetOriginalVerificationReport()
    {
        var result = Project.RunAutomaticTask(TaskFiles.GetIds(), AutomaticTaskTemplateIds.VerifyFiles);
        var reportId = result.Reports.First().Id;

        var reportFilePath = $"{Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())}.xml";
        Project.SaveTaskReportAs(reportId, reportFilePath, ReportFormat.Xml);

        new ProjectReportsOperations((FileBasedProject)Project).RemoveReports([reportId]);

        var xmlString = File.ReadAllText(reportFilePath);
        return xmlString;
    }
}