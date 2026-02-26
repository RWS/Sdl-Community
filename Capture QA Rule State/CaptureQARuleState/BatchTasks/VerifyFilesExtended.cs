using CaptureQARuleState.Components.Report_Extender;
using CaptureQARuleState.Components.SegmentMetadata_Provider;
using CaptureQARuleState.Components.SettingsProvider;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using System.IO;
using System.Linq;

namespace CaptureQARuleState.BatchTasks;

[AutomaticTask(nameof(VerifyFilesExtended),
    "BatchTaskName",
    "BatchTaskName",
    GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
[RequiresSettings(typeof(VerifyFilesExtendedSettings), typeof(VerifyFilesExtendedSettingsPage))]
public class VerifyFilesExtended : AbstractFileContentProcessingAutomaticTask
{
    public ContentVerifier ContentVerifier { get; set; } = new();
    public string XmlString { get; set; }
    private ReportExtender ReportExtender { get; } = new();
    private SegmentMetadataProvider SegmentMetadataProvider { get; set; } = new();
    private VerifyFilesExtendedSettings Settings { get; set; }
    private VerificationSettingsDataProvider VerificationSettingsDataProvider { get; set; } = new();

    public override void TaskComplete()
    {
        //var xmlString = GetOriginalVerificationReport();
        var extendedReport = ReportExtender.CreateReport(XmlString);

        AddProjectFilesTotal(extendedReport);
        if (Settings.IncludeVerificationDetails)
            AddActiveQaProviders(extendedReport);
        AddMetadataToSegments(extendedReport);
        ApplySettings(extendedReport);

        CreateReport(extendedReport);
    }

    protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
    {
        multiFileConverter.AddBilingualProcessor(ContentVerifier);
    }

    protected override void OnInitializeTask()
    {
        Settings = GetSetting<VerifyFilesExtendedSettings>();

        var settingsBundle = Project.GetSettings();
        var verificationSettings = settingsBundle.GetSettingsGroup("VerificationSettings");
        verificationSettings.GetSetting<bool>("IncludeIgnoredMessages").Value = Settings.IncludeIgnoredMessages;

        Project.UpdateSettings(settingsBundle);
        Project.Save();

        XmlString = GetOriginalVerificationReport();
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

    private void AddProjectFilesTotal(IExtendedReport extendedReport)
    {
        var projectFilesTotal = Project.GetTargetLanguageFiles().Length;
        extendedReport.AddProjectFilesTotal(projectFilesTotal);
    }

    private void ApplySettings(IExtendedReport extendedReport)
    {
        var statuses = Settings.ReportStatuses;
        extendedReport.FilterMessages(statuses);
    }

    private void CreateReport(IExtendedReport extendedReport)
    {
        var extendedReportXmlString = extendedReport.GetExtendedReportXmlString();

        var noLanguages = TaskFiles.Select(f => f.Language).Distinct().Count();
        if (noLanguages == 1)
            CreateReport("Capture QA Rule State", "Capture QA Rule State", extendedReportXmlString, TaskFiles.First().GetLanguageDirection());
        else
            CreateReport("Capture QA Rule State", "Capture QA Rule State", extendedReportXmlString);
    }

    private string GetOriginalVerificationReport()
    {
        var result = Project.RunAutomaticTask(TaskFiles.GetIds(), AutomaticTaskTemplateIds.VerifyFiles);

        ContentVerifier.Messages = result.Messages;

        var reportId = result.Reports.First().Id;

        var reportFilePath = $"{Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())}.xml";
        Project.SaveTaskReportAs(reportId, reportFilePath, ReportFormat.Xml);

        new ProjectReportsOperations((FileBasedProject)Project).RemoveReports([reportId]);

        var xmlString = File.ReadAllText(reportFilePath);
        return xmlString;
    }
}