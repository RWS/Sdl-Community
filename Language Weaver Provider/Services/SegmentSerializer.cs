using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LanguageWeaverProvider.Services;

public class SegmentSerializer
{
    private static readonly XNamespace Ns = "urn:oasis:names:tc:xliff:document:1.2";

    private readonly string _sourceLanguage;
    private readonly string _targetLanguage;
    private XElement _source;

    /// <summary>
    /// Creates a serializer for <paramref name="segment"/>, emitting an XLIFF document whose
    /// <c>source-language</c>/<c>target-language</c> attributes reflect the real translation
    /// direction rather than a hardcoded pair.
    /// </summary>
    /// <param name="segment">The source segment to serialize.</param>
    /// <param name="sourceLanguage">Source language code (e.g. the LanguageWeaver 3-letter code "eng").</param>
    /// <param name="targetLanguage">Target language code (e.g. "ger").</param>
    public SegmentSerializer(Segment segment, string sourceLanguage, string targetLanguage)
    {
        if (string.IsNullOrWhiteSpace(sourceLanguage))
            throw new ArgumentException("A source language code is required.", nameof(sourceLanguage));
        if (string.IsNullOrWhiteSpace(targetLanguage))
            throw new ArgumentException("A target language code is required.", nameof(targetLanguage));

        _sourceLanguage = sourceLanguage;
        _targetLanguage = targetLanguage;
        SerializedSegment = SerializeSegment(segment);
    }

    public string SerializedSegment { get; set; }

    /// <summary>Source language code this serializer was created with (e.g. "eng").</summary>
    public string SourceLanguage => _sourceLanguage;

    /// <summary>Target language code this serializer was created with (e.g. "ger").</summary>
    public string TargetLanguage => _targetLanguage;

    private List<SegmentElement> Elements { get; set; }

    /// <summary>
    /// Creates a fresh XLIFF <c>&lt;trans-unit&gt;</c> element for this segment with the given
    /// <paramref name="id"/>. Engines that batch several segments into one consolidated document
    /// (Edge) compose these elements directly via <see cref="BuildXliffDocument"/>, so there is no
    /// need to serialize each segment to a string and reparse it. The stored <c>&lt;source&gt;</c>
    /// is cloned so the same serializer can be reused without reparenting its element tree.
    /// </summary>
    public XElement CreateTransUnit(int id)
        => new XElement(Ns + "trans-unit",
            new XAttribute("id", id.ToString()),
            new XElement(_source));

