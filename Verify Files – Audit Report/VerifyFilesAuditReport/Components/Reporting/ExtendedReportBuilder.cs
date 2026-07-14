using System.Collections.Generic;
using System.Linq;
using Sdl.ProjectAutomation.Core;
using VerifyFilesAuditReport.Components.SegmentMetadata;
using VerifyFilesAuditReport.Components.SettingsProvider;

namespace VerifyFilesAuditReport.Components.Reporting;

/// <summary>
/// Assembles the extended verification report from the built-in Verify Files
/// report and the project's QA configuration and segment metadata.
/// </summary>
public class ExtendedReportBuilder(
    VerificationSettingsDataProvider verificationSettingsProvider,
    SegmentMetadataProvider segmentMetadataProvider)
{
    public ExtendedReportBuilder() : this(new VerificationSettingsDataProvider(), new SegmentMetadataProvider())
    { }

    public string Build(string originalReportXml, IProject project, bool includeVerificationDetails,
        List<string> reportStatuses)
    {
        IExtendedReport report = new ExtendedReport(originalReportXml);

        var targetLanguageFiles = project.GetTargetLanguageFiles();
        report.AddProjectFilesTotal(targetLanguageFiles.Length);

        if (includeVerificationDetails)
            report.AddActiveQaProviders(verificationSettingsProvider.GetVerificationSettings(project));

        foreach (var languageFile in targetLanguageFiles.Where(lf => lf.Role != FileRole.Reference))
        {
            var statuses = segmentMetadataProvider.GetAllSegmentStatuses(project, languageFile.Id);
            report.AddStatuses(statuses, languageFile.Id);
        }

        report.FilterMessages(reportStatuses);

        return report.GetExtendedReportXmlString();
    }
}
