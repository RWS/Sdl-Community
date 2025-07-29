using System;
using System.Collections.Generic;
using CaptureQARuleState.Components.SegmentMetadata_Provider.Model;
using CaptureQARuleState.Components.SettingsProvider.Model;

namespace CaptureQARuleState.Components.Report_Extender;

public interface IExtendedReport
{
    void AddActiveQaProviders(VerificationProviderSettings qaProvidersXmlString);
    string GetExtendedReportXmlString();
    void AddStatuses(List<Segment> statuses, Guid languageFileId);
    void FilterMessages(List<string> statuses);
}