using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NLog;
using Sdl.ProjectAutomation.Core;
using VerifyFilesAuditReport.Components.SegmentMetadata.Model;
using VerifyFilesAuditReport.Logging;

namespace VerifyFilesAuditReport.Components.SegmentMetadata;

public class SegmentMetadataProvider
{
    private static readonly Logger Logger = Log.GetLogger(typeof(SegmentMetadataProvider).FullName);

    private static readonly XNamespace XliffNs = "urn:oasis:names:tc:xliff:document:1.2";
    private static readonly XNamespace SdlNs = "http://sdl.com/FileTypes/SdlXliff/1.0";

    public List<Segment> GetAllSegmentStatuses(IProject project, Guid languageFileGuid)
    {
        var sdlxliffPath = GetSdlxliffPath(project, languageFileGuid);
        if (sdlxliffPath is null)
            return null;

        Logger.Debug($"Reading segment statuses from '{sdlxliffPath}'.");

        XDocument doc;
        try
        {
            doc = XDocument.Load(sdlxliffPath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to load sdlxliff file '{sdlxliffPath}'.");
            throw;
        }

        return doc
            .Descendants(XliffNs + "trans-unit")
            .SelectMany(tu => tu
                .Elements(SdlNs + "seg-defs")
                .Elements(SdlNs + "seg")
                .Select(seg => new Segment
                {
                    Id = (string)seg.Attribute("id"),
                    Status = GetUiStatusString(seg)
                })
            )
            .ToList();
    }

    private static string GetSdlxliffPath(IProject project, Guid languageFileGuid)
    {
        var langFile = project.GetTargetLanguageFiles()
            .Single(f => f.Id == languageFileGuid);

        return langFile.Role == FileRole.Reference
            ? null
            : langFile.LocalFilePath;
    }

    private static string GetUiStatusString(XElement seg)
    {
        var status = (string)seg.Attribute("conf") ?? (string)seg.Attribute("state") ?? "Not Translated";
        return SegmentStatuses.ToUiName(status);
    }
}
