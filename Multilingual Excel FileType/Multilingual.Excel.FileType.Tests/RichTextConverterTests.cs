using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Services;
using Xunit;

namespace Multilingual.Excel.FileType.Tests
{
	public class RichTextConverterTests
	{
		[Fact]
		public void ToRuns_Superscript_SplitsIntoRuns()
		{
			var runs = RichTextConverter.ToRuns("1<sup>st</sup> place");

			Assert.Collection(runs,
				run => Assert.Equal(("1", RichTextRun.VerticalAlignment.Baseline), (run.Text, run.VertAlign)),
				run => Assert.Equal(("st", RichTextRun.VerticalAlignment.Superscript), (run.Text, run.VertAlign)),
				run => Assert.Equal((" place", RichTextRun.VerticalAlignment.Baseline), (run.Text, run.VertAlign)));
		}

		[Fact]
		public void ToRuns_NestedAndSynonymTags_CombineFormatting()
		{
			var runs = RichTextConverter.ToRuns("<strong>a<em>b<sub>c</sub></em></strong>");

			Assert.Equal(3, runs.Count);
			Assert.True(runs[0].Bold && !runs[0].Italic);
			Assert.True(runs[1].Bold && runs[1].Italic);
			Assert.True(runs[2].Bold && runs[2].Italic && runs[2].VertAlign == RichTextRun.VerticalAlignment.Subscript);
		}

		[Fact]
		public void ToRuns_KeepsTextVerbatim()
		{
			var runs = RichTextConverter.ToRuns("&lt;x&gt; <b>a</b> <i>b</i>");

			Assert.Equal(new[] { "&lt;x&gt; ", "a", " ", "b" }, runs.ConvertAll(run => run.Text));
		}

		[Theory]
		[InlineData("plain text")]
		[InlineData("a < b > c")]
		[InlineData("<b>unclosed")]
		[InlineData("<b>crossed<i></b></i>")]
		[InlineData("<span>unknown</span>")]
		public void ToRuns_NoFormattingOrMalformedTags_ReturnsNull(string content)
		{
			Assert.Null(RichTextConverter.ToRuns(content));
		}
	}
}
