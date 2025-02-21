using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.Formatting;

namespace Multilingual.Excel.FileType.Constants
{
	public static class FormattingConstants
	{
		public const string ContentFormattingStart = "<cf{0}>";
		public const string ContentFormattingEnd = "</cf>";

		public const string ContentFormattingTag = "cf";
		public const string ContentFormattingTagPairTypeKey = "ContentFormattingTagPairTypeKey";

		public const string FormattingListMetaKey = "FormattingList";

		public const string FontScheme = "FontScheme";
		public const string FamilyName = "FontFamily";

		public static readonly Dictionary<string, string> TagContentNameFormats =
			new Dictionary<string, string>
			{
				{Bold.Name, " bold=\"{0}\""},
				{Italic.Name, " italic=\"{0}\""},
				{Underline.Name, " underline=\"{0}\""},
				{Strikethrough.Name, " strikethrough=\"{0}\""},
				{FontSize.Name, " size=\"{0}\""},
				{TextPosition.Name, " {0}=\"True\""},
				{TextColor.Name, " color=\"{0}\""},
				{FontName.Name, " font=\"{0}\""}
			};
	}
}
