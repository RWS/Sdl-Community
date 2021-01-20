﻿using System.Text;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Converter
{
	public static  class Extensions
	{
		public static string ToXliffString(this Segment segment)
		{
			// No matter what, always encode < to &lt; so xliff doesn't recognize this as part of a tag
			if (!segment.HasTags)
				return segment.ToPlain().Replace("<", "&lt;");

			var result = new StringBuilder();
			foreach (var element in segment.Elements)
			{
				if (element is Text txt)
				{
					result.Append(txt.Value.Replace("<", "&lt;"));
				}
				else if (element is Tag)
				{
					var tag = (Tag)element;
					var tagString = tag.ToString().Replace("<", "&lt;");
					switch (tag.Type)
					{
						case TagType.Start:
							result.Append($"<bpt id=\"{ tag.TagID}\">{tagString}</bpt>");
							break;
						case TagType.End:
							result.Append($"<ept id=\"{tag.TagID}\">{tagString}</ept>");
							break;
						case TagType.UnmatchedStart:
						case TagType.UnmatchedEnd:
							result.Append($"<it id=\"{tag.TagID}\">{tagString}</it>");
							break;
						case TagType.Standalone:
						case TagType.TextPlaceholder:
						case TagType.LockedContent:
							result.Append($"<x id=\"{tag.TagID}\">{tagString}</x>");
							break;
						default:
							System.Diagnostics.Debug.Assert(false, "Unexpected tag type");
							break;
					}
				}
			}
			return result.ToString();
		}

		// Encodes plain text string so that text like <PROJECT> will become safe for being within xliff. Our main
		// concern is just strings that contain non-trados tags.
	}
}
