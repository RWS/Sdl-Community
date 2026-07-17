using System;
using System.Collections.Generic;
using VerifyFilesAuditReport.Components.SegmentMetadata.Model;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport.Components.Reporting;

public interface IExtendedReport
{
    void AddActiveQaProviders(VerificationProviderSettings providerSettings);
    void AddProjectFilesTotal(int projectFilesTotal);
    void AddStatuses(List<Segment> statuses, Guid languageFileId);
    void FilterMessages(List<string> statuses);
    IReadOnlyCollection<Guid> GetFileIdsWithMessages();
    string GetExtendedReportXmlString();
}
