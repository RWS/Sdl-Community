using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Prepares an SDL <see cref="Segment"/> for translation by replacing inline
    /// <see cref="Tag"/> elements with XLIFF 1.2 self-closing placeholders
    /// (<c>&lt;x id="N"/&gt;</c>) that Language Weaver preserves verbatim when the
    /// request's input format is HTML. Text content is HTML-encoded before sending
    /// and decoded on the way back.
    ///
    /// Consecutive adjacent tags are collapsed into a single placeholder so that
    /// the API never receives two placeholders with no text between them (which
    /// would cause it to insert spurious spaces). The original tag objects are
    /// kept so the translated text can be rehydrated into a target <see cref="Segment"/>.
    /// </summary>
    public sealed class SegmentTagPlacer
    {
        private static readonly Regex PlaceholderRegex = new(
            @"<x\s+id\s*=\s*[""'](\d+)[""']\s*/>",
            RegexOptions.Compiled);

        // Each entry is a group of one-or-more consecutive source tags that were
        // collapsed into a single <x id="N"/> placeholder.
        private readonly List<List<Tag>> _tagGroups = new();

        public SegmentTagPlacer(Segment sourceSegment) => PreparedText = BuildPreparedText(sourceSegment);

        public string PreparedText { get; }

        public Segment BuildTargetSegment(string translatedText, CultureCode targetCulture)
        {
            var segment = new Segment(targetCulture);
            if (string.IsNullOrEmpty(translatedText)) return segment;

            var lastIndex = 0;
            foreach (Match match in PlaceholderRegex.Matches(translatedText))
            {
                if (match.Index > lastIndex)
                {
                    var leadingText = translatedText.Substring(lastIndex, match.Index - lastIndex);
                    if (leadingText.Length > 0) segment.Add(WebUtility.HtmlDecode(leadingText));
                }

                if (int.TryParse(match.Groups[1].Value, out var groupIndex)
                    && groupIndex >= 0
                    && groupIndex < _tagGroups.Count)
                {
                    foreach (var tag in _tagGroups[groupIndex])
                        segment.Add(tag);
                }

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < translatedText.Length)
            {
                var trailingText = translatedText.Substring(lastIndex);
                if (trailingText.Length > 0) segment.Add(WebUtility.HtmlDecode(trailingText));
            }

            return segment;
        }

        private string BuildPreparedText(Segment sourceSegment)
        {
            var builder = new StringBuilder();
            var pendingGroup = new List<Tag>();

            foreach (var element in sourceSegment.Elements)
            {
                if (element is Tag tag)
                {
                    pendingGroup.Add(tag);
                    continue;
                }

                FlushTagGroup(builder, pendingGroup);
                builder.Append(WebUtility.HtmlEncode(element.ToString()));
            }

            FlushTagGroup(builder, pendingGroup);
            return builder.ToString();
        }

        private void FlushTagGroup(StringBuilder builder, List<Tag> pendingGroup)
        {
            if (pendingGroup.Count == 0) return;
            var groupIndex = _tagGroups.Count;
            _tagGroups.Add(new List<Tag>(pendingGroup));
            pendingGroup.Clear();
            builder.Append("<x id=\"").Append(groupIndex).Append("\"/>");
        }
    }
}
