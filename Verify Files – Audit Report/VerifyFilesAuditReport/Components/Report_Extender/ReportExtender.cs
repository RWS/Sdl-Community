namespace VerifyFilesAuditReport.Components.Report_Extender;

public class ReportExtender
{
    public IExtendedReport CreateReport(string xmlString)
    {
        return new ExtendedReport(xmlString);
    }
}