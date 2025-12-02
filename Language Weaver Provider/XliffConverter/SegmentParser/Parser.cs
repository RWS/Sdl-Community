using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.XliffConverter.SegmentParser
{
    public class Parser
    {
        #region Tag Regex Patterns

        // Base pattern fragments
        private const string Anchor = @"(\d+)";
        private const string Alignment = @"(?:\s+x=(\d+))?";
        private const string Id = @"\s+id=([^ >]+)";
        private const string TextEquiv = @"\s+text-equiv\\?=""([^""]*?)""";
        private const string Locked = @"\s+locked=""true""";

        // Specific tag patterns
        private const string StartingTagPattern = $@"<{Anchor}{Alignment}{Id}\s*>";
        private const string EndingTagPattern = $@"</{Anchor}\s*>";
        private const string StandaloneTagPattern = $@"<{Anchor}{Alignment}{Id}\s*/>";
        private const string PlaceholderTagPattern = $@"<{Anchor}{Alignment}{Id}{TextEquiv}\s*/>";
        private const string LockedTagPattern = $@"<{Anchor}{Alignment}{Id}{TextEquiv}{Locked}\s*/>";

        // Compiled regex instances for individual detection
        private static readonly Regex StartingTag = new(StartingTagPattern, RegexOptions.Compiled);
        private static readonly Regex EndingTag = new(EndingTagPattern, RegexOptions.Compiled);
        private static readonly Regex StandaloneTag = new(StandaloneTagPattern, RegexOptions.Compiled);
        private static readonly Regex PlaceholderTag = new(PlaceholderTagPattern, RegexOptions.Compiled);
        private static readonly Regex LockedTag = new(LockedTagPattern, RegexOptions.Compiled);

        // Master regex used by SplitTags — uses the same exact patterns
        private static readonly Regex MasterTagRegex =
            new($"({LockedTagPattern}|{PlaceholderTagPattern}|{StandaloneTagPattern}|{StartingTagPattern}|{EndingTagPattern})",
                RegexOptions.Compiled);

        #endregion

        public static Segment ParseLine(string text)
        {
            var segment = new Segment();
            if (string.IsNullOrEmpty(text))
                return segment;

            var pieces = SplitTags(text);
            var startTags = new Stack<Tag>();

            foreach (var chunk in pieces)
            {
                if (string.IsNullOrEmpty(chunk))
                    continue;

                var tag = ParseTag(chunk);
                if (tag == null)
                {
                    segment.Add(chunk.Replace("\r\n", "\n"));
                    continue;
                }

                switch (tag.Type)
                {
                    case TagType.Start:
                        startTags.Push(tag);
                        break;

                    case TagType.End:
                        if (startTags.Count > 0 && tag.Anchor == startTags.Peek().Anchor)
                        {
                            var start = startTags.Pop();
                            tag.TagID = start.TagID;
                        }
                        break;
                }

                segment.Add(tag);
            }

            if (startTags.Count > 0)
                throw new Exception($"Line does not have matching starting/ending tags: {text}");

            return segment;
        }

        public static Tag ParseTag(string tag)
        {
            Match match;

            if ((match = StartingTag.Match(tag)).Success)
                return NewTag(TagType.Start, match);

            if ((match = EndingTag.Match(tag)).Success)
                return new Tag(TagType.End, "0", int.Parse(match.Groups[1].Value));

            if ((match = StandaloneTag.Match(tag)).Success)
                return NewTag(TagType.Standalone, match);

            if ((match = LockedTag.Match(tag)).Success)
                return NewTag(TagType.LockedContent, match, hasTextEquivalent: true);

            if ((match = PlaceholderTag.Match(tag)).Success)
                return NewTag(TagType.TextPlaceholder, match, hasTextEquivalent: true);

            return null;
        }

        private static List<string> SplitTags(string text)
        {
            var parts = new List<string>();
            int lastIndex = 0;

            foreach (Match match in MasterTagRegex.Matches(text))
            {
                if (match.Index > lastIndex)
                {
                    string plain = text.Substring(lastIndex, match.Index - lastIndex);
                    if (!string.IsNullOrEmpty(plain))
                        parts.Add(plain);
                }

                parts.Add(match.Value);
                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < text.Length)
                parts.Add(text.Substring(lastIndex));

            return parts;
        }

        private static Tag NewTag(TagType type, Match m, bool hasTextEquivalent = false)
        {
            string tagId = m.Groups[3].Value;
            int anchor = int.Parse(m.Groups[1].Value);
            int alignmentAnchor = !string.IsNullOrEmpty(m.Groups[2].Value) ? int.Parse(m.Groups[2].Value) : 0;
            string textEq = hasTextEquivalent && m.Groups.Count >= 5 ? m.Groups[4].Value : null;

            return new Tag(type, tagId, anchor, alignmentAnchor, textEq);
        }
    }
}