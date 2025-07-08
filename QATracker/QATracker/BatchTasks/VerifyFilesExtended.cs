using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.IO;
using System.Linq;
using QATracker.Components.Report_Extender;
using QATracker.Components.SettingsProvider;

namespace QATracker.BatchTasks;

[AutomaticTask(nameof(VerifyFilesExtended),
    "BatchTaskName",
    "BatchTaskName",
    GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
public class VerifyFilesExtended : AbstractFileContentProcessingAutomaticTask
{
    private ReportExtender ReportExtender { get; } = new();

    public override void TaskComplete()
    {
        var xmlString = GetOriginalVerificationReport();

        var extendedReport = ReportExtender.CreateReport(xmlString);

        var activeQaProvidersXmlString = VerificationSettingsDataProvider.GetVerificationSettings(Project);
        //extendedReport.AddActiveQaProviders(activeQaProvidersXmlString);

        CreateReport(extendedReport.GetExtendedReportXmlString());
    }

    public VerificationSettingsDataProvider VerificationSettingsDataProvider { get; set; } = new();

    protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
    { }

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