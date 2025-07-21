using System;
using System.Collections.Generic;
using QATracker.Components.SegmentMetadata_Provider.Model;
using QATracker.Components.SettingsProvider.Model;

namespace QATracker.Components.Report_Extender;

public interface IExtendedReport
{
    void AddActiveQaProviders(VerificationProviderSettings qaProvidersXmlString);
    string GetExtendedReportXmlString();
    void AddStatuses(List<Segment> statuses, Guid languageFileId);
    void FilterMessages(List<string> statuses);
}