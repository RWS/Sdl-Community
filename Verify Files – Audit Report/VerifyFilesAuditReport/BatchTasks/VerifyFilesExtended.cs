using System;
using System.Diagnostics;
using System.Linq;
using NLog;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using System.Threading.Tasks;
using VerifyFilesAuditReport.Components.Reporting;
using VerifyFilesAuditReport.Logging;

namespace VerifyFilesAuditReport.BatchTasks;

[AutomaticTask(nameof(VerifyFilesExtended),
    "BatchTaskName",
    "BatchTaskName",
    GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
[RequiresSettings(typeof(VerifyFilesExtendedSettings), typeof(VerifyFilesExtendedSettingsPage))]
public class VerifyFilesExtended : AbstractFileContentProcessingAutomaticTask
{
    private const string ReportName = "Capture QA Rule State";
    private const string VerificationSettingsGroupId = "VerificationSettings";
    private const string IncludeIgnoredMessagesSettingId = "IncludeIgnoredMessages";

    private static readonly Logger Logger = Log.GetLogger("Verify Files - Audit Report Task");

    private readonly VerificationMessageChannel _messageChannel = new();
    private readonly ContentVerifier _contentVerifier;
    private readonly ExtendedReportBuilder _reportBuilder = new();

    private Task<string> _originalReportXml;
    private VerifyFilesExtendedSettings _settings;

    public VerifyFilesExtended()
    {
        _contentVerifier = new ContentVerifier(_messageChannel);
    }

    public override void TaskComplete()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            // Studio finalizes the task as soon as this method returns; the extended
            // report must be registered before then or it misses the results window.
            var originalReportXml = _originalReportXml.GetAwaiter().GetResult();
            Logger.Info($"Waited {stopwatch.ElapsedMilliseconds} ms for the built-in Verify Files report.");

            stopwatch.Restart();
            var extendedReportXml = _reportBuilder.Build(originalReportXml, Project,
                _settings.IncludeVerificationDetails, _settings.ReportStatuses);
            Logger.Info($"Built the extended report in {stopwatch.ElapsedMilliseconds} ms.");

            stopwatch.Restart();
            CreateExtendedReport(extendedReportXml);
            Logger.Info($"Studio persisted the extended report in {stopwatch.ElapsedMilliseconds} ms.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to build the extended verification report.");
        }
        finally
        {
            _messageChannel.Dispose();
        }
    }

    protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
    {
        multiFileConverter.AddBilingualProcessor(_contentVerifier);
    }

    protected override void OnInitializeTask()
    {
        Logger.Info("Initializing Verify Files - Audit Report task.");

        _settings = GetSetting<VerifyFilesExtendedSettings>();

        ApplyIncludeIgnoredMessagesSetting();

        _originalReportXml = new OriginalReportProvider(_messageChannel)
            .RunVerifyFilesAsync(Project, TaskFiles.GetIds());
    }

    private void ApplyIncludeIgnoredMessagesSetting()
    {
        var settingsBundle = Project.GetSettings();
        var verificationSettings = settingsBundle.GetSettingsGroup(VerificationSettingsGroupId);
        verificationSettings.GetSetting<bool>(IncludeIgnoredMessagesSettingId).Value = _settings.IncludeIgnoredMessages;

        Project.UpdateSettings(settingsBundle);
        Project.Save();
    }

    private void CreateExtendedReport(string extendedReportXml)
    {
        var languageCount = TaskFiles.Select(f => f.Language).Distinct().Count();
        if (languageCount == 1)
            CreateReport(ReportName, ReportName, extendedReportXml, TaskFiles.First().GetLanguageDirection());
        else
            CreateReport(ReportName, ReportName, extendedReportXml);
    }
}
