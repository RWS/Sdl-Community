using System.Linq;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Xunit;

namespace XliffXsltPreviewSample.Tests
{
	public class SegmentMarkerTests
	{
		[Fact]
		public void ProcessParagraphUnit_PrefixesSourceAndTargetSegmentsWithTheirStudioIds()
		{
			var items = new DocumentItemFactory();
			var properties = new PropertiesFactory();
			var paragraph = items.CreateParagraphUnit(LockTypeFlags.Unlocked);
			var pair = items.CreateSegmentPairProperties();
			pair.Id = new SegmentId("7");
			var source = items.CreateSegment(pair);
			source.Add(items.CreateText(properties.CreateTextProperties("Hello")));
			paragraph.Source.Add(source);
			var target = items.CreateSegment(pair);
			target.Add(items.CreateText(properties.CreateTextProperties("Hallo")));
			paragraph.Target.Add(target);

			new SegmentMarker { ItemFactory = items, Output = new Sink() }
				.ProcessParagraphUnit(paragraph);

			var marker = "{{SDLSEG:" + paragraph.Properties.ParagraphUnitId.Id + "DELIMITER7}}";
			Assert.Equal(marker + "Hello", TextOf(source));
			Assert.Equal(marker + "Hallo", TextOf(target));
		}

		private static string TextOf(ISegment segment) =>
			string.Concat(segment.AllSubItems.OfType<IText>().Select(t => t.Properties.Text));

		private class Sink : AbstractBilingualContentHandler
		{
		}
	}
}
