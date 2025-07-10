using QATracker.Components.SettingsProvider.Model;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace QATracker.Components.Report_Extender;

public class ExtendedReport(string originalXmlString) : IExtendedReport
{
    private string OriginalXmlString { get; } = originalXmlString;

    public void AddActiveQaProviders(VerificationProviderSettings qaProvidersXmlString)
    {
        var projectVerifierSettings = qaProvidersXmlString.ProjectVerificationProviders;

        var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));
        using var stringWriter = new StringWriter();

        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = false
        };

        using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", ""); // Remove default namespace

        serializer.Serialize(xmlWriter, projectVerifierSettings, namespaces);
        ActiveQaProvidersXml = stringWriter.ToString();
    }

    private string ActiveQaProvidersXml { get; set; }

    public string GetExtendedReportXmlString()
    {
        if (string.IsNullOrEmpty(ActiveQaProvidersXml))
            return OriginalXmlString;

        // Insert the QA provider settings into the original report
        // This assumes the original XML has a root element where we can add the settings
        var closingTagIndex = OriginalXmlString.LastIndexOf("</task>");
        if (closingTagIndex <= 0)
            return OriginalXmlString;

        var afterClosingTagIndex = closingTagIndex;
        var beforeClosing = OriginalXmlString.Substring(0, afterClosingTagIndex);
        var afterClosing = OriginalXmlString.Substring(closingTagIndex);

        return $"{beforeClosing}{ActiveQaProvidersXml}{afterClosing}";

    }
}