using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Pure (no I/O, no UI) XLIFF batching for the Edge translation path. Edge only preserves inline
    /// <c>&lt;g&gt;</c>/<c>&lt;x&gt;</c> tags when a whole batch is submitted as ONE consolidated
    /// <c>application/x-xliff</c> document, and it echoes the request <c>&lt;trans-unit&gt;</c> ids in
    /// its response. This type owns both halves of that contract so the logic is unit-testable in
    /// isolation from <see cref="EdgeService"/>'s HTTP/auth/dialog dependencies.
    /// </summary>
    public static class EdgeXliffBatch
    {
        /// <summary>
        /// Builds the single consolidated XLIFF 1.2 document Edge requires: one body holding one
        /// <c>&lt;trans-unit&gt;</c> per segment (ids 1..N), produced straight from the
        /// <see cref="SegmentSerializer"/> structure. Edge only preserves inline <c>&lt;g&gt;</c>/<c>&lt;x&gt;</c>
        /// tags when the whole batch is one document under <c>application/x-xliff</c>; concatenating
        /// standalone XLIFF documents is rejected with <c>400 Bad Request</c>.
        /// </summary>
        public static string Consolidate(IReadOnlyList<SegmentSerializer> segmentSerializers)
        {
            var transUnits = new List<XElement>(segmentSerializers.Count);
            for (var i = 0; i < segmentSerializers.Count; i++)
                transUnits.Add(segmentSerializers[i].CreateTransUnit(i + 1));

            var first = segmentSerializers.Count > 0 ? segmentSerializers[0] : null;
            return SegmentSerializer.BuildXliffDocument(
                first?.SourceLanguage ?? string.Empty,
                first?.TargetLanguage ?? string.Empty,
                transUnits);
        }

        /// <summary>
        /// Splits the consolidated Edge XLIFF response back into per-segment XLIFF fragments,
        /// ordered to match the request. Edge echoes the request <c>&lt;trans-unit&gt;</c> ids, so each
        /// fragment is mapped to its segment index by id; missing ids yield an empty string so the
        /// caller still produces one result per source segment. Each returned fragment is a complete
        /// <c>&lt;trans-unit&gt;</c> element that <see cref="SegmentSerializer.DeserializeSegment"/> can parse
        /// (it locates the nested <c>&lt;alt-trans&gt;&lt;target&gt;</c> at any depth).
        /// </summary>
        public static string[] Split(string responseXliff, int segmentCount)
        {
            var fragments = new string[segmentCount];

            var transUnits = XDocument.Parse(responseXliff)
                .Descendants()
                .Where(e => e.Name.LocalName == "trans-unit");

            foreach (var transUnit in transUnits)
            {
                var idValue = transUnit.Attribute("id")?.Value;
                if (!int.TryParse(idValue, out var id))
                    continue;

                var index = id - 1;
                if (index < 0 || index >= segmentCount)
                    continue;

                fragments[index] = transUnit.ToString(SaveOptions.DisableFormatting);
            }

            for (var i = 0; i < fragments.Length; i++)
                fragments[i] ??= string.Empty;

            return fragments;
        }
    }
}
