using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.XliffConverter.SegmentParser
{
	public class Parser
	{
		private static readonly Regex StartingTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)>");
		private static readonly Regex EndingTag = new(@"</(\d+)>");
		private static readonly Regex StandaloneTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)/>");
		private static readonly Regex PlaceholderTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+) text-equiv=""([\S\s]+)""/>");
		private static readonly Regex LockedTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+) text-equiv=""([\S\s]+)"" locked=""true""/>");

		public static Segment ParseLine(string text)
		{
			var segment = new Segment();
			if (string.IsNullOrEmpty(text))
			{
				return segment;
			}

			var tags = SplitTags(text);

			//var tags = Regex.Split(text, @"(<(?:""[^""]*""['""]*|'[^']*'['""]*|[^'"">])+>)");
			var startingTags = new Stack<Tag>();
			foreach (var tag in tags)
			{
				if (tag == string.Empty)
				{
					continue;
				}

				var parsedTag = ParseTag(tag);
				if (parsedTag == null)
				{
					segment.Add(tag.Replace("\r\n", "\n"));
					continue;
				}

				if (parsedTag.Type == TagType.Start)
				{
					startingTags.Push(parsedTag);
				}
				else if (parsedTag.Type == TagType.End && startingTags.Count == 0)
				{
					throw new Exception($"Line does not have matching starting and ending tags: {text}");
				}
				else if (parsedTag.Type == TagType.End)
				{
					if (parsedTag.Anchor == startingTags.Peek().Anchor)
					{
						var correspondingStartTag = startingTags.Pop();
						parsedTag.TagID = correspondingStartTag.TagID;
					}
					else
					{
						segment.Add(tag);
						continue;
					}
				}

				segment.Add(parsedTag);
			}

			if (startingTags.Count != 0)
			{
				throw new Exception($"Line does not have matching starting and ending tags: {text}");
			}

			return segment;
		}

		/// <summary>
		/// Converts text in a tag
		/// </summary>
		/// <param name="tag"></param>
		/// <returns cref="Tag"/>
		public static Tag ParseTag(string tag) 
		{
			Match match;
			if ((match = StartingTag.Match(tag)).Success)
			{
				const TagType TagType = TagType.Start;
				var tagId = match.Groups[3].Value;
				var anchor = int.Parse(match.Groups[1].Value);
				var alignmentAnchor = !string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0;
				return new Tag(TagType, tagId, anchor, alignmentAnchor, null);
			}

			if ((match = EndingTag.Match(tag)).Success)
			{
				const TagType TagType = TagType.End;
				const string TagId = "0";
				var anchor = int.Parse(match.Groups[1].Value);

				return new Tag(TagType, TagId, anchor);
			}

			if ((match = StandaloneTag.Match(tag)).Success)
			{
				const TagType TagType = TagType.Standalone;
				var tagId = match.Groups[3].Value;
				var anchor = int.Parse(match.Groups[1].Value);
				var alignmentAnchor = !string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0;

				return new Tag(TagType, tagId, anchor, alignmentAnchor, null);
			}

			if ((match = LockedTag.Match(tag)).Success)
			{
				const TagType TagType = TagType.LockedContent;
				var tagId = match.Groups[3].Value;
				var anchor = int.Parse(match.Groups[1].Value);
				var alignmentAnchor = !string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0;
				var textEquivalent = match.Groups[4].Value;

				return new Tag(TagType, tagId, anchor, alignmentAnchor, textEquivalent);
			}

            if ((match = PlaceholderTag.Match(tag)).Success)
            {
				const TagType TagType = TagType.TextPlaceholder;
				var tagId = match.Groups[3].Value;
	            var textEquivalent = match.Groups[4].Value;
				var anchor = int.Parse(match.Groups[1].Value);
				var alignmentAnchor = !string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0;

				return new Tag(TagType, tagId, anchor, alignmentAnchor, textEquivalent);
            }

			return null;
		}

		private static List<string> SplitTags(string text)
		{
			var result = new List<string>();

			var master = new Regex(
				$@"({StartingTag}|{EndingTag}|{StandaloneTag}|{PlaceholderTag}|{LockedTag})",
				RegexOptions.Compiled
			);

			int lastIndex = 0;
			foreach (Match match in master.Matches(text))
			{
				// Add plain text before the tag
				if (match.Index > lastIndex)
				{
					var plain = text.Substring(lastIndex, match.Index - lastIndex);
					if (!string.IsNullOrEmpty(plain))
						result.Add(plain);
				}

				result.Add(match.Value);

				lastIndex = match.Index + match.Length;
			}

			if (lastIndex < text.Length)
			{
				result.Add(text.Substring(lastIndex));
			}

			return result;
		}
	}
}