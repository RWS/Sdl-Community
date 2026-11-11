using System;
using Multilingual.Excel.FileType.Models;

namespace Multilingual.Excel.FileType.Providers.OpenXml.Model
{
	public class ExcelCell: ICloneable
	{
		public string Value { get; set; }

		public Hyperlink Hyperlink { get; set; }

		public ExcelColumn Column { get; set; }

		public string Background { get; set; }

		/// <summary>
		/// The cell holds plain text that literally contains formatting tags (e.g. HTML), so the tags
		/// must be written back as text rather than converted to rich text formatting.
		/// </summary>
		public bool HasLiteralFormattingTags { get; set; }

		public object Clone()
		{
			return new ExcelCell
			{
				Background = Background,
				Value = Value,
				HasLiteralFormattingTags = HasLiteralFormattingTags,
				Hyperlink = Hyperlink,
				Column = Column.Clone() as ExcelColumn
			};
		}
	}
}
