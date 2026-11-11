using DocumentFormat.OpenXml.Spreadsheet;
using Multilingual.Excel.FileType.Providers.OpenXml;
using Xunit;

namespace Multilingual.Excel.FileType.Tests
{
	public class RichTextMarkupBuilderTests
	{
		[Fact]
		public void GetMarkup_SuperscriptRun_ReturnsSupTag()
		{
			var richText = new SharedStringItem(
				new Run(new Text("1")),
				new Run(new RunProperties(new VerticalTextAlignment { Val = VerticalAlignmentRunValues.Superscript }), new Text("st")),
				new Run(new RunProperties(new FontSize { Val = 11 }), new Text(" place")));

			Assert.Equal("1<sup>st</sup> place", RichTextMarkupBuilder.GetMarkup(richText, new Font()));
		}

		[Fact]
		public void GetMarkup_AdjacentRunsWithSharedFormatting_KeepTagsOpen()
		{
			var richText = new SharedStringItem(
				new Run(new RunProperties(new Bold()), new Text("a")),
				new Run(new RunProperties(new Bold(), new Italic()), new Text("b")),
				new Run(new Text(" ") { Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve }),
				new Run(new RunProperties(new Underline()), new Text("c")));

			Assert.Equal("<b>a<i>b</i></b> <u>c</u>", RichTextMarkupBuilder.GetMarkup(richText, new Font()));
		}

		[Fact]
		public void GetMarkup_FormattingOfWholeCell_IsNotTagged()
		{
			var boldCellFont = new Font(new Bold());
			var richText = new SharedStringItem(
				new Run(new Text("H")),
				new Run(new RunProperties(new Bold(), new VerticalTextAlignment { Val = VerticalAlignmentRunValues.Subscript }), new Text("2")),
				new Run(new RunProperties(new Bold()), new Text("O")));

			Assert.Equal("H<sub>2</sub>O", RichTextMarkupBuilder.GetMarkup(richText, boldCellFont));
		}

		[Fact]
		public void GetMarkup_PartialBoldInBoldStyledCell_TagsOnlyBoldRun()
		{
			var boldCellFont = new Font(new Bold());
			var richText = new SharedStringItem(
				new Run(new Text("Hello")),
				new Run(new RunProperties(new FontSize { Val = 11 }), new Text(" world")));

			Assert.Equal("<b>Hello</b> world", RichTextMarkupBuilder.GetMarkup(richText, boldCellFont));
		}

		[Fact]
		public void GetMarkup_TextContainsLessThan_ReturnsNull()
		{
			var richText = new SharedStringItem(
				new Run(new Text("a < ")),
				new Run(new RunProperties(new Bold()), new Text("b")));

			Assert.Null(RichTextMarkupBuilder.GetMarkup(richText, new Font()));
		}

		[Fact]
		public void GetMarkup_NoRunFormatting_ReturnsNull()
		{
			Assert.Null(RichTextMarkupBuilder.GetMarkup(new SharedStringItem(new Text("plain")), new Font()));
			Assert.Null(RichTextMarkupBuilder.GetMarkup(new SharedStringItem(
				new Run(new Text("a")), new Run(new RunProperties(new FontSize { Val = 11 }), new Text("b"))), new Font()));
		}
	}
}
