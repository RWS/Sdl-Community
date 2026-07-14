using System;
using System.Collections.Generic;
using VerifyFilesAuditReport.Components.SegmentMetadata_Provider.Model;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport.Components.Report_Extender;

public interface IExtendedReport
{
    void AddActiveQaProviders(VerificationProviderSettings qaProvidersXmlString);
    string GetExtendedReportXmlString();
    void AddStatuses(List<Segment> statuses, Guid languageFileId);
    void FilterMessages(List<string> statuses);
    void AddProjectFilesTotal(int projectFilesTotal);
}