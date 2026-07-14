using System;
using System.IO;
using System.Linq;
using NLog;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;
using System.Threading.Tasks;
using VerifyFilesAuditReport.Logging;

namespace VerifyFilesAuditReport.BatchTasks;

/// <summary>
/// Runs the built-in Verify Files task in the background and yields its XML report,
/// relaying verification messages through the <see cref="VerificationMessageChannel"/>.
/// The built-in report is removed from the project so only the extended one remains.
/// </summary>
public class OriginalReportProvider(VerificationMessageChannel messageChannel)
{
    private static readonly Logger Logger = Log.GetLogger(typeof(OriginalReportProvider).FullName);

    public Task<string> RunVerifyFilesAsync(IProject project, Guid[] taskFileIds)
    {
        return System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                var verifyFilesTask = project.RunAutomaticTask
                (
                    taskFileIds,
                    AutomaticTaskTemplateIds.VerifyFiles,
                    (_, _) => { }, (_, args) => messageChannel.Post(args.Message)
                );

                return HarvestReport(project, verifyFilesTask);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to generate the original verification report.");
                throw;
            }
            finally
            {
                messageChannel.Complete();
            }
        });
    }

    private static string HarvestReport(IProject project, AutomaticTask verifyFilesTask)
    {
        var reportId = verifyFilesTask.Reports.First().Id;

        var reportFilePath = $"{Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())}.xml";
        project.SaveTaskReportAs(reportId, reportFilePath, ReportFormat.Xml);

        new ProjectReportsOperations((FileBasedProject)project).RemoveReports([reportId]);

        Logger.Info($"Verify Files temp report saved at: {reportFilePath}");

        return File.ReadAllText(reportFilePath);
    }
}
