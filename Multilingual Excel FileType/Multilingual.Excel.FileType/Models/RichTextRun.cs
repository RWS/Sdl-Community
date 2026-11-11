namespace Multilingual.Excel.FileType.Models
{
	public class RichTextRun
	{
		public enum VerticalAlignment
		{
			Baseline,
			Superscript,
			Subscript
		}

		public string Text { get; set; }

		public bool Bold { get; set; }

		public bool Italic { get; set; }

		public bool Underline { get; set; }

		public bool Strikethrough { get; set; }

		public VerticalAlignment VertAlign { get; set; }

		public bool HasFormatting => Bold || Italic || Underline || Strikethrough || VertAlign != VerticalAlignment.Baseline;

		public bool HasSameFormatting(RichTextRun other)
		{
			return other != null
				   && Bold == other.Bold
				   && Italic == other.Italic
				   && Underline == other.Underline
				   && Strikethrough == other.Strikethrough
				   && VertAlign == other.VertAlign;
		}
	}
}
