using QATracker.Components.SettingsProvider.Model;

namespace QATracker.Components.Report_Extender;

public interface IExtendedReport
{
    void AddActiveQaProviders(VerificationProviderSettings qaProvidersXmlString);
    string GetExtendedReportXmlString();
}