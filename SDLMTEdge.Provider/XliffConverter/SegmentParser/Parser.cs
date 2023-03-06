using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.SegmentParser
{
	public class Parser
	{
		private static readonly Regex StartingTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)>");
        private static readonly Regex EndingTag = new(@"</(\d+)>");
        private static readonly Regex StandaloneTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)/>");
        private static readonly Regex PlaceholderTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+) text-equiv=""([\S\s]+)""/>");
        private static readonly Regex LockedTag = new(@"<(\d+) (?:x=(\d+) )?id=([^ />]+) text-equiv=""([\S\s]+)"" locked=""true""/>");

		public static Segment[] ParseFile(string path)
		{
            return File.Exists(path)
				 ? File.ReadAllLines(path).Select(ParseLine).ToArray()
                 : throw new FileNotFoundException("File not found: " + path);
        }

        public static Segment ParseLine(string text)
		{
			var segment = new Segment();
			// Allow for including any character as long as it's within quotes. If there are no quotes, default to
			// allow any character except >
			if (string.IsNullOrEmpty(text))
            {
                return segment;
            }

            var tags = Regex.Split(text, @"(<(?:""[^""]*""['""]*|'[^']*'['""]*|[^'"">])+>)");
			var startingTags = new Stack<Tag>();
			foreach (var tag in tags)
			{
				if (tag.Equals(string.Empty))
				{
					continue;
				}

				if (ParseTag(tag) is not Tag parsedTag)
				{
					segment.Add(tag.Replace("\r\n", "\n"));
					continue;
				}

				switch (parsedTag.Type)
				{
					case TagType.Start:
						startingTags.Push(parsedTag);
						break;
					case TagType.End when startingTags.Count == 0:
						throw new Exception($"Line does not have matching starting and ending tags: {text}");
					case TagType.End:
						{
							if (parsedTag.Anchor == startingTags.Peek().Anchor)
							{
								var correspondingStartTag = startingTags.Pop();
								parsedTag.TagID = correspondingStartTag.TagID;
								continue;
							}

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

		public static Tag ParseTag(string tag)
		{
			Match match;
			if ((match = StartingTag.Match(tag)).Success)
			{
				return new Tag(
				   TagType.Start,
				   match.Groups[3].Value,
				   int.Parse(match.Groups[1].Value),
				   !string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0,
				   null);
			}
			else if ((match = EndingTag.Match(tag)).Success)
			{
				return new Tag(
					TagType.End,
					"0",
					int.Parse(match.Groups[1].Value));
			}
			else if ((match = StandaloneTag.Match(tag)).Success)
			{
				return new Tag(
				   TagType.Standalone,
				   match.Groups[3].Value,
				   int.Parse(match.Groups[1].Value),
				   !string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0,
				   null);
			}
			else if ((match = LockedTag.Match(tag)).Success)
			{
				return new Tag(
					TagType.LockedContent,
					match.Groups[3].Value,
					int.Parse(match.Groups[1].Value),
					!string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0,
                    match.Groups[4].Value);
			}
			else if ((match = PlaceholderTag.Match(tag)).Success)
			{
				return new Tag(
					TagType.TextPlaceholder,
					match.Groups[3].Value,
					int.Parse(match.Groups[1].Value),
					!string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0,
                    match.Groups[4].Value);
			}

			return null;
		}
	}
}