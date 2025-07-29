using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CaptureQARuleState.Components.SegmentMetadata_Provider.Model;
using CaptureQARuleState.Components.SettingsProvider.Model;

namespace CaptureQARuleState.Components.Report_Extender;

public class ExtendedReport(string originalXmlString) : IExtendedReport
{
    private string ActiveQaProvidersXml { get; set; }
    private string OriginalXmlString { get; } = originalXmlString;
    private string UpdatedXmlString { get; set; }

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
        xmlDoc.LoadXml(UpdatedXmlString ?? OriginalXmlString);

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

        UpdatedXmlString = xmlDoc.OuterXml;
    }

    public void FilterMessages(List<string> statuses)
    {
        if (statuses == null || !statuses.Any() || statuses.Count == 8)
            return;

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(UpdatedXmlString ?? OriginalXmlString);

        var messageNodes = xmlDoc.SelectNodes("//Message");
        if (messageNodes == null)
            return;

        var statusesSet = new HashSet<string>(statuses, StringComparer.OrdinalIgnoreCase);
        foreach (XmlNode messageNode in messageNodes)
        {
            var statusNode = messageNode.SelectSingleNode("Status");
            if (statusNode != null && statusesSet.Contains(statusNode.InnerText))
                continue;

            // Remove the message node if it does not match the statuses
            var parent = messageNode.ParentNode;
            parent?.RemoveChild(messageNode);
        }
        UpdatedXmlString = xmlDoc.OuterXml;
    }

    public void AddProjectFilesTotal(int projectFilesTotal)
    {
        // Use the most up-to-date XML string
        var xmlString = UpdatedXmlString ?? OriginalXmlString;

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);

        // Find the <taskInfo>/<project> element
        var projectNode = xmlDoc.SelectSingleNode("//taskInfo/project");
        if (projectNode == null)
            return;

        // Add or update the projectFilesTotal attribute
        var attr = projectNode.Attributes["projectFilesTotal"];
        if (attr == null)
        {
            attr = xmlDoc.CreateAttribute("projectFilesTotal");
            projectNode.Attributes.Append(attr);
        }
        attr.Value = projectFilesTotal.ToString();

        // Save the updated XML
        UpdatedXmlString = xmlDoc.OuterXml;
    }

    public string GetExtendedReportXmlString()
    {
        var xmlString = UpdatedXmlString ?? OriginalXmlString;

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