    /// <summary>
    /// Wraps the supplied <paramref name="transUnits"/> into a complete XLIFF 1.2 document string
    /// for the given language direction. Shared by the single-segment (Cloud) and consolidated
    /// multi-segment (Edge) paths so the emitted wire envelope is identical.
    /// </summary>
    public static string BuildXliffDocument(string sourceLanguage, string targetLanguage, IEnumerable<XElement> transUnits)
    {
        var xliff = new XElement(Ns + "xliff",
            new XAttribute("version", "1.2"),
            new XElement(Ns + "file",
                new XAttribute("original", "segment"),
                new XAttribute("source-language", sourceLanguage),
                new XAttribute("target-language", targetLanguage),
                new XAttribute("datatype", "plaintext"),
                new XElement(Ns + "body", transUnits)));

        return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
               xliff.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Extracts the quality-estimation label that LanguageWeaver embeds in a translated XLIFF as the
    /// <c>match-quality</c> attribute of the <c>&lt;alt-trans&gt;</c> element (values "Good", "Adequate",
    /// "Poor"). This is the authoritative per-segment QE: it travels with the translated content itself,
    /// so it stays aligned even when segments are batched. Returns <c>null</c> when the engine does not
    /// emit QE (e.g. a non-QE model), in which case there is simply no estimation to report.
    /// </summary>
    public static string ExtractQualityEstimation(string xliff)
    {
        if (string.IsNullOrWhiteSpace(xliff))
            return null;

        var document = XDocument.Parse(xliff);
        var altTrans = document.Descendants().FirstOrDefault(e => e.Name.LocalName == "alt-trans");
        return altTrans?.Attribute("match-quality")?.Value;
    }

    /// <summary>
    /// Rebuilds a <see cref="Segment"/> from the XLIFF <c>&lt;target&gt;</c> returned by the
    /// translation engine. The original <see cref="Tag"/> instances captured during serialization
    /// are reused (looked up by anchor) so every tag's type and metadata is preserved exactly.
    /// </summary>
    public Segment DeserializeSegment(string segment)
    {
        if (Elements is null)
            throw new InvalidOperationException(
                "SerializeSegment must be called before DeserializeSegment.");

        var result = new Segment();

        // The engine can return an empty translation (e.g. null/missing result for a segment).
        // There is nothing to parse in that case, so return an empty segment.
        if (string.IsNullOrWhiteSpace(segment))
            return result;

        // Paired tags (Start/End) are keyed on (Anchor, IsStart).
        // Self-closing tags (Standalone, TextPlaceholder, LockedContent, ...) are keyed on Anchor.
        var sourceTags = Elements.OfType<Tag>().ToList();
        var pairedTags = sourceTags
            .Where(t => t.Type == TagType.Start || t.Type == TagType.End)
            .ToDictionary(t => (t.Anchor, IsStart: t.Type == TagType.Start));
        var selfClosingTags = sourceTags
            .Where(t => t.Type != TagType.Start && t.Type != TagType.End)
            .ToDictionary(t => t.Anchor);

        var document = XDocument.Parse(segment, LoadOptions.PreserveWhitespace);
        var target = document.Descendants().FirstOrDefault(e => e.Name.LocalName == "target")
                  ?? throw new InvalidOperationException(
                         "No <target> element found in the XLIFF translation response.");

        foreach (var node in target.Nodes())
            AppendNode(node, result, pairedTags, selfClosingTags);

        return result;
    }

    private static void AppendNode(
        XNode node,
        Segment result,
        IReadOnlyDictionary<(int Anchor, bool IsStart), Tag> pairedTags,
        IReadOnlyDictionary<int, Tag> selfClosingTags)
    {
        switch (node)
        {
            case XText text:
                result.Add(text.Value);
                break;

            // Paired inline tag: emit the original Start tag, recurse into the content, emit the End tag.
            case XElement element when element.Name.LocalName == "g":
            {
                var anchor = int.Parse(element.Attribute("id").Value);
                if (pairedTags.TryGetValue((anchor, true), out var startTag))
                    result.Add(startTag);

                foreach (var child in element.Nodes())
                    AppendNode(child, result, pairedTags, selfClosingTags);

                if (pairedTags.TryGetValue((anchor, false), out var endTag))
                    result.Add(endTag);
                break;
            }

            // Self-closing inline tag.
            case XElement element when element.Name.LocalName == "x":
            {
                var anchor = int.Parse(element.Attribute("id").Value);
                if (selfClosingTags.TryGetValue(anchor, out var tag))
                    result.Add(tag);
                break;
            }
        }
    }

    /// <summary>
    /// Serializes a <see cref="Segment"/> into an XLIFF 1.2 document. Paired Start/End tags become
    /// nested <c>&lt;g&gt;</c> elements and self-closing tags become <c>&lt;x&gt;</c> elements, keyed
    /// by tag anchor. Submitting tags as proper XLIFF inline markup (rather than HTML) lets the engine
    /// treat them as formatting rather than content, which preserves sentence-initial casing.
    /// </summary>
    private string SerializeSegment(Segment segment)
    {
        Elements = segment.Elements.ToList();

        var source = new XElement(Ns + "source");
        var open = new Stack<XElement>();
        var current = source;

        foreach (var element in Elements)
        {
            switch (element)
            {
                case Text text:
                    current.Add(new XText(text.Value));
                    break;

                case Tag tag when tag.Type == TagType.Start:
                    var group = new XElement(Ns + "g", new XAttribute("id", tag.Anchor));
                    current.Add(group);
                    open.Push(current);
                    current = group;
                    break;

                case Tag tag when tag.Type == TagType.End:
                    if (open.Count > 0) current = open.Pop();
                    break;

                case Tag tag:
                    current.Add(new XElement(Ns + "x", new XAttribute("id", tag.Anchor)));
                    break;
            }
        }

        _source = source;

        return BuildXliffDocument(_sourceLanguage, _targetLanguage, new[] { CreateTransUnit(1) });
    }
}