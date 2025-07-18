using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using QATracker.Components.SegmentMetadata_Provider.Model;
using Sdl.ProjectAutomation.Core;

namespace QATracker.Components.SegmentMetadata_Provider;

public class SegmentMetadataProvider
{
    public static List<Segment> GetAllSegmentStatuses(IProject project, Guid languageFileGuid)
    {
        var sdlxliffPath = GetSdlxliffPath(project, languageFileGuid);

        XNamespace xliffNs = "urn:oasis:names:tc:xliff:document:1.2";
        XNamespace sdlNs = "http://sdl.com/FileTypes/SdlXliff/1.0";
        var doc = XDocument.Load(sdlxliffPath);

        var segments = doc
            .Descendants(xliffNs + "trans-unit")
            .SelectMany(tu => tu
                .Elements(sdlNs + "seg-defs")
                .Elements(sdlNs + "seg")
                .Select(seg => new Segment
                {
                    Id = (string)seg.Attribute("id"),
                    Status = (string)seg.Attribute("conf") ?? (string)seg.Attribute("state") ?? "Not Translated"
                })
            )
            .ToList();

        return segments;
    }

    public static string GetSdlxliffPath(IProject project, Guid languageFileGuid)
    {
        // locate the language‐file in the project by its Guid
        var langFile = project.GetTargetLanguageFiles()
            .Single(f => f.Id == languageFileGuid);

        // LocalFilePath is the full path to the .sdlxliff on disk
        return langFile.LocalFilePath;
    }
}