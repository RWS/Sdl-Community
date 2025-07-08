namespace QATracker.Components.Report_Extender;

public interface IExtendedReport
{
    void AddActiveQaProviders(string qaProvidersXmlString);
    string GetExtendedReportXmlString();
}