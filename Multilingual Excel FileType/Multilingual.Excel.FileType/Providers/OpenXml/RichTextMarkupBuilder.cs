using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Multilingual.Excel.FileType.Providers.OpenXml
{
	/// <summary>
	/// Converts Excel rich text (shared or inline string runs) into formatting tag markup (e.g. 1&lt;sup&gt;st&lt;/sup&gt;),
	/// the inverse of <see cref="Services.RichTextConverter"/>.
	/// </summary>
	internal static class RichTextMarkupBuilder
	{
		private static readonly string[] TagOrder = { "b", "i", "u", "s", "sup", "sub" };

		/// <summary>
		/// Returns the markup for the rich text, or null when the content has no run formatting that can be
		/// represented as tags, or contains '&lt;' characters that would be ambiguous with tags.
		/// </summary>
		public static string GetMarkup(RstType richText, Font cellFont)
		{
			var runs = richText?.Elements<Run>().ToList();
			if (runs == null || runs.Count == 0)
			{
				return null;
			}

			var cellFontTags = GetTags(cellFont);

			// Runs without properties use the cell font; runs with properties define their own formatting
			var runTags = runs
				.Where(run => !string.IsNullOrEmpty(run.Text?.Text))
				.Select(run => (Text: run.Text.Text, Tags: run.RunProperties == null ? cellFontTags : GetTags(run.RunProperties)))
				.ToList();

			// Formatting shared by the cell font and all runs belongs to the cell, not to individual words
			var cellTags = cellFontTags.Where(tag => runTags.All(run => run.Tags.Contains(tag))).ToList();

			var markup = new StringBuilder();
			var openTags = new List<string>();
			var hasFormatting = false;

			foreach (var (text, allTags) in runTags)
			{
				if (text.IndexOf('<') >= 0)
				{
					return null;
				}

				var tags = allTags.Except(cellTags).ToList();
				hasFormatting |= tags.Count > 0;

				var keep = 0;
				while (keep < openTags.Count && tags.Contains(openTags[keep]))
				{
					keep++;
				}

				for (var i = openTags.Count - 1; i >= keep; i--)
				{
					markup.Append("</" + openTags[i] + ">");
				}

				openTags.RemoveRange(keep, openTags.Count - keep);

				foreach (var tag in tags.Where(tag => !openTags.Contains(tag)))
				{
					markup.Append("<" + tag + ">");
					openTags.Add(tag);
				}

				markup.Append(text);
			}

			for (var i = openTags.Count - 1; i >= 0; i--)
			{
				markup.Append("</" + openTags[i] + ">");
			}

			return hasFormatting ? markup.ToString() : null;
		}

		private static List<string> GetTags(OpenXmlElement properties)
		{
			var tags = new List<string>();
			if (properties == null)
			{
				return tags;
			}

			foreach (var element in properties.ChildElements)
			{
				switch (element)
				{
					case Bold bold when bold.Val == null || bold.Val.Value:
						tags.Add("b");
						break;
					case Italic italic when italic.Val == null || italic.Val.Value:
						tags.Add("i");
						break;
					case Underline underline when underline.Val == null || underline.Val.Value != UnderlineValues.None:
						tags.Add("u");
						break;
					case Strike strike when strike.Val == null || strike.Val.Value:
						tags.Add("s");
						break;
					case VerticalTextAlignment verticalAlignment when verticalAlignment.Val?.Value == VerticalAlignmentRunValues.Superscript:
						tags.Add("sup");
						break;
					case VerticalTextAlignment verticalAlignment when verticalAlignment.Val?.Value == VerticalAlignmentRunValues.Subscript:
						tags.Add("sub");
						break;
				}
			}

			return TagOrder.Where(tags.Contains).ToList();
		}
	}
}
