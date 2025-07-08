namespace QATracker.Components.Report_Extender;

public class ExtendedReport(string originalXmlString) : IExtendedReport
{
    private string OriginalXmlString { get; } = originalXmlString;

    public void AddActiveQaProviders(string qaProvidersXmlString)
    {

    }

    public string GetExtendedReportXmlString()
    {
        return OriginalXmlString;
    }
}