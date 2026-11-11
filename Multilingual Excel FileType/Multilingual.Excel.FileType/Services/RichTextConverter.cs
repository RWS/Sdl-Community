using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Multilingual.Excel.FileType.Models;

namespace Multilingual.Excel.FileType.Services
{
	/// <summary>
	/// Converts cell content containing formatting tags (e.g. &lt;b&gt;, &lt;sup&gt;) into Excel rich text runs.
	/// Text between the tags is preserved verbatim; unrecognized tags remain literal text.
	/// </summary>
	public static class RichTextConverter
	{
		private static readonly Regex FormattingTagRegex = new Regex(
			@"<(?<close>/)?(?<name>b|strong|i|em|u|ins|s|del|sub|sup)>",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Returns the rich text runs for the content, or null when the content has no formatting tags
		/// or the tags are not well-formed (in which case the content should be written as plain text).
		/// </summary>
		public static List<RichTextRun> ToRuns(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return null;
			}

			var matches = FormattingTagRegex.Matches(content);
			if (matches.Count == 0)
			{
				return null;
			}

			var runs = new List<RichTextRun>();
			var openTags = new Stack<string>();
			var position = 0;

			foreach (Match match in matches)
			{
				AddRun(runs, content.Substring(position, match.Index - position), openTags);
				position = match.Index + match.Length;

				var name = NormalizeTagName(match.Groups["name"].Value);
				if (match.Groups["close"].Success)
				{
					if (openTags.Count == 0 || openTags.Peek() != name)
					{
						return null;
					}

					openTags.Pop();
				}
				else
				{
					openTags.Push(name);
				}
			}

			if (openTags.Count > 0)
			{
				return null;
			}

			AddRun(runs, content.Substring(position), openTags);

			return runs.Any(run => run.HasFormatting) ? runs : null;
		}

		private static void AddRun(List<RichTextRun> runs, string text, Stack<string> openTags)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			var run = new RichTextRun { Text = text };

			// Apply outermost to innermost, so the innermost sub/sup wins
			foreach (var tag in openTags.Reverse())
			{
				switch (tag)
				{
					case "b":
						run.Bold = true;
						break;
					case "i":
						run.Italic = true;
						break;
					case "u":
						run.Underline = true;
						break;
					case "s":
						run.Strikethrough = true;
						break;
					case "sub":
						run.VertAlign = RichTextRun.VerticalAlignment.Subscript;
						break;
					case "sup":
						run.VertAlign = RichTextRun.VerticalAlignment.Superscript;
						break;
				}
			}

			var previous = runs.LastOrDefault();
			if (previous != null && previous.HasSameFormatting(run))
			{
				previous.Text += run.Text;
				return;
			}

			runs.Add(run);
		}

		private static string NormalizeTagName(string name)
		{
			switch (name.ToLowerInvariant())
			{
				case "strong":
					return "b";
				case "em":
					return "i";
				case "ins":
					return "u";
				case "del":
					return "s";
				default:
					return name.ToLowerInvariant();
			}
		}
	}
}
