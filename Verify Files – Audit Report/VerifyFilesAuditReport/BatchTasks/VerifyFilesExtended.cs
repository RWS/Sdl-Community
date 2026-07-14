using NLog;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using System;
using System.IO;
using System.Linq;
using VerifyFilesAuditReport.Components.Report_Extender;
using VerifyFilesAuditReport.Components.SegmentMetadata_Provider;
using VerifyFilesAuditReport.Components.SettingsProvider;
using VerifyFilesAuditReport.Logging;
using Task = System.Threading.Tasks.Task;

namespace VerifyFilesAuditReport.BatchTasks;

[AutomaticTask(nameof(VerifyFilesExtended),
    "BatchTaskName",
    "BatchTaskName",
    GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
[RequiresSettings(typeof(VerifyFilesExtendedSettings), typeof(VerifyFilesExtendedSettingsPage))]
public class VerifyFilesExtended : AbstractFileContentProcessingAutomaticTask
{

    private static readonly Logger Logger = Log.GetLogger("Verify Files - Audit Report Task");
    public ContentVerifier ContentVerifier { get; set; } = new();
    public string XmlString { get; set; }
    private ReportExtender ReportExtender { get; } = new();
    private SegmentMetadataProvider SegmentMetadataProvider { get; set; } = new();
    private VerifyFilesExtendedSettings Settings { get; set; }
    private VerificationSettingsDataProvider VerificationSettingsDataProvider { get; set; } = new();

    public override void TaskComplete()
    {
        // Block until the background Verify Files run has delivered its report:
        // returning earlier lets Studio finalize the task before the extended
        // report is registered, so it would miss the task results window.
        Signal.WhenFinished().GetAwaiter().GetResult();

        Signal.Reset();

        try
        {
            Logger.Info("Building extended verification report.");

            var extendedReport = ReportExtender.CreateReport(XmlString);

            AddProjectFilesTotal(extendedReport);
            if (Settings.IncludeVerificationDetails)
                AddActiveQaProviders(extendedReport);
            AddMetadataToSegments(extendedReport);
            ApplySettings(extendedReport);

            CreateReport(extendedReport);

            Logger.Info("Extended verification report created.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to build the extended verification report.");
        }
    }

    protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
    {
        multiFileConverter.AddBilingualProcessor(ContentVerifier);
    }

    protected override void OnInitializeTask()
    {
        Logger.Info("Initializing Verify Files - Audit Report task.");

        Settings = GetSetting<VerifyFilesExtendedSettings>();

        var settingsBundle = Project.GetSettings();
        var verificationSettings = settingsBundle.GetSettingsGroup("VerificationSettings");
        verificationSettings.GetSetting<bool>("IncludeIgnoredMessages").Value = Settings.IncludeIgnoredMessages;

        Project.UpdateSettings(settingsBundle);
        Project.Save();

        SetOriginalVerificationReport();
    }

    private void AddActiveQaProviders(IExtendedReport extendedReport)
    {
        var activeQaProvidersXmlString = VerificationSettingsDataProvider.GetVerificationSettings(Project);
        extendedReport.AddActiveQaProviders(activeQaProvidersXmlString);
    }

    private void AddMetadataToSegments(IExtendedReport extendedReport)
    {
        var languageFiles = Project.GetTargetLanguageFiles().Where(lf=>lf.Role != FileRole.Reference);

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

    private void SetOriginalVerificationReport()
    {
        AutomaticTask result = null;
        Task.Run(() =>
        {
            try
            {
                result = Project.RunAutomaticTask
                (
                    TaskFiles.GetIds(),
                    AutomaticTaskTemplateIds.VerifyFiles,
                    (_, _) => { }, (_, args) => Signal.SendMessage(args.Message)
                );

                var reportId = result.Reports.First().Id;

                var reportFilePath = $"{Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())}.xml";
                Project.SaveTaskReportAs(reportId, reportFilePath, ReportFormat.Xml);

                new ProjectReportsOperations((FileBasedProject)Project).RemoveReports([reportId]);

                XmlString = File.ReadAllText(reportFilePath);

                Logger.Log(LogLevel.Info, $"Verify Files temp report saved at: {reportFilePath}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to generate the original verification report.");
            }
            finally
            {
                Signal.Finish();
            }
        });
    }
}