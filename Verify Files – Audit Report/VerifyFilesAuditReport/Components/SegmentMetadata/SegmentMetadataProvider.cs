using System;
using System.Collections.Generic;
using System.Xml;
using NLog;
using VerifyFilesAuditReport.Components.SegmentMetadata.Model;
using VerifyFilesAuditReport.Logging;

namespace VerifyFilesAuditReport.Components.SegmentMetadata;

public class SegmentMetadataProvider
{
    private static readonly Logger Logger = Log.GetLogger(typeof(SegmentMetadataProvider).FullName);

    private const string XliffNamespace = "urn:oasis:names:tc:xliff:document:1.2";
    private const string SdlNamespace = "http://sdl.com/FileTypes/SdlXliff/1.0";

    public List<Segment> GetAllSegmentStatuses(string sdlxliffPath)
    {
        if (string.IsNullOrEmpty(sdlxliffPath))
            return null;

        Logger.Debug($"Reading segment statuses from '{sdlxliffPath}'.");

        try
        {
            return ReadSegmentStatuses(sdlxliffPath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to read segment statuses from '{sdlxliffPath}'.");
            return null;
        }
    }

    /// <summary>
    /// Streams the sdlxliff with an XmlReader: only seg-defs attributes are inspected and the
    /// header (which embeds the original document) is skipped, so the file is never DOM-loaded.
    /// </summary>
    public static List<Segment> ReadSegmentStatuses(string sdlxliffPath)
    {
        var segments = new List<Segment>();

        var settings = new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true };
        using var reader = XmlReader.Create(sdlxliffPath, settings);

        var insideSegDefs = false;
        while (!reader.EOF)
        {
            if (reader.NodeType == XmlNodeType.Element &&
                reader.NamespaceURI == XliffNamespace && reader.LocalName == "header")
            {
                reader.Skip();
                continue;
            }

            if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == SdlNamespace)
            {
                if (reader.LocalName == "seg-defs" && !reader.IsEmptyElement)
                    insideSegDefs = true;
                else if (insideSegDefs && reader.LocalName == "seg")
                    segments.Add(ReadSegment(reader));
            }
            else if (reader.NodeType == XmlNodeType.EndElement &&
                     reader.NamespaceURI == SdlNamespace && reader.LocalName == "seg-defs")
            {
                insideSegDefs = false;
            }

            if (!reader.Read())
                break;
        }

        return segments;
    }

    private static Segment ReadSegment(XmlReader reader)
    {
        var status = reader.GetAttribute("conf") ?? reader.GetAttribute("state") ?? "Not Translated";

        return new Segment
        {
            Id = reader.GetAttribute("id"),
            Status = SegmentStatuses.ToUiName(status)
        };
    }
}
