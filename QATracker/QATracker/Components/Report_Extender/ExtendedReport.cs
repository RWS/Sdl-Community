using QATracker.Components.SegmentMetadata_Provider.Model;
using QATracker.Components.SettingsProvider.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace QATracker.Components.Report_Extender;

public class ExtendedReport(string originalXmlString) : IExtendedReport
{
    private string ActiveQaProvidersXml { get; set; }
    private string MetadataUpdatedXmlString { get; set; }
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

    public void AddStatuses(List<Segment> statuses, Guid languageFileId)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(OriginalXmlString);

        var statusLookup = statuses.ToDictionary(s => s.Id, s => s.Status);

        var fileNode = xmlDoc.SelectSingleNode($"//file[@guid='{languageFileId}']");
        var messageNodes = fileNode?.SelectNodes(".//Message");
        if (messageNodes == null)
            return;

        foreach (XmlNode messageNode in messageNodes)
        {
            var segmentIdNode = messageNode.SelectSingleNode("SegmentId");
            if (segmentIdNode == null)
                continue;

            var segmentId = segmentIdNode.InnerText;
            if (!statusLookup.TryGetValue(segmentId, out var status))
                continue;

            var statusNode = messageNode.SelectSingleNode("Status");
            if (statusNode == null)
            {
                statusNode = xmlDoc.CreateElement("Status");
                messageNode.AppendChild(statusNode);
            }

            statusNode.InnerText = status ?? string.Empty;
        }

        MetadataUpdatedXmlString = xmlDoc.OuterXml;
    }

    public string GetExtendedReportXmlString()
    {
        var xmlString = MetadataUpdatedXmlString ?? OriginalXmlString;

        if (string.IsNullOrEmpty(ActiveQaProvidersXml))
            return xmlString;

        // Insert the QA provider settings into the original report
        // This assumes the original XML has a root element where we can add the settings
        var closingTagIndex = xmlString.LastIndexOf("</task>");
        if (closingTagIndex <= 0)
            return xmlString;

        var afterClosingTagIndex = closingTagIndex;
        var beforeClosing = xmlString.Substring(0, afterClosingTagIndex);
        var afterClosing = xmlString.Substring(closingTagIndex);

        return $"{beforeClosing}{ActiveQaProvidersXml}{afterClosing}";
    }
}