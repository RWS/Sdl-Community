using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Prepares an SDL <see cref="Segment"/> for plain-text translation by replacing each
    /// inline <see cref="Tag"/> with a numbered XML-style placeholder that the MT engine
    /// is expected to preserve verbatim. The original tag objects are kept in order so the
    /// translated text can be rehydrated into a target <see cref="Segment"/>.
    /// </summary>
    public sealed class SegmentTagPlacer
    {
        private const string PlaceholderPrefix = "lwtg";

        private static readonly Regex PlaceholderRegex = new(
            @"<" + PlaceholderPrefix + @"(\d+)\s*/?>",
            RegexOptions.Compiled);

        private readonly List<Tag> _tagsByIndex = new();

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
                    if (leadingText.Length > 0) segment.Add(leadingText);
                }

                if (int.TryParse(match.Groups[1].Value, out var tagIndex)
                    && tagIndex >= 0
                    && tagIndex < _tagsByIndex.Count) segment.Add(_tagsByIndex[tagIndex]);

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex >= translatedText.Length) return segment;

            var trailingText = translatedText.Substring(lastIndex);
            if (trailingText.Length > 0) segment.Add(trailingText);

            return segment;
        }

        private string BuildPreparedText(Segment sourceSegment)
        {
            var builder = new StringBuilder();
            foreach (var element in sourceSegment.Elements)
            {
                if (element is Tag tag)
                {
                    var placeholderIndex = _tagsByIndex.Count;
                    _tagsByIndex.Add(tag);
                    builder.Append('<').Append(PlaceholderPrefix).Append(placeholderIndex).Append("/>");
                    continue;
                }

                builder.Append(element);
            }

            return builder.ToString();
        }
    }
}