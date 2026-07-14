using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using VerifyFilesAuditReport.Components.SegmentMetadata;
using VerifyFilesAuditReport.Components.SegmentMetadata.Model;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport.Components.Reporting;

/// <summary>
/// Wraps the XML report produced by the built-in Verify Files task and
/// enriches it with the data that makes it auditable.
/// </summary>
public class ExtendedReport : IExtendedReport
{
    private readonly XmlDocument _document = new();

    public ExtendedReport(string originalXmlString)
    {
        _document.LoadXml(originalXmlString);
    }

    public void AddActiveQaProviders(VerificationProviderSettings providerSettings)
    {
        var settingsXml = SerializeToXmlString(providerSettings.ProjectVerificationProviders);

        var fragment = _document.CreateDocumentFragment();
        fragment.InnerXml = settingsXml;

        _document.DocumentElement?.AppendChild(fragment);
    }

    public void AddProjectFilesTotal(int projectFilesTotal)
    {
        var projectNode = _document.SelectSingleNode("//taskInfo/project");
        if (projectNode == null)
            return;

        var attribute = projectNode.Attributes["projectFilesTotal"];
        if (attribute == null)
        {
            attribute = _document.CreateAttribute("projectFilesTotal");
            projectNode.Attributes.Append(attribute);
        }

        attribute.Value = projectFilesTotal.ToString();
    }

    public void AddStatuses(List<Segment> statuses, Guid languageFileId)
    {
        if (statuses == null)
            return;

        var fileNode = _document.SelectSingleNode($"//file[@guid='{languageFileId}']");
        var messageNodes = fileNode?.SelectNodes(".//Message");
        if (messageNodes == null)
            return;

        var statusLookup = statuses.ToDictionary(s => s.Id, s => s.Status);

        foreach (XmlNode messageNode in messageNodes)
        {
            var segmentId = messageNode.SelectSingleNode("SegmentId")?.InnerText;
            if (segmentId == null || !statusLookup.TryGetValue(segmentId, out var status))
                continue;

            var statusNode = messageNode.SelectSingleNode("Status");
            if (statusNode == null)
            {
                statusNode = _document.CreateElement("Status");
                messageNode.AppendChild(statusNode);
            }

            statusNode.InnerText = status ?? string.Empty;
        }
    }

    public void FilterMessages(List<string> statuses)
    {
        if (statuses == null || !statuses.Any() || statuses.Count == SegmentStatuses.UiNames.Count)
            return;

        var messageNodes = _document.SelectNodes("//Message");
        if (messageNodes == null)
            return;

        var statusesToKeep = new HashSet<string>(statuses, StringComparer.OrdinalIgnoreCase);
        foreach (XmlNode messageNode in messageNodes)
        {
            var statusNode = messageNode.SelectSingleNode("Status");
            if (statusNode != null && statusesToKeep.Contains(statusNode.InnerText))
                continue;

            messageNode.ParentNode?.RemoveChild(messageNode);
        }
    }

    public string GetExtendedReportXmlString() => _document.OuterXml;

    private static string SerializeToXmlString(VerificationSettingsTreeNode settings)
    {
        var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));

        using var stringWriter = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(stringWriter,
                   new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false }))
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            serializer.Serialize(xmlWriter, settings, namespaces);
        }

        return stringWriter.ToString();
    }
}
