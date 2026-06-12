using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Prepares an SDL <see cref="Segment"/> for translation by replacing inline
    /// <see cref="Tag"/> elements with XLIFF 1.2 self-closing placeholders
    /// (<c>&lt;x id="N"/&gt;</c>) that Language Weaver preserves when the
    /// request's input format is HTML. Text content is HTML-encoded before sending
    /// and decoded on the way back.
    ///
    /// Consecutive adjacent tags are collapsed into a single placeholder so that
    /// the API never receives two placeholders with no text between them (which
    /// would cause it to insert spurious spaces). The original tag objects are
    /// kept so the translated text can be rehydrated into a target <see cref="Segment"/>.
    ///
    /// Because <c>x</c> is not an HTML void element, the engine may echo a
    /// placeholder back in a different serialization (<c>&lt;x id="0"&gt;</c>,
    /// <c>&lt;x id=0/&gt;</c>, a separate <c>&lt;/x&gt;</c> close, ...), repeat it,
    /// or drop it altogether. Rehydration therefore matches placeholders leniently,
    /// honours only the first occurrence of each, and re-inserts dropped tag groups
    /// at the closest position that keeps the groups in source order, so tags are
    /// never lost and start/end pairs stay correctly nested.
    /// </summary>
    public sealed class SegmentTagPlacer
    {
        private static readonly Regex PlaceholderRegex = new(
            @"<x\s+id\s*=\s*[""']?(?<id>\d+)[""']?\s*/?\s*>|</x\s*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Each entry is a group of one-or-more consecutive source tags that were
        // collapsed into a single <x id="N"/> placeholder.
        private readonly List<List<Tag>> _tagGroups = new();

        public SegmentTagPlacer(Segment sourceSegment) => PreparedText = BuildPreparedText(sourceSegment);

        public string PreparedText { get; }

        public Segment BuildTargetSegment(string translatedText, CultureCode targetCulture)
        {
            var segment = new Segment(targetCulture);
            if (string.IsNullOrEmpty(translatedText)) return segment;

            var items = ParseTranslatedText(translatedText);
            InsertMissingTagGroups(items);

            foreach (var item in items)
            {
                if (item is int groupIndex)
                {
                    foreach (var tag in _tagGroups[groupIndex])
                        segment.Add(tag);
                }
                else if (item is string text && text.Length > 0)
                {
                    segment.Add(WebUtility.HtmlDecode(text));
                }
            }

            return segment;
        }

        // Splits the translated text into a flat list of items: text runs (string)
        // and recognized tag-group placeholders (int). Unknown ids, repeated ids
        // and stray </x> closers are dropped from the output entirely.
        private List<object> ParseTranslatedText(string translatedText)
        {
            var items = new List<object>();
            var seenGroups = new HashSet<int>();
            var lastIndex = 0;

            foreach (Match match in PlaceholderRegex.Matches(translatedText))
            {
                if (match.Index > lastIndex)
                {
                    items.Add(translatedText.Substring(lastIndex, match.Index - lastIndex));
                }

                var id = match.Groups["id"];
                if (id.Success
                    && int.TryParse(id.Value, out var groupIndex)
                    && groupIndex >= 0
                    && groupIndex < _tagGroups.Count
                    && seenGroups.Add(groupIndex))
                {
                    items.Add(groupIndex);
                }

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < translatedText.Length)
            {
                items.Add(translatedText.Substring(lastIndex));
            }

            return items;
        }

        // Re-inserts tag groups whose placeholders the engine did not echo back.
        // Each missing group goes between the placed groups with smaller and larger
        // indices: start-only groups as early as that window allows (so they open
        // before their pair closes), every other group as late as it allows.
        private void InsertMissingTagGroups(List<object> items)
        {
            for (var groupIndex = 0; groupIndex < _tagGroups.Count; groupIndex++)
            {
                if (items.Contains(groupIndex)) continue;

                var lowerBound = 0;
                var upperBound = items.Count;
                for (var i = 0; i < items.Count; i++)
                {
                    if (items[i] is not int placedGroup) continue;
                    if (placedGroup < groupIndex)
                    {
                        lowerBound = i + 1;
                    }
                    else
                    {
                        upperBound = i;
                        break;
                    }
                }

                if (upperBound < lowerBound) upperBound = lowerBound;

                var startTagsOnly = _tagGroups[groupIndex].All(tag => tag.Type == TagType.Start);
                items.Insert(startTagsOnly ? lowerBound : upperBound, groupIndex);
            }
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
