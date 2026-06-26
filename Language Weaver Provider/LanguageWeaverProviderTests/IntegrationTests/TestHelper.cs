using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace LanguageWeaverProviderTests.IntegrationTests;

public class TestHelper
{
    /// <summary>
    /// Debug-time helper: serializes every segment to Base64 and writes one entry per line to
    /// <paramref name="outputFilePath"/>. Call this from a tracepoint or a temporary breakpoint
    /// action to capture live segments for use in test data classes.
    /// </summary>
    public static bool CaptureSegmentsToFile(IEnumerable<Segment> segments, string outputFilePath)
    {
        var lines = new List<string>();
        foreach (var segment in segments)
        {
            lines.Add(SerializeSegmentToBase64(segment));
        }

        File.WriteAllLines(outputFilePath, lines);
        return true; // return value makes this usable in a conditional breakpoint expression
    }

    /// <summary>
    /// Deserializes a <see cref="Segment"/> from a Base64 string produced by <see cref="SerializeSegmentToBase64"/>.
    /// Restores the Culture from the stored culture name prefix.
    /// </summary>
    public static Segment DeserializeSegmentFromBase64(string encoded)
    {
        var separatorIndex = encoded.IndexOf('|');
        var cultureName = encoded.Substring(0, separatorIndex);
        var base64 = encoded.Substring(separatorIndex + 1);

        var bytes = Convert.FromBase64String(base64.Trim());
        var serializer = new DataContractSerializer(typeof(Segment));
        using var ms = new MemoryStream(bytes);
        var segment = (Segment)serializer.ReadObject(ms);

        if (!string.IsNullOrEmpty(cultureName))
        {
            segment.Culture = new CultureCode(cultureName);
        }

        return segment;
    }

    /// <summary>
    /// Serializes a <see cref="Segment"/> to a Base64 string using <see cref="DataContractSerializer"/>.
    /// The Culture is stored separately because <see cref="CultureCode"/> is not a DataContract type.
    /// Format: "&lt;cultureName&gt;|&lt;base64DataContractXml&gt;"
    /// </summary>
    public static string SerializeSegmentToBase64(Segment segment)
    {
        var serializer = new DataContractSerializer(typeof(Segment));
        using var ms = new MemoryStream();
        serializer.WriteObject(ms, segment);
        var base64 = Convert.ToBase64String(ms.ToArray());
        var culture = segment.Culture?.Name ?? string.Empty;
        return $"{culture}|{base64}";
    }
}