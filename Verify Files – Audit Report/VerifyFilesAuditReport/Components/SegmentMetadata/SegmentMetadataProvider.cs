using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NLog;
using Sdl.ProjectAutomation.Core;
using VerifyFilesAuditReport.BatchTasks;
using VerifyFilesAuditReport.Components.SegmentMetadata_Provider.Model;
using VerifyFilesAuditReport.Logging;

namespace VerifyFilesAuditReport.Components.SegmentMetadata_Provider;

public class SegmentMetadataProvider
{
    private static readonly Logger Logger = Log.GetLogger(typeof(SegmentMetadataProvider).FullName);

    public List<Segment> GetAllSegmentStatuses(IProject project, Guid languageFileGuid)
    {
        var sdlxliffPath = GetSdlxliffPath(project, languageFileGuid);
        if (sdlxliffPath is null)
            return null;

        XNamespace xliffNs = "urn:oasis:names:tc:xliff:document:1.2";
        XNamespace sdlNs = "http://sdl.com/FileTypes/SdlXliff/1.0";

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

        var segments = doc
            .Descendants(xliffNs + "trans-unit")
            .SelectMany(tu => tu
                .Elements(sdlNs + "seg-defs")
                .Elements(sdlNs + "seg")
                .Select(seg => new Segment
                {
                    Id = (string)seg.Attribute("id"),
                    Status = GetUiStatusString(seg)
                })
            )
            .ToList();

        return segments;
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
        return Constants.Statuses[status];
    }
}