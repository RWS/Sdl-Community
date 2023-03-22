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
		private static readonly Regex StartingTag = new Regex(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)>");
		private static readonly Regex EndingTag = new Regex(@"</(\d+)>");
		private static readonly Regex StandaloneTag = new Regex(@"<(\d+) (?:x=(\d+) )?id=([^ />]+)/>");
		private static readonly Regex PlaceholderTag = new Regex(@"<(\d+) (?:x=(\d+) )?id=([^ />]+) text-equiv=""([\S\s]+)""/>");
		private static readonly Regex LockedTag = new Regex(@"<(\d+) (?:x=(\d+) )?id=([^ />]+) text-equiv=""([\S\s]+)"" locked=""true""/>");

		/// <summary>
		/// Method used in Unit tests
		/// </summary>
		/// <param name="path"></param>
		/// <returns cref="Segment">Array of Segments</returns>
		public static Segment[] ParseFile(string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File not found: " + path);
			}
			var text = File.ReadAllLines(path);
			return text.Select(ParseLine).ToArray();
		}

		/// <summary>
		/// Converts plain text into Segment
		/// </summary>
		/// <param name="text">Text to be converted in Segment</param>
		/// <returns cref="Segment"/>
		public static Segment ParseLine(string text)
		{
			var segment = new Segment();
			// Allow for including any character as long as it's within quotes. If there are no quotes, default to
			// allow any character except >
			if (string.IsNullOrEmpty(text)) return segment;

			var tags = Regex.Split(text, @"(<(?:""[^""]*""['""]*|'[^']*'['""]*|[^'"">])+>)");
			var startingTags = new Stack<Tag>();
			foreach (var tag in tags)
			{
				if (tag == string.Empty)
					continue;

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
				return new Tag(TagType.Start, match.Groups[3].Value, int.Parse(match.Groups[1].Value),
					!string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0, null);
			}
			if ((match = EndingTag.Match(tag)).Success)
			{
				return new Tag(TagType.End, "0", int.Parse(match.Groups[1].Value));
			}
			if ((match = StandaloneTag.Match(tag)).Success)
			{
				return new Tag(TagType.Standalone, match.Groups[3].Value, int.Parse(match.Groups[1].Value),
					!string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0, null);
			}
			if ((match = LockedTag.Match(tag)).Success)
			{
				var textEquivalent = match.Groups[4].Value;
				return new Tag(TagType.LockedContent, match.Groups[3].Value, int.Parse(match.Groups[1].Value),
					!string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0, textEquivalent);
			}
			if ((match = PlaceholderTag.Match(tag)).Success)
			{
				var textEquivalent = match.Groups[4].Value;
				return new Tag(TagType.TextPlaceholder, match.Groups[3].Value, int.Parse(match.Groups[1].Value),
					!string.IsNullOrEmpty(match.Groups[2].Value) ? int.Parse(match.Groups[2].Value) : 0, textEquivalent);
			}

			return null;
		}
	}
